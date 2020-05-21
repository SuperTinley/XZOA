using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class BillEntity:IEntity<BillEntity>
    {
        public int id { get; set; }
        public int fromID { get; set; }
        public string billType { get; set; }
        public string billNo { get; set; }
        public decimal? num { get; set; }
        public string remark { get; set;}
        public string prtTag { get; set; }
        public decimal? outNum { get; set; }
        public decimal? inNum { get; set; }
        public string caseTag { get; set; }
        public string checkNo { get; set; }
        public string purSup { get; set; }
        public DateTime? chkDate { get; set; }
        public int? erpItm { get; set; }
        public decimal? viceNum { get; set; }
        public string FINBILLDATE { get; set; }



    }
}
