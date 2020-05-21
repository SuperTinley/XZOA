using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class PRDTEntity
    {
        public string PRD_NO { get; set; }
        public string SNM { get; set; }
        public string IDX1 { get; set; }
        public string IDX2 { get; set; }
        public string UT { get; set; }
        public string UT1 { get; set; }
        public string DFU_UT { get; set; }
        public string NAME { get; set; }
        public string SPC { get; set; }
        public string CCC { get; set; }
        public string KND { get; set; }
        public decimal? SPC_TAX { get; set; }
        public string PK2_UT { get; set; }
        public float? PK2_QTY { get; set; }
        public string PK3_UT { get; set; }
        public float? PK3_QTY { get; set; }
        public string MRK { get; set; }
        public string SUP1 { get; set; }
        public string SUP2 { get; set; }
        public decimal? UPR { get; set; }
        public int? UBPR { get; set; }

        public string WH { get; set; }
    }
}
