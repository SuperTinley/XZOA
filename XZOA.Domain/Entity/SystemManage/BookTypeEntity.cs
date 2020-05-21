using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class BookTypeEntity : IEntity<BookTypeEntity>
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string SortCode { get; set; }
        public bool? EnabledMark { get; set; }
        public string Remark { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateMan { get; set; }
    }
}
