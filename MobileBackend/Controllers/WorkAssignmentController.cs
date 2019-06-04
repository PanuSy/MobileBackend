using MobileApp.Models;
using MobileBackend.DataAccess;
using System;
using System.Linq;
using System.Web.Http;

namespace MobileBackend.Controllers
{
    public class WorkAssignmentController : ApiController
    {
        public string[] GetAll()
        {

            string[] assignmentNames = null;
            panconDatabaseEntities entities = new panconDatabaseEntities();

            try
            {
                assignmentNames = (from wa in entities.WorkAssignments
                                 where (wa.Active == true)
                                 select wa.Title).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return assignmentNames;
        }

        [HttpPost]
        public bool PostStatus(WorkAssignmentOperationModel input)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            try
            {
                WorkAssignments assignment = (from wa in entities.WorkAssignments
                                   where (wa.Active == true) && 
                                   (wa.Title == input.AssignmentTitle)
                                   select wa).FirstOrDefault();

                if (assignment == null)
                {
                    return false;
                }
                if (input.Operation == "Start")
                {
                    int assignmentId = assignment.WorkAssignmentId;

                    Timesheets NewEntry = new Timesheets()
                    {
                        WorkAssignmentId = assignmentId,
                        StartTime = DateTime.Now,
                        WorkComplete = false,
                        Active = true,
                        CreatedAt = DateTime.Now
                    };
                    entities.Timesheets.Add(NewEntry);

                    assignment.InProgress = true;
                    assignment.InProgressAt = DateTime.Now;
                    assignment.LastModified = DateTime.Now;
                }
                else if (input.Operation == "Stop")
                {
                    int assignmentId = assignment.WorkAssignmentId;

                    Timesheets existing = (from ts in entities.Timesheets
                                           where (ts.WorkAssignmentId == assignmentId) &&
                                           (ts.Active == true) && (ts.WorkComplete == false)
                                           orderby ts.StartTime descending
                                           select ts).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.StopTime = DateTime.Now;
                        existing.WorkComplete = true;
                        existing.LastModified = DateTime.Now;

                        assignment.InProgress = false;
                        assignment.Completed = true;
                        assignment.CompletedAt = DateTime.Now;
                        assignment.LastModified = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                }
                entities.SaveChanges();
            }
            catch
            {
                return false;
            }
            finally
            {
                entities.Dispose();
            }

            return true;
        }
    }
}
