using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public  class MeetRoomEntity
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool? HasProjector { get; set; }
        public bool? HasNotebook { get; set; }
        public bool? Teleconferencing { get; set; }
        public bool? Videoconferencing { get; set; }
        public int? Number { get; set; }
        public string Location { get; set; }
        public string Remark { get; set; }
        public DateTime? CreateTime { get; set; }

    }
}
