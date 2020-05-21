using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class AppointManEntity :IEntity<AppointManEntity>
    {
        public int ID { get; set; }

        public string tag { get; set; }

        public string man { get; set; }
    }
}
