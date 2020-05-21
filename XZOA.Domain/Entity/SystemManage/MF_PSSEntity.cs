using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class MF_PSSEntity
    {
        public string PS_ID { get; set; }
        public string PS_NO { get; set; }
        public DateTime? PS_DD { get; set; }
        public DateTime? PAY_DD { get; set; }
        public DateTime? CHK_DD { get; set; }
        public string TRAD_MTH { get; set; }
        public string BAT_NO { get; set; }
        public string CUS_NO { get; set; }
        public string VOH_ID { get; set; }
        public string VOH_NO { get; set; }
        public string DEP { get; set; }
        public string INV_NO { get; set; }
        public string TAX_ID { get; set; }
        public string OS_ID { get; set; }
        public string OS_NO { get; set; }
        public string SEND_MTH { get; set; }
        public string SEND_WH { get; set; }
        public string ZHANG_ID { get; set; }
        public decimal? EXC_RTO { get; set; }
        public string REM { get; set; }
        public string PAY_MTH { get; set; }
        public short? PAY_DAYS { get; set; }
        public short? CHK_DAYS { get; set; }
        public short? INT_DAYS { get; set; }
        public string USR { get; set; }
        public string CHK_MAN { get; set; }
        public string PRT_SW { get; set; }
        public DateTime? CLS_DATE { get; set; }
        public string PO_ID { get; set; }
        public string LZ_CLS_ID { get; set; }
        public DateTime? SYS_DATE { get; set; }
        public string PRT_USR { get; set; }
        public DateTime? PRT_DATE { get; set; }
        public short? AMT_POI { get; set; }
    }
}
