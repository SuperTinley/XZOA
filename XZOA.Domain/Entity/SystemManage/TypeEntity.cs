using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class TypeEntity:IEntity<TypeEntity>
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public string Remark { get; set; }
        
    }
}
