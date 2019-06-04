using MobileBackend.DataAccess;
using System;
using System.Linq;
using System.Web.Http;
using MobileApp.Models;

namespace MobileBackend.Controllers
{
    public class WorkController : ApiController
    {
        public string[] GetAll()
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            string[] WorkAssignments = null;
            try
            {
                WorkAssignments = (from wa in entities.WorkAssignments
                                   where (wa.Active == true) && 
                                   (wa.InProgressAt == null)
                                   select wa.Title + " Deadline: " 
                                   + wa.Deadline + " ( " + wa.WorkAssignmentId + " )")
                                   .ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return WorkAssignments;
        }
        public string[] GetWorksInProgress(int id)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            id = 5;
            try
            {
                string[] chosenWorkData = (from cw in entities.WorkAssignments
                                           where (cw.Active == true) && 
                                           (cw.InProgressAt != null) && 
                                           (cw.Completed != true)
                                           select cw.Title + " | Started at:  " 
                                           + cw.InProgressAt + " ( " + cw.WorkAssignmentId + " )")
                                           .ToArray();

                return chosenWorkData;
            }
            finally
            {
                entities.Dispose();
            }
        }
        public WorkModel GetModel(string workName)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                string chosenWorkId = workName;
                WorkAssignments chosenWorkData = (from cw in entities.WorkAssignments
                                                 where (cw.Active == true) && 
                                                 (cw.WorkAssignmentId.ToString() == chosenWorkId)
                                                 select cw).FirstOrDefault();

                WorkModel chosenWorkModel = new WorkModel()
                {
                    WorkTitle = chosenWorkData.Title,
                    Description = chosenWorkData.Description,
                    Deadline = chosenWorkData.Deadline.Value
                };
                return chosenWorkModel;
            }
            finally
            {
                entities.Dispose();
            }
        }
        [HttpPost]
        public bool PostWork(WorkModel model)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            Customers customer = (from cu in entities.Customers
                                  where (cu.Active == true) && 
                                  (cu.CustomerName == model.CustomerName)
                                  select cu).FirstOrDefault();

            try
            {
                if (model.Operation == "Save")
                {
                    WorkAssignments newEntry = new WorkAssignments()
                    {
                        CustomerId = customer.CustomerId,
                        Title = model.WorkTitle,
                        Description = model.Description,
                        Deadline = model.Deadline,
                        InProgress = true,
                        CreatedAt = DateTime.Now,
                        Active = true
                    };
                    entities.WorkAssignments.Add(newEntry);
                }
                else if (model.Operation == "Modify")
                {
                    WorkAssignments existing = (from wa in entities.WorkAssignments
                                                where (wa.WorkAssignmentId == model.WorkId) && 
                                                (wa.Active == true)
                                                select wa).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.Title = model.WorkTitle;
                        existing.Description = model.Description;
                        existing.Deadline = model.Deadline;
                        existing.LastModified = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (model.Operation == "Delete")
                {
                    WorkAssignments existing = (from wa in entities.WorkAssignments
                                                where (wa.WorkAssignmentId == model.WorkId)
                                                select wa).FirstOrDefault();

                    if (existing != null)
                    {
                        entities.WorkAssignments.Remove(existing);
                    }
                    else
                    {
                        return false;
                    }
                }
      
                else if (model.Operation == "Assign")
                {
                    WorkAssignments assignment = (from wa in entities.WorkAssignments
                                                  where (wa.WorkAssignmentId == model.WorkId) && 
                                                  (wa.Active == true) && (wa.InProgress == true)
                                                  select wa).FirstOrDefault();

                    if (assignment == null)
                    {
                        return false;
                    }
                    Employees emp = (from e in entities.Employees
                                     where (e.EmployeeId == model.EmployeeId)
                                     select e).FirstOrDefault();

                    if (emp == null)
                    {
                        return false;
                    }
                    int workId = assignment.WorkAssignmentId;
                    int customerId = assignment.CustomerId.Value;
                    assignment.InProgressAt = DateTime.Now;
                    Timesheets newEntry = new Timesheets()
                    {
                        CustomerId = customerId,
                        ContractorId = emp.ContractorId,
                        EmployeeId = emp.EmployeeId,
                        WorkAssignmentId = workId,
                        StartTime = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        Active = true,
                        WorkComplete = false
                    };
                    entities.Timesheets.Add(newEntry);
                }
                else if (model.Operation == "MarkComplete")
                {
                    WorkAssignments assignment = (from wa in entities.WorkAssignments
                                                  where (wa.WorkAssignmentId == model.WorkId) && 
                                                  (wa.Active == true) && 
                                                  (wa.InProgress == true) && 
                                                  (wa.InProgressAt != null)
                                                  select wa).FirstOrDefault();

                    if (assignment == null)
                    {
                        return false;
                    }
                    int workId = assignment.WorkAssignmentId;
                    int customerId = assignment.CustomerId.Value;
                    assignment.CompletedAt = DateTime.Now;
                    assignment.Completed = true;
                    assignment.InProgress = false;

                    Timesheets existing = (from ts in entities.Timesheets
                                           where (ts.WorkAssignmentId == workId) && 
                                           (ts.CustomerId == customerId)
                                           select ts).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.WorkComplete = true;
                        existing.StopTime = DateTime.Now;
                        existing.LastModified = DateTime.Now;
                        existing.Comments = "Work set to completed by Admin";
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
