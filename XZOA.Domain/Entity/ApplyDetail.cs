using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.Entity;

namespace XZOA.Domain.Entity
{
   public class ApplyDetail
    {
        public string prdName { get; set; }
        public string orderNo { get; set; }
        public string spc { get; set; }
        public decimal? num { get; set; }
        public string unit { get; set; }
        public decimal? viceNum { get; set; }
        public string viceUnit { get; set; }
        public decimal? price { get; set; }
        public decimal? total { get; set; }
        public string currency { get; set; }
        public string Remark { get; set; }
    }
}
