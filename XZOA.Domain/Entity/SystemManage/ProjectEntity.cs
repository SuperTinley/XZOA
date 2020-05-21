using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class ProjectEntity
    {           
        public string F_Id { get; set; }
        public string F_ProjectEngineer { get; set; }
        public string F_Customer { get; set; }
        public string F_Model { get; set; }
        public string F_Spc { get; set; }
        public string F_TypeID { get; set; }
        public decimal? F_SampleQty { get; set; }
        public string F_OrderNo { get; set; }
        public string F_OrderItem { get; set; }
        public DateTime? F_OrderItemDate { get; set; }
        public DateTime? F_PlanSendSampleDate { get; set; }
        public DateTime? F_SampleReturnDate { get; set; }
        public string F_Remark { get; set; }
        public int? F_Choose { get; set; }
        public string F_Buyer { get; set; }
        public string F_Supplier { get; set; }
        public string F_Clerk { get; set; }
        public string F_Workshop { get; set; }
        public DateTime? F_SampleFinishDate { get; set; }
        public decimal? F_FactFinishNum { get; set; }
        public DateTime? F_FactFinishDate { get; set; }
        public string F_BuyRemark { get; set; }
        public DateTime? F_FactSendSampleDate { get; set; }
        public bool? F_SampleOnTime { get; set; }
        public bool? F_IsAudit { get; set; }
        public decimal? F_AccuracyRate { get; set; }
        public DateTime? F_AccuracyDate { get; set; }
        public DateTime? F_PublishDate { get; set; }
        public string F_SignSample { get; set; }
        public bool F_CaseTag { get; set; }
        
    }
}
