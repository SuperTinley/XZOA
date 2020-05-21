using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public  class TemplateEntity
    {
        public string TEM_ID { get; set; }
        public string SOURCE_ID { get; set; }
        public string TEM_NAME { get; set; }
        public string CUSTOMER { get; set; }
        public string TEM_TYPE { get; set; }
        public int? TEM_NUM { get; set; }
        public string MAT_REQ { get; set; }
        public DateTime HOPE_DD { get; set; }
        public string DRAWING { get; set; }
        public string DRAWING_ALIAS { get; set; }
        public string TEM_IMG { get; set; }
        public string SIZE { get; set; }
        public string TEM_DEP { get; set; }
        public string APP_DEP { get; set; }
        public string TEM_CHARGE { get; set; }
        public string TEM_REMARK { get; set; }
        public string AUDIT_TAG { get; set; }
        public string AUDIT_MAN { get; set; }
        public DateTime? AUDIT_DATE { get; set; }
        public string RATIFY_TAG { get; set; }
        public string RATIFY_MAN { get; set; }
        public DateTime? RATIFY_DATE { get; set; }
        public string BACK_TAG { get; set; }
        public string BACK_REA { get; set; }
        public string CLOSE_ID { get; set; }
        public int PROCESS_STEP_NUM { get; set; }
        public string PROCESS { get; set; }
        public DateTime? FINISH_DATE { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public string CREATE_USER_ID { get; set; }
        public string CREATE_USER { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string TEM_ACCEPT { get; set; }
        public string NOTICE_GET { get; set; }
        public string NOTICE_MAN { get; set; }
        public DateTime? NOTICE_DATE { get; set; }
        public decimal? MAT_FEE { get; set; }
        public decimal? MDDE_FEE { get; set; }
        public int? FINISH_NUM { get; set; }
        public string TAKE_MAN { get; set; }
        public DateTime? TAKE_DATE { get; set; }
        public string PRT_TAG { get; set; }
        public string PRT_IMG { get; set; }
        public string TEM_NO { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? WORK_TIME { get; set; }
        public string MATERIAL { get; set; }
        public string GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
    }
}
