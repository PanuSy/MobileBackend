using System;
using System.Linq;
using System.Web.Http;
using MobileBackend.DataAccess;
using MobileApp.Models;

namespace MobileBackend.Controllers
{
    public class CustomerController : ApiController
    {
        public string[] GetAll()
        {
            string[] customers = null;
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                customers = (from c in entities.Customers
                             where (c.Active == true)
                             select c.CustomerName).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return customers;
        }
        public WorkModel GetCustomerModel(string customerName)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                string chosenCustomer = customerName;
                Customers customer = (from c in entities.Customers
                                     where (c.Active == true) && 
                                     (c.CustomerName == chosenCustomer)
                                     select c).FirstOrDefault();

                WorkModel chosenCustomerModel = new WorkModel()
                {
                    CustomerName = customer.CustomerName,
                    ContactPerson = customer.ContactPerson,
                    CustomerPhone = customer.Phone,
                    CustomerEmail = customer.Email,
                    CustomerId = customer.CustomerId
                };
                return chosenCustomerModel;
            }
            finally
            {
                entities.Dispose();
            }
        }
        [HttpPost]
        public bool PostCustomer(WorkModel model)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                if (model.CustOperation == "Save")
                {
                    Customers newEntry = new Customers()
                    {
                        CustomerName = model.CustomerName,
                        ContactPerson = model.ContactPerson,
                        Phone = model.CustomerPhone,
                        Email = model.CustomerEmail,
                        CreatedAt = DateTime.Now,
                        Active = true
                    };
                    entities.Customers.Add(newEntry);
                }
                else if (model.CustOperation == "Modify")
                {
                    Customers existing = (from c in entities.Customers
                                         where (c.CustomerId == model.CustomerId) && 
                                         (c.Active == true)
                                         select c).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.CustomerName = model.CustomerName;
                        existing.ContactPerson = model.ContactPerson;
                        existing.Phone = model.CustomerPhone;
                        existing.Email = model.CustomerEmail;
                        existing.LastModified = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (model.CustOperation == "Delete")
                {
                    Customers chosenCustomer = (from cc in entities.Customers
                                               where (cc.CustomerName == model.CustomerName)
                                               select cc).FirstOrDefault();

                    if (chosenCustomer == null)
                    {
                        return false;
                    }
                    int customerId = chosenCustomer.CustomerId;
                    Customers existing = (from e in entities.Customers
                                         where (e.CustomerId == customerId)
                                         select e).FirstOrDefault();

                    if (existing != null)
                    {
                        entities.Customers.Remove(existing);
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