using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class BookEntity:IEntity<BookEntity>
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string NickName { get; set; }
        public string Size { get; set; }
        public int? TypeID { get; set; }
        public string Subject { get; set; }
        public string CreateMan { get; set; }
        public DateTime? UploadTime { get; set; }
        public string Remark { get; set; }
       
    }
}
