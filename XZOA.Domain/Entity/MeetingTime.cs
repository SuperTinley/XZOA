using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.Entity;

namespace XZOA.Domain.Entity
{
   public class MeetingTime
    {
        public long ID { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime PreEndTime { get; set; }
        public string Subject { get; set; }
        public string AppMan { get; set; }
        public int? AuditTag { get; set; }
    }
}
