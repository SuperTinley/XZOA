using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public  class MaterialPriceEntity
    {
        public int ID { get; set; }
        public string MATERIAL { get; set; }
        public decimal PRICE { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public DateTime? UPDATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }
    }
}
