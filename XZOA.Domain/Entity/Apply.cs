using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.Entity;

namespace XZOA.Domain.Entity
{
   public class Apply
    {
        public string purNo { get; set; }
        public string purSup { get; set; }
        public string contacts { get; set; }
        public string date { get; set; }
        public string hopeDD { get; set; }
        public string telPhone { get; set; }
        public string Fax { get; set; }
        public string purExaMan { get; set; }
        public string purAuthMan { get; set; }
        public string purMan { get; set; }

        public decimal? TotalMoney { get; set; }

        public string remark { get; set; }

        public string currency { get; set; }
        
        public List<ApplyDetail> applys { get; set; }
        
    }
}
