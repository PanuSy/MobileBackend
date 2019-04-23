using System;
using System.Collections.Generic;
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
            PanconDatabaseEntities entities = new PanconDatabaseEntities();
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
    }
}
