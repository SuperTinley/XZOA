using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class LeaveEntity : IEntity<LeaveEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_UserId { get; set; }
        public DateTime F_CreateTime { get; set; }
        public DateTime F_BeginTime { get; set; }
        public DateTime F_EndTime { get; set; }
        public int F_VacationTypeId { get; set; }
        public int F_LeaveStatus { get; set; }
        public int F_LeaveTypeId { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_DeleteUserId { get; set; }

        public bool? F_DeleteMark { get; set; }

        public Guid? F_FileId { get; set; }

        public string F_CheckLeaderId { get; set; }

        public string F_ApprovalLeaderId { get; set; }

        public string F_LeaveReason { get; set; }

        public int F_TimeLength_Day { get; set; }

        public int F_TimeLength_Hour { get; set; }

        public int F_TimeLength_Minute { get; set; }

        public string F_FileName { get; set; }

        public string F_SuffixName { get; set; }

        public bool F_IsOffLine { get; set; }

        public bool? F_IsCheck { get; set; }

        public bool? F_IsApproval { get; set; }

        public string F_CheckOpinion { get; set; }

        public string F_ApprovalOpinion { get; set; }

        public bool? F_IsSellOff { get; set; }
        
        public string F_SellOffCheckOpinion { get; set; }
        
        public DateTime? F_ResumptionLeaveTime { get; set; }//销假时间

        public int? F_SellOffTimeLength_Day { get; set; }

        public int? F_SellOffTimeLength_Hour { get; set; }

        public int? F_SellOffTimeLength_Minute { get; set; }
       
        public string F_Account { get; set; }
        public string F_UserName { get; set; }
        public string F_Department { get; set; }
        public string F_Duty { get; set; }
        public string F_Sex { get; set; }
        public DateTime? F_ApprovalTime { get; set; }
        public string F_CheckUserName { get; set; }
        public string F_ApprovalUserName { get; set; }

        public DateTime? F_ResumptionBeginTime { get; set; }//销假开始时间

    }
}
