using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public  class BookMeetingEntity
    {
        public long ID { get; set; }
        public int? RoomID { get; set; }
        public string AppMan { get; set; }
        public DateTime? BeginTime { get; set; }
        public int? AuditTag { get; set; }
        public int? PreResult { get; set; }
        public string Subject { get; set; }
        public DateTime? EndTime { get; set; }
        public string AuditIdea { get; set; }
        public string Remark { get; set; }
        public int? Status { get; set; }
        public DateTime? PreEndTime { get; set; }
        public string CancelRea { get; set; }
        public string RoomName { get; set; }
        public DateTime? MeetingDate { get; set; }

        public DateTime? CreateTime { get; set; }

    }
}
