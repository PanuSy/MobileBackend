using MobileBackend.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http;
using MobileApp.Models;

namespace MobileBackend.Controllers
{
    public class EmployeeController : ApiController
    {
        /*public string[] GetAll()
        {

            string[] employeeNames = null;
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                employeeNames = (from e in entities.Employees
                                 where (e.Active == true)
                                 select e.Firstname + " " +
                                     e.Lastname).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return employeeNames;
        }*/
        public byte[] GetEmployeeImage(string employeeName)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                string[] nameParts = employeeName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string first = nameParts[0];
                string last = nameParts[1];
                byte[] bytes = (from e in entities.Employees
                                where (e.Active == true) &&
                                (e.Firstname == first) &&
                                (e.Lastname == last)
                                select e.EmployeePicture).FirstOrDefault();

                return bytes;
            }
            finally
            {
                entities.Dispose();
            }
        }
        public string PutEmployeeImage()
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                Employees newEmployee = new Employees()
                {
                    Firstname = "X",
                    Lastname = "MAN",
                    EmployeePicture = File.ReadAllBytes(@"C:\Users\Admin\Pictures\car.png")
                };
                entities.Employees.Add(newEmployee);
                entities.SaveChanges();

                return "OK!";
            }
            finally
            {
                entities.Dispose();
            }
            return "Error!";
        }
        public List<string> GetAll()
        {
            string[] employees = null;
            panconDatabaseEntities entities = new panconDatabaseEntities();
            List<string> empList = new List<string>();
            employees = (from e in entities.Employees where (e.Active == true) select e.ContractorId + " " + e.EmployeeId).ToArray();
            try
            {
                foreach (var item in employees)
                {
                    string[] data = item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string contractor = data[0];
                    string contr = (from c in entities.Contractors where (c.ContractorId.ToString() == contractor) select c.CompanyName).Single();
                    string employee = data[1];
                    string emp = (from e in entities.Employees where (e.EmployeeId.ToString() == employee) select e.Firstname + " " + e.Lastname).Single();
                    empList.Add("Contractor: " + contr + " | Name: " + emp + " ( " + employee + " )");
                }
            }
            finally
            {
                entities.Dispose();
            }
            return empList;
        }
        public WorkModel GetModel(string employeeName)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                string chosenEmployee = employeeName;
                Employees employee = (from e in entities.Employees
                                     where (e.Active == true) && 
                                     (e.EmployeeId.ToString() == chosenEmployee)
                                     select e).FirstOrDefault();

                WorkModel chosenEmployeeModel = new WorkModel()
                {
                    Firstname = employee.Firstname,
                    Lastname = employee.Lastname,
                    Phone = employee.Phone,
                    Email = employee.Email,
                    Picture = employee.EmployeePicture
                };
                return chosenEmployeeModel;
            }
            finally
            {
                entities.Dispose();
            }
        }
        [HttpPost]
        public bool PostEmployee(WorkModel model)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                if (model.EmpOperation == "Save")
                {
                    Contractors contractor = (from c in entities.Contractors
                                             where (c.CompanyName == model.ContractorName)
                                             select c).FirstOrDefault();

                    Employees newEntry = new Employees()
                    {
                        ContractorId = contractor.ContractorId,
                        Username = model.Username,
                        Password = model.Password,
                        Firstname = model.Firstname,
                        Lastname = model.Lastname,
                        Phone = model.Phone.ToString(),
                        Email = model.Email,
                        CreatedAt = DateTime.Now,
                        Active = true,
                    };
                    entities.Employees.Add(newEntry);
                }
                else if (model.EmpOperation == "Modify")
                {
                    Employees existing = (from e in entities.Employees
                                         where (e.EmployeeId == model.EmployeeId) && 
                                         (e.Active == true)
                                         select e).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.Firstname = model.Firstname;
                        existing.Lastname = model.Lastname;
                        existing.Phone = model.Phone.ToString();
                        existing.Email = model.Email;
                        existing.LastModified = DateTime.Now;
                        existing.EmployeePicture = model.Picture;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (model.EmpOperation == "Delete")
                {
                    Employees existing = (from e in entities.Employees
                                         where (e.EmployeeId == model.EmployeeId)
                                         select e).FirstOrDefault();

                    if (existing != null)
                    {
                        entities.Employees.Remove(existing);
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