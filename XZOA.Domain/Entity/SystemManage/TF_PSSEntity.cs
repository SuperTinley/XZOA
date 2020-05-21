using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class TF_PSSEntity
    {
        public string PS_ID { get; set; }
        public string PS_NO { get; set; }
        public int ITM { get; set; }
        public DateTime? PS_DD { get; set; }
        public string WH { get; set; }
        public string PRD_NO { get; set; }
        public string PRD_NAME { get; set; }
        public string PRD_MARK { get; set; }
        public string UNIT { get; set; }
        public decimal? QTY { get; set; }
        public decimal? UP { get; set; }
        public decimal? AMTN_NET { get; set; }
        public decimal? AMT { get; set; }
        public decimal? CSTN_SAL { get; set; }
        public decimal? TAX { get; set; }
        public decimal? TAX_RTO { get; set; }
        public int? PRE_ITM { get; set; }
        public decimal? QTY1 { get; set; }
        public string PAK_WEIGHT_UNIT { get; set; }
        public string REM { get; set; }
        public int? OTH_ITM { get; set; }
        public string CUS_OS_NO { get; set; }
        public string OS_ID { get; set; }
        public decimal? UP_QTY1 { get; set; }
        public int? SL_ITM { get; set; }
        public int? BL_OS_ITM { get; set; }
        public string FREE_ID_DEF { get; set; }
        public string OS_NO { get; set; }
        public decimal? QTY_RTN { get; set; }
    }
}
