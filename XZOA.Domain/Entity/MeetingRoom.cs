using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.Entity;

namespace XZOA.Domain.Entity
{
   public class MeetingRoom
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }

        public DateTime MeetingDate { get; set; }

        public List<MeetingTime> meetingTimes { get; set; }
    }
}
