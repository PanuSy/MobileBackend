
using MobileBackend.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MobileBackend.Controllers
{
    public class EmployeeController : ApiController
    {
        public string[] GetAll()
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
        }
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
    }
}
