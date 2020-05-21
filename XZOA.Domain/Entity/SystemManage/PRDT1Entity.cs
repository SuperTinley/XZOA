using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class PRDT1Entity
    {
        public string WH { get; set; }
        public string PRD_NO { get; set; }
        public string PRD_MARK { get; set; }
        public decimal? QTY { get; set; }
        public decimal? QTY1 { get; set; }
        public DateTime? LST_IND { get; set; }
        public DateTime? LST_OTD { get; set; }
        public decimal? AMT_CST { get; set; }
    }
}
