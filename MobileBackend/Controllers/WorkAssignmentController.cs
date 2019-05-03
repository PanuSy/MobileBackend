
using MobileApp.Models;
using MobileBackend.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                int assignmentId = assignment.WorkAssignmentId;

                Timesheets NewEntry = new Timesheets()
                {
                    WorkAssignmentId = assignmentId,
                    StartTime = DateTime.Now,
                    Active = true,
                    CreatedAt = DateTime.Now
                };
                entities.Timesheets.Add(NewEntry);
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
