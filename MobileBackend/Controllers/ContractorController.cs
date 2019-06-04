using System;
using System.Linq;
using System.Web.Http;
using MobileBackend.DataAccess;
using MobileApp.Models;

namespace WorkSheetBackend.Controllers
{
    public class ContractorController : ApiController
    {
        public string[] GetAll()
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            string[] contractors = null;

            try
            {
                contractors = (from c in entities.Contractors
                               where (c.Active == true)
                               select c.CompanyName).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return contractors;
        }
        public WorkModel GetContractorModel(string contractorName)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                string chosenContractor = contractorName;
                Contractors contractor = (from co in entities.Contractors
                                          where (co.Active == true) && 
                                          (co.CompanyName == chosenContractor)
                                          select co).FirstOrDefault();

                WorkModel chosenContractorModel = new WorkModel()
                {
                    ContractorId = contractor.ContractorId,
                    ContractorName = contractor.CompanyName,
                    ContractorContactPerson = contractor.ContactPerson,
                    ContractorPhone = contractor.Phone,
                    ContractorEmail = contractor.Email,
                    VatId = contractor.VatId,
                    HourlyRate = contractor.HourlyRate.ToString(),
                };
                return chosenContractorModel;
            }
            finally
            {
                entities.Dispose();
            }
        }
        [HttpPost]
        public bool PostContractor(WorkModel model)
        {
            panconDatabaseEntities entities = new panconDatabaseEntities();
            try
            {
                if (model.ContOperation == "Save")
                {
                    Contractors newEntry = new Contractors()
                    {
                        CompanyName = model.ContractorName,
                        ContactPerson = model.ContractorContactPerson,
                        Phone = model.ContractorPhone,
                        Email = model.ContractorEmail,
                        VatId = model.VatId,
                        HourlyRate = int.Parse(model.HourlyRate),
                        Active = true,
                        CreatedAt = DateTime.Now
                    };
                    entities.Contractors.Add(newEntry);
                }
                else if (model.ContOperation == "Modify")
                {                
                    Contractors existing = (from co in entities.Contractors
                                            where (co.ContractorId == model.ContractorId) && 
                                            (co.Active == true)
                                            select co).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.CompanyName = model.ContractorName;
                        existing.ContactPerson = model.ContractorContactPerson;
                        existing.Phone = model.ContractorPhone;
                        existing.Email = model.ContractorEmail;
                        existing.VatId = model.VatId;
                        existing.HourlyRate = int.Parse(model.HourlyRate);
                        existing.LastModified = DateTime.Now;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (model.ContOperation == "Delete")
                {
                    Contractors existing = (from e in entities.Contractors
                                            where (e.ContractorId == model.ContractorId)
                                            select e).FirstOrDefault();

                    if (existing != null)
                    {
                        entities.Contractors.Remove(existing);
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