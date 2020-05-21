using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity
{
   public class ApplyBill
    {
        public string billNo { get; set; }
        public string billType { get; set; }
        public string chkDate { get; set; }
        public string purSup { get; set; }
        public string purNo { get; set; }
        public decimal principal { get; set; }
        public string purMan { get; set; }
        public string QC { get; set; }
        public string table { get; set; }
        public string PrintMan { get; set; }
        public string appExaMan { get; set; }
        public string appMan { get; set; }
        public int ware { get; set; }
        public string appDep { get; set; }
        public List<Bill> bills { get; set; }

        public string currency { get; set; }
        public decimal TotalMoney { get; set; }
    }
}
