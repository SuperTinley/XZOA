using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity
{
   public class Bill
    {
       public string billNo { get; set; }
        public string prdName {get;set;}
       public string cus_no {get;set;}
       public string spc {get;set;}
        public decimal? num {get;set;}
        public decimal? viceNum { get; set; }
        public string unit  {get;set;}
        public string viceUnit { get; set; }
        public string purNo { get; set; }
        public decimal? TotalMoney { get;set;}
       public decimal price { get;set;}
       public string appDep  {get;set;}
       public string appMan  {get;set;}
       public string remark { get; set; } 
        public string useGroup { get; set; }

        public string priNO { get; set; }
    }
}
