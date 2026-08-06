//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace WinFormsApp1
//{
//    internal class PurchaseRequestModel
//    {
//    }
//}

using System;
using System.Collections.Generic;

namespace WinFormsApp1
{
    public class PurchaseRequestModel
    {
        public int PRID { get; set; }
        public string PRNumber { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }
        public DateTime RequestDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public string HoldReason { get; set; }
        public string Remarks { get; set; }
        public List<PurchaseRequestDetailModel> LineItems { get; set; }

        public PurchaseRequestModel()
        {
            LineItems = new List<PurchaseRequestDetailModel>();
        }
    }

    public class PurchaseRequestDetailModel
    {
        public int DetailID { get; set; }
        public string PRID { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get { return Quantity * UnitCost; } }
        public string UOM { get; set; }
        public string BOMProjectCode { get; set; }
    }
}
