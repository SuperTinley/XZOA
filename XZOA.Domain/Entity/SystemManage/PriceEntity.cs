using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class PriceEntity:IEntity<PriceEntity>
    {
        public string priNO { get; set; }
        public string sup { get; set; }
        public string prdName { get; set; }
        public string spc { get; set; }

        public decimal? price { get; set; }
        public DateTime? startDD { get; set; }
        public DateTime? endDD { get; set; }
        public string rem { get; set; }

        public string chkTag { get; set; }
        public string chkIdea { get; set; }
        public string orderNo { get; set; }

        public string conMan { get; set; }
        
        public string tel { get; set; }
        public string fax { get; set; }
        public string unit { get; set; }
        public decimal? principal { get; set; }

        public int? currency { get; set; }

        public string quoter { get; set; }
    }
}
