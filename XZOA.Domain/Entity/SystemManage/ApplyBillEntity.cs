using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XZOA.Domain.Entity.SystemManage
{
   public class ApplyBillEntity:IEntity<ApplyBillEntity>
    {
        public int ID { get; set; }
        public DateTime? date { get; set; }
        public string prdName { get; set; }
        public string spc { get; set; }
        public string mat { get; set; }
        public decimal? appNum { get; set; }
        public string appUnit { get; set; }
        public decimal? yiJiaoNum { get; set; }
        public DateTime? hopeDD { get; set; }
        public string rem { get; set; }
        public string appDep { get; set; }
        public string appMan { get; set; }
        public string appExaTag { get; set; }
        public string appExaIdea { get; set; }
        //审核领导
        public string appExaMan { get; set; }
        public DateTime? appExaDate { get; set; }
        public string appAuthTag { get; set; }
        public string appAuthIdea { get; set; }
        public string appAuthMan { get; set; }
        public DateTime? appAuthDate { get; set; }
        public DateTime? purDD { get; set; }
        public string purNo { get; set; }
        public string purSup { get; set; }
        public decimal? purPrice { get; set; }
        public string purExaTag { get; set; }
        public string purExaIdea { get; set; }
        public string purExaMan { get; set; }
        public DateTime? purExaDate { get; set; }

        public string purAuthTag { get; set; }
        public string purAuthIdea { get; set; }
        public string purAuthMan { get; set; }
        public DateTime? purAuthDate { get; set; }
        public string purMan { get; set; }
        public string caseTag { get; set; }
        public DateTime? retDate { get; set; }
        public string priNO { get; set; }
        public string prtTag { get; set; }
        public decimal? inNum { get; set; }
        public decimal? outNum { get; set; }
        public decimal? takeNum { get; set; }
        public decimal? inAddNum { get; set; }
        public string orderNo { get; set; }
        public string annex { get; set; }
        public decimal? prePrice { get; set; }
        public int? purWay { get; set; }
        public string url { get; set; }
        public string webPO { get; set; }
        public decimal? webPrice { get; set; }
        public decimal? viceNum { get; set; }
        public string viceUnit { get; set; }

        public string prdNo { get; set; }
        public string backReason { get; set; }
        public DateTime? prtDate { get; set; }
        public string useGroup { get; set; }
        public string isPreMoney { get; set; }
        public int? WAREWAY { get; set; }
        public string TEMGUID { get; set; }
        public string purIsTem { get; set; }
        public string FirstExaMan { get; set; }
        public int TypeID { get; set; }
    }
}
