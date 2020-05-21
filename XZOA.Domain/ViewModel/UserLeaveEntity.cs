using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Domain.ViewModel
{
   public  class UserLeaveEntity
    {
        public UserLeaveEntity(LeaveEntity leaveEntity)
        {
            this.F_Id = leaveEntity.F_Id;
            this.F_UserId= leaveEntity.F_UserId;
            this.F_CreateTime = leaveEntity.F_CreateTime;
            this.F_BeginTime = leaveEntity.F_BeginTime;
            this.F_EndTime = leaveEntity.F_EndTime;
            this.F_VacationTypeId = leaveEntity.F_VacationTypeId;
            this.F_LeaveStatus = leaveEntity.F_LeaveStatus;
            this.F_LeaveTypeId = leaveEntity.F_LeaveTypeId;
            this.F_FileId = leaveEntity.F_FileId;
            this.F_CheckLeaderId = leaveEntity.F_CheckLeaderId;
            this.F_ApprovalLeaderId = leaveEntity.F_ApprovalLeaderId;
            this.F_LeaveReason = leaveEntity.F_LeaveReason;
            this.F_TimeLength_Day = leaveEntity.F_TimeLength_Day;
            this.F_TimeLength_Hour = leaveEntity.F_TimeLength_Hour;
            this.F_TimeLength_Minute = leaveEntity.F_TimeLength_Minute;
        }
        public string F_Id { get; set; }
        public string F_UserId { get; set; }
        public DateTime F_CreateTime { get; set; }
        public DateTime F_BeginTime { get; set; }
        public DateTime F_EndTime { get; set; }
        public int F_VacationTypeId { get; set; }
        public int F_LeaveStatus { get; set; }
       // public string F_Remark { get; set; }
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
        
        public string ApprovalLeader { get; set; }
        
        public string CheckLeader { get; set; }

     
        public string UserName { get; set; }
        
        public string Account { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public string Department { get; set; }
       
        public string Position { get; set; }

    }
}
