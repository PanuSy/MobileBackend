using MobileBackend.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MobileBackend.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {

            panconDatabaseEntities entities = new panconDatabaseEntities();
            List<Employees> model = entities.Employees.ToList();
            entities.Dispose();

            return View(model);
        }

        public JsonResult GetList()
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            var model = (from e in entities.Employees
                         select new
                         {
                             EmployeeId = e.EmployeeId,
                             Firstname = e.Firstname,
                             Lastname = e.Lastname

                         }).ToList();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSingle(int id)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            
            var model = (from e in entities.Employees
                         where e.EmployeeId == id
                         select new
                         {
                             EmployeeId = e.EmployeeId,
                             Firstname = e.Firstname,
                             Lastname = e.Lastname

                         }).FirstOrDefault();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Employees empl)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            bool OK = false;

            if (empl.EmployeeId == 0)
            {
                Employees dbItem = new Employees()
                {

                    Firstname = empl.Firstname,
                    Lastname = empl.Lastname,
                };
                
                entities.Employees.Add(dbItem);
                entities.SaveChanges();

                OK = true;
                entities.Dispose();
                return Json(OK);

            }
            else
            {
                Employees dbItem = (from e in entities.Employees
                                   where e.EmployeeId == empl.EmployeeId
                                   select e).FirstOrDefault();
                if (dbItem != null)
                {
                    dbItem.Firstname = empl.Firstname;
                    dbItem.Lastname = empl.Lastname;
                    
                    entities.SaveChanges();

                };
                OK = true;
                entities.Dispose();
                return Json(OK, JsonRequestBehavior.AllowGet);


            }

        }

        public ActionResult Delete(int id)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            
            bool OK = false;

            Employees dbItem = (from e in entities.Employees
                               where e.EmployeeId == id
                               select e).FirstOrDefault();
            if (dbItem != null)
            {
                entities.Employees.Remove(dbItem);
                entities.SaveChanges();
                OK = true;

            }

            entities.Dispose();
            return Json(OK, JsonRequestBehavior.AllowGet);

        }

    }
}