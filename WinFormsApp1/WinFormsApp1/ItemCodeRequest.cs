using System;

namespace WinFormsApp1
{
    /// <summary>
    /// Data Model for Item Code Creation Request
    /// </summary>
    public class ItemCodeRequest
    {
        public int ID { get; set; }
        public string RequestID { get; set; }
        public DateTime RequestDate { get; set; }
        public string Requestor { get; set; }
        public string Department { get; set; }
        public string ItemCategory { get; set; }
        public string ItemDescription { get; set; }
        public string TechnicalSpec { get; set; }
        public string UnitOfMeasure { get; set; }
        public string DrawingReference { get; set; }
        public string CriticalityLevel { get; set; }
        public string HSNCode { get; set; }
        public string ItemCode { get; set; }
        public string AuthorizedCreator { get; set; }
        public string ApprovalAuthority { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalRemarks { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string StoreLocation { get; set; }
        public string BinNumber { get; set; }
        public bool IsEmergency { get; set; }
        public string EmergencyApprovedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsPushedToItemMaster { get; set; }
        public DateTime? PushDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}