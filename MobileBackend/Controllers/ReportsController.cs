using MobileBackend.DataAccess;
using MobileBackend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MobileBackend.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult HoursPerWorkAssignment()
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tommorrow = today.AddDays(1);

                List<Timesheets> allTimesheetsToday = (from ts in entities.Timesheets
                                                       where (ts.StartTime > today) &&
                                                       (ts.StartTime < tommorrow) &&
                                                       (ts.WorkComplete == true)
                                                       select ts).ToList();

                List<HoursPerWorkAssignmentModel> model = new List<HoursPerWorkAssignmentModel>();

                foreach (Timesheets timesheet in allTimesheetsToday)
                {
                    int assignmentId = timesheet.WorkAssignmentId.Value;
                    HoursPerWorkAssignmentModel existing = model.Where(
                        m => m.WorkAssignmentId == assignmentId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.TotalHours += (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours;
                    }
                    else
                    {
                        existing = new HoursPerWorkAssignmentModel()
                        {
                            WorkAssignmentId = assignmentId,
                            WorkAssignmentName = timesheet.WorkAssignments.Title,
                            TotalHours = (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours
                        };
                        model.Add(existing);
                    }
                }
                return View(model);
            }
            finally
            {
                entities.Dispose();
            }
        }
        public ActionResult HoursPerWorkAssignmentToExcelAsCsv()
        {
            StringBuilder csv = new StringBuilder();

            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tommorrow = today.AddDays(1);

                List<Timesheets> allTimesheetsToday = (from ts in entities.Timesheets
                                                       where (ts.StartTime > today) &&
                                                       (ts.StartTime < tommorrow) &&
                                                       (ts.WorkComplete == true)
                                                       select ts).ToList();
                foreach (Timesheets timesheet in allTimesheetsToday)
                {
                    csv.AppendLine(timesheet.WorkAssignmentId + ";" +
                        timesheet.StartTime + ";" + timesheet.StopTime + ";");
                }
            }
            finally
            {
                entities.Dispose();
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Työtunnit.csv");
        }
        public ActionResult GetTimesheetsCount(string onlyComplete)
        {
            ReportChartDataViewModel model = new ReportChartDataViewModel();
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                model.Labels = (from wa in entities.WorkAssignments
                                orderby wa.WorkAssignmentId
                                select wa.Title).ToArray();

                if (onlyComplete == "1")
                {
                    model.Counts = (from ts in entities.Timesheets
                                    where (ts.WorkComplete == true)
                                    orderby ts.WorkAssignmentId
                                    group ts by ts.WorkAssignmentId into grp
                                    select grp.Count()).ToArray();
                }
                else
                {
                    model.Counts = (from ts in entities.Timesheets
                                    orderby ts.WorkAssignmentId
                                    group ts by ts.WorkAssignmentId into grp
                                    select grp.Count()).ToArray();
                }
            }
            finally
            {
                entities.Dispose();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}