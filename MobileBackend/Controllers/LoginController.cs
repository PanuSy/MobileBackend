using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using MobileBackend.DataAccess;
using MobileApp.Models;
using System.Net.Http;
using System.Net;

namespace WorkSheetBackend.Controllers
{
    public class LoginController : ApiController
    {
        [HttpGet]
        [HttpPost]
        public bool GetLogin(string LogData)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();

            string[] logDataParts = LogData.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            string usrname = logDataParts[0];
            string pw = logDataParts[1];
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(pw);
            byte[] hash = sha512.ComputeHash(bytes);

            Employees emp = (from e in entities.Employees
                             where (e.Username == usrname)
                             select e).FirstOrDefault();

            StringBuilder dbPW = new StringBuilder();
            for (int i = 0; i < emp.Password.Length; i++)
            {
                dbPW.Append(emp.Password[i].ToString("X2"));
            }
            StringBuilder logDataPW = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                logDataPW.Append(hash[i].ToString("X2"));
            }
            string DBPass = dbPW.ToString();
            string logDataPass = logDataPW.ToString();
            if (emp.Username + DBPass == usrname + logDataPass)
            {
                return true;
            }
            return false;
        }
        [HttpGet]
        public WorkModel GetProfileInfo(string userName)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                Employees profile = (from p in entities.Employees
                                     where (p.Active == true) &&
                                     (p.Username == userName)
                                     select p).FirstOrDefault();

                string cont = (from c in entities.Contractors
                               where (c.ContractorId == profile.ContractorId)
                               select c.CompanyName).Single();

                WorkModel loggedProfile = new WorkModel()
                {
                    ContractorName = cont,
                    Firstname = profile.Firstname,
                    Lastname = profile.Lastname,
                    Phone = profile.Phone,
                    Email = profile.Email,
                    Picture = profile.EmployeePicture,
                    Username = profile.Username
                };
                return loggedProfile;
            }
            finally
            {
                entities.Dispose();
            }
        }
        [HttpPost]
        public bool PostChanges(WorkModel model)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                Employees MyProfile = (from ce in entities.Employees
                                       where (ce.Active == true) &&
                                       (ce.Firstname + " "
                                       + ce.Lastname == model.Firstname
                                       + " " + model.Lastname)
                                       select ce).FirstOrDefault();

                if (MyProfile == null)
                {
                    return false;
                }
                int employeeId = MyProfile.EmployeeId;
                Employees existing = (from e in entities.Employees
                                      where (e.EmployeeId == employeeId) &&
                                      (e.Active == true)
                                      select e).FirstOrDefault();

                if (existing != null && model.Picture != null && model.Operation == "Save")
                {
                    existing.Firstname = model.Firstname;
                    existing.Lastname = model.Lastname;
                    existing.Phone = model.Phone.ToString();
                    existing.Email = model.Email;
                    existing.LastModified = DateTime.Now;
                    existing.EmployeePicture = model.Picture;
                }
                else if (existing != null && model.Picture == null && model.Operation == "Save")
                {
                    existing.Firstname = model.Firstname;
                    existing.Lastname = model.Lastname;
                    existing.Phone = model.Phone.ToString();
                    existing.Email = model.Email;
                    existing.LastModified = DateTime.Now;
                }
                else if (existing != null && model.Operation == "SavePw")
                {
                    existing.Password = model.Password;
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
[RoutePrefix("/api/login")]
public class LoginController : ApiController
{
    public static Employees name;
    panconDatabaseEntities entities = new panconDatabaseEntities();
    [HttpPost]
    [ActionName("XAMARIN_REG")]
    public HttpResponseMessage Xamarin_reg(WorkModel input)
    {
        try
        {
            var same = entities.Employees.Any(u => u.Username == input.Username);
            if (same == true)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Username is in use, try a."
                    + same.ToString());
            }
            else
            {
                Employees employee = new Employees();
                employee.Firstname = input.Firstname;
                employee.Lastname = input.Lastname;
                employee.Phone = input.Phone;
                employee.Email = input.Email;
                employee.Username = input.Username;
                employee.Password = input.Password;
                entities.Employees.Add(employee);
                entities.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.Accepted, "Successfully Created");
            }
        }
        finally
        {
            entities.Dispose();
        }
    }
    [HttpGet]
    [ActionName("XAMARIN_Login")]
    public HttpResponseMessage Xamarin_login(string username, byte[] password)
    {
        try
        {
            var user = entities.Employees.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Please Enter valid UserName and Password");
            }
            else
            {
                name = user;
                user.Active = true;
                entities.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.Accepted, "Success");
            }
        }
        finally
        {
            entities.Dispose();
        }
    }
}

//using MobileBackend.DataAccess;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using MobileApp.Models;

//namespace MobileBackend.Controllers
//{
//    [RoutePrefix("/api/login")]
//    public class LoginController : ApiController
//    {
//        public static Employees name;
//        panconDatabaseEntities entities = new panconDatabaseEntities();
//        [HttpPost]
//        [ActionName("XAMARIN_REG")]
//        public HttpResponseMessage Xamarin_reg(NewUserModel input)
//        {
//            try
//            {
//                var same = entities.Employees.Any(u => u.Username == input.Username);
//                if (same == true)
//                {
//                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Username is in use, try a."
//                        + same.ToString());
//                }
//                else
//                {
//                    Employees employee = new Employees();
//                    employee.Firstname = input.Firstname;
//                    employee.Lastname = input.Lastname;
//                    employee.Phone = input.Phone;
//                    employee.Email = input.Email;
//                    employee.Username = input.Username;
//                    employee.Password = input.Password;
//                    entities.Employees.Add(employee);
//                    entities.SaveChanges();

//                    return Request.CreateResponse(HttpStatusCode.Accepted, "Successfully Created");
//                }
//            }
//            finally
//            {
//                entities.Dispose();
//            }
//        }
//        [HttpGet]
//        [ActionName("XAMARIN_Login")]
//        public HttpResponseMessage Xamarin_login(string username, string password)
//        {
//            try
//            {
//                var user = entities.Employees.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
//                if (user == null)
//                {
//                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Please Enter valid UserName and Password");
//                }
//                else
//                {
//                    name = user;
//                    user.Active = true;
//                    entities.SaveChanges();
//                    return Request.CreateResponse(HttpStatusCode.Accepted, "Success");
//                }
//            }
//            finally
//            {
//                entities.Dispose();
//            }
//        }
//    }
//}