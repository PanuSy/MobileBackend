using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MobileBackend.DataAccess;
using MobileApp.Models;

namespace MobileBackend.Controllers
{
    public class TimesheetController : ApiController
    {
        public List<string[]> GetPickerData(int picker)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            List<string[]> pickerData = new List<string[]>();
            string[] emp = { "Employees:---------- " };
            string[] work = { "Workassignments:---------- " };
            string[] cont = { "Contractors:---------- " };
            try
            {
                string[] workAssignments = (from wa in entities.WorkAssignments
                                            where (wa.Active == true)
                                            select wa.Title).ToArray();

                string[] employees = (from e in entities.Employees
                                      where (e.Active == true)
                                      select e.Firstname + " " + e.Lastname).ToArray();

                string[] contractors = (from co in entities.Contractors
                                        where (co.Active == true)
                                        select co.CompanyName).ToArray();

                pickerData.Add(work);
                pickerData.Add(workAssignments);
                pickerData.Add(emp);
                pickerData.Add(employees);
                pickerData.Add(cont);
                pickerData.Add(contractors);
            }
            finally
            {
                entities.Dispose();
            }
            return pickerData;
        }
        [HttpGet]
        public List<string> TimeSheetList(int tsID)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            List<string> realList = new List<string>();
            tsID = 2;
            string[] sheetId = (from ts in entities.Timesheets
                                where (ts.WorkComplete == true)
                                select ts.ContractorId.ToString() 
                                + " " + ts.EmployeeId.ToString() 
                                + " " + ts.WorkAssignmentId.ToString()).ToArray();
            try
            {
                foreach (var item in sheetId)
                {
                    string[] data = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    string contractor = data[0];
                    string contr = (from c in entities.Contractors
                                    where (c.ContractorId.ToString() == contractor)
                                    select c.CompanyName).Single();

                    string employee = data[1];
                    string emp = (from e in entities.Employees
                                  where (e.EmployeeId.ToString() == employee)
                                  select e.Firstname + " " + e.Lastname).Single();

                    string workid = data[2];
                    string work = (from w in entities.WorkAssignments
                                   where (w.WorkAssignmentId.ToString() == workid)
                                   select w.Title).Single();

                    realList.Add(contr + " | " + emp + " | " + work + " " + "( " + workid + " )");
                }
            }
            finally
            {
                entities.Dispose();
            }
            return realList;
        }
        public List<string> GetChosenEntity(string Entity)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            string[] sheet = null;
            string[] workAssignments = (from wa in entities.WorkAssignments
                                        where (wa.Active == true)
                                        select wa.Title).ToArray();

            string[] employees = (from e in entities.Employees
                                  where (e.Active == true)
                                  select e.Firstname + " " + e.Lastname).ToArray();

            string[] contractors = (from cont in entities.Contractors
                                    where (cont.Active == true)
                                    select cont.CompanyName).ToArray();

            List<string[]> empData = new List<string[]>();
            List<string[]> contData = new List<string[]>();
            List<string[]> WorkData = new List<string[]>();
            WorkData.Add(workAssignments);
            empData.Add(employees);
            contData.Add(contractors);

            foreach (var item in empData)
            {
                foreach (var i in item)
                {
                    if (i.ToString() == Entity)
                    {
                        string emplo = (from e in entities.Employees
                                        where (e.Firstname + " " + e.Lastname == Entity)
                                        select e.EmployeeId.ToString()).Single();

                        sheet = (from ts in entities.Timesheets
                                 where (ts.EmployeeId.ToString() == emplo)
                                 select ts.ContractorId.ToString() 
                                 + " " + ts.EmployeeId.ToString() + " " 
                                 + ts.WorkAssignmentId.ToString()).ToArray();
                    }
                }
            }
            foreach (var itemm in WorkData)
            {
                foreach (var it in itemm)
                {
                    if (it.ToString() == Entity)
                    {
                        string worka = (from w in entities.WorkAssignments
                                        where (w.Title == Entity)
                                        select w.WorkAssignmentId.ToString()).Single();

                        sheet = (from ts in entities.Timesheets
                                 where (ts.WorkAssignmentId.ToString() == worka)
                                 select ts.ContractorId.ToString() 
                                 + " " + ts.EmployeeId.ToString() 
                                 + " " + ts.WorkAssignmentId.ToString()).ToArray();
                    }
                }
            }
            foreach (var iten in contData)
            {
                foreach (var ite in iten)
                {
                    if (ite.ToString() == Entity)
                    {
                        string contra = (from c in entities.Contractors
                                         where (c.CompanyName == Entity)
                                         select c.ContractorId.ToString()).Single();

                        sheet = (from ts in entities.Timesheets
                                 where (ts.ContractorId.ToString() == contra)
                                 select ts.ContractorId.ToString() 
                                 + " " + ts.EmployeeId.ToString() 
                                 + " " + ts.WorkAssignmentId.ToString()).ToArray();
                    }
                }
            }
            List<string> entityList = new List<string>();
            try
            {
                foreach (var item in sheet)
                {
                    string[] datas = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    string contractor = datas[0];
                    string contr = (from c in entities.Contractors
                                    where (c.ContractorId.ToString() == contractor)
                                    select c.CompanyName).Single();

                    string employee = datas[1];
                    string emp = (from e in entities.Employees
                                  where (e.EmployeeId.ToString() == employee)
                                  select e.Firstname + " " + e.Lastname).Single();

                    string workid = datas[2];
                    string work = (from w in entities.WorkAssignments
                                   where (w.WorkAssignmentId.ToString() == workid)
                                   select w.Title).Single();

                    entityList.Add(contr + " | " + emp + " | " + work + " " + "( " + workid + " )");
                }
            }
            finally
            {
                entities.Dispose();
            }
            return entityList;
        }
        public WorkModel GetDetailModel(string Details)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            string[] Sheet = Details.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            int count = Sheet.Count();
            string workid = Sheet[count - 2];

            Timesheets sheet = (from ts in entities.Timesheets
                               where (ts.WorkAssignmentId.ToString() == workid)
                               select ts).FirstOrDefault();

            string cont = (from con in entities.Contractors
                           where (con.ContractorId == sheet.ContractorId)
                           select con.CompanyName).Single();

            string cust = (from c in entities.Customers
                           where (c.CustomerId == sheet.CustomerId)
                           select c.CustomerName).Single();

            string work = (from co in entities.WorkAssignments
                           where (co.WorkAssignmentId == sheet.WorkAssignmentId)
                           select co.Title).Single();

            string emp = (from e in entities.Employees
                          where (e.EmployeeId == sheet.EmployeeId)
                          select e.Firstname + " " + e.Lastname).Single();

            double workTime = (sheet.StopTime.Value - sheet.StartTime.Value).TotalHours;

            string comments = (from ct in entities.Timesheets
                               where (ct.TimesheetId == sheet.TimesheetId)
                               select ct.Comments).Single();

            WorkModel details = new WorkModel()
            {
                CustomerName = cust,
                ContractorName = cont,
                Firstname = emp,
                WorkTitle = work,
                CountedHours = workTime,
                Comments = comments
            };
            return details;
        }
    }
}