using System;

namespace MobileApp.Models
{
    public class WorkModel
    {
        #region Workassignment fields
        public string Operation { get; set; }
        public string WorkTitle { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public int WorkId { get; set; }
        #endregion

        #region Employee fields
        public int EmployeeId { get; set; }
        public string EmpOperation { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public byte[] Picture { get; set; }
        #endregion

        #region Customer fields
        public string CustOperation { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        #endregion

        #region Contractor fields
        public string ContOperation { get; set; }
        public int ContractorId { get; set; }
        public string ContractorName { get; set; }
        public string ContractorContactPerson { get; set; }
        public string ContractorPhone { get; set; }
        public string ContractorEmail { get; set; }
        public string VatId { get; set; }
        public string HourlyRate { get; set; }
        #endregion

        #region Timesheet fields           
        public string[] WorkPickerData { get; set; }
        public string[] EmployeePickerData { get; set; }
        public string[] ContractorPickerData { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public double CountedHours { get; set; }
        public string Comments { get; set; }
        #endregion

        #region Login fields
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string PasswordString { get; set; }
        #endregion
    }
}
