using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Code.Excel;
using XZOA.Domain.Entity;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Web.Areas.PurchaseManage.Controllers
{
    [HandlerLogin]
    public class PurchaseController : ControllerBase
    {
        ApplyBillApp applyBillApp = new ApplyBillApp();
        PriceApp priceApp = new PriceApp();
        UserApp userApp = new UserApp();
        OrganizeApp organizeApp = new OrganizeApp();
        DutyApp dutyApp = new DutyApp();
        MailHelper mHelper = new MailHelper();
        BillApp billApp = new BillApp();
        // GET: PurchaseManage/Purchase
        /// <summary>
        /// 转单
        /// </summary>
        /// <param name="applyBillEntity"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ApplyBillEntity applyBillEntity,int? keyValue,string code)
        {
            ApplyBillEntity apply = null;
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            apply = applyBillApp.GetForm(keyValue.Value);
            var price = priceApp.GetFormJson(applyBillEntity.priNO);
            if (!string.IsNullOrEmpty(code))//转厂
            {
                ApplyBillEntity apply1 = new ApplyBillEntity();
                var applyBill = applyBillApp.GetForm(keyValue.Value);
                apply1.purNo = applyBillApp.ProducePurNO(applyBillEntity.purSup, price.currency.Value);
                apply1.appExaTag = applyBill.appExaTag;
                apply1.appExaDate = applyBill.appExaDate;
                apply1.appExaIdea = applyBill.appExaIdea;
                apply1.appExaMan = applyBill.appExaMan;
                apply1.appAuthTag = applyBill.appAuthTag;
                apply1.appAuthDate = applyBill.appAuthDate;
                apply1.appAuthMan = applyBill.appAuthMan;
                apply1.appAuthIdea = applyBill.appAuthIdea;
                apply1.prdName = applyBill.prdName;
                apply1.spc = applyBill.spc;
                apply1.rem = applyBill.rem;
                apply1.prdNo = applyBill.prdNo;
                apply1.useGroup = applyBill.useGroup;
                apply1.viceNum = applyBill.viceNum;
                apply1.viceUnit = applyBill.viceUnit;
                apply1.appNum = (applyBill.appNum ?? 0) - (applyBill.yiJiaoNum ?? 0);
                apply1.appUnit = applyBill.appUnit;
                apply1.purIsTem = applyBill.purIsTem;
                apply1.WAREWAY = applyBill.WAREWAY;
                apply1.hopeDD = applyBill.hopeDD;
                apply1.mat = applyBill.mat;
                apply1.orderNo = applyBill.orderNo;
                apply1.appMan = applyBill.appMan;
                apply1.appDep = applyBill.appDep;
                apply1.date = applyBill.date;
                apply1.isPreMoney = "N";
                apply1.prtTag = "F";
                apply1.caseTag = "F";
                apply1.purWay = 0;
                apply1.purAuthTag = "D";
                apply1.purExaTag = "D";
                apply1.purDD = DateTime.Now;
                apply1.purExaMan = applyBillEntity.purExaMan;
                apply1.purAuthMan = applyBillEntity.purAuthMan;
                apply1.purSup = applyBillEntity.purSup;
                apply1.purPrice = applyBillEntity.purPrice;
                apply1.priNO = applyBillEntity.priNO;
                apply1.purMan = userName;
                apply1.priNO = applyBillEntity.priNO;
                apply1.purExaDate = null;
                apply1.purAuthDate = null;
                apply1.purAuthIdea = null;
                apply1.purExaIdea = null;
                applyBillApp.SubmitForm(apply1);
                applyBill.caseTag = "T";//当前项结案
                applyBill.rem = "转厂：" + apply1.purNo;
                applyBillApp.SubmitForm(applyBill, applyBill.ID);
                if (!string.IsNullOrEmpty(apply1.purExaMan))
                {
                    var purExaMan = userApp.GetFormByName(apply1.purExaMan);
                    mHelper.MailServer = "10.110.120.2";
                    if (!string.IsNullOrEmpty(purExaMan.F_Email))
                    {
                        mHelper.Send(purExaMan.F_Email, "采购单审核", "你好," + apply1.purMan + "有采购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                    }

                }
            }
            else {//转单
                apply.purNo = applyBillApp.ProducePurNO(applyBillEntity.purSup, price.currency.Value);
                apply.purDD = DateTime.Now;
                apply.purExaMan = applyBillEntity.purExaMan;
                apply.purAuthMan = applyBillEntity.purAuthMan;
                apply.purSup = applyBillEntity.purSup;
                apply.purPrice = applyBillEntity.purPrice;
                apply.priNO = applyBillEntity.priNO;
                apply.purMan = userName;
                apply.priNO = applyBillEntity.priNO;
                apply.purExaDate = null;
                apply.purAuthDate = null;
                apply.purAuthIdea = null;
                apply.purExaIdea = null;
                if (apply.purExaTag != "D")
                {
                    apply.purExaTag = "M";
                }
                if (apply.purAuthTag == "F")
                {
                    apply.purAuthTag = "M";
                }
                applyBillApp.SubmitForm(apply, keyValue);//转单
                if (!string.IsNullOrEmpty(apply.purExaMan))
                {
                    var purExaMan = userApp.GetFormByName(apply.purExaMan);
                    mHelper.MailServer = "10.110.120.2";
                    if (!string.IsNullOrEmpty(purExaMan.F_Email))
                    {
                        mHelper.Send(purExaMan.F_Email, "采购单审核", "你好," + apply.purMan + "有采购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                    }

                }
            }
           
           
            return Success("操作成功。");
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPurchaseCheckList(Pagination pagination)
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var data = applyBillApp.GetPurchaseCheckList(userName);
            var priNos = data.Select(t=>t.priNO).ToList();
            var prices = priceApp.GetPurchaseList(priNos);
            var query = from a in data
                        join p in prices
                        on a.priNO equals p.priNO
                        select new {
                            ID=a.ID,
                            date=a.date,
                            purExaTag=a.purExaTag,
                            prdName=a.prdName,
                            orderNo=a.orderNo,
                            spc=a.spc,
                            mat=a.mat,
                            hopeDD=a.hopeDD,
                            appNum=a.appNum,
                            appUnit=a.appUnit,
                            appDep=a.appDep,
                            appMan=a.appMan,
                            purMan=a.purMan,
                            purNo=a.purNo,
                            sup=p.sup,
                            price=p.price,
                            totalMoney=GetSum(a,p),
                            pName=p.prdName,
                            pOrderNo=p.orderNo,
                            pSpc=p.spc,
                            unit= p.unit,
                            viceNum=a.viceNum,
                            viceUnit=a.viceUnit
                        };
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            var data1 = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data1.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPurchaseApprovalList(Pagination pagination)
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var data = applyBillApp.GetPurchaseApprovalList(userName);
            var priNos = data.Select(t => t.priNO).ToList();
            var prices = priceApp.GetPurchaseList(priNos);
            var query = from a in data
                        join p in prices
                        on a.priNO equals p.priNO
                        select new
                        {
                            ID = a.ID,
                            date = a.date,
                            purExaTag = a.purExaTag,
                            purAuthTag = a.purAuthTag,
                            prdName = a.prdName,
                            orderNo = a.orderNo,
                            spc = a.spc,
                            mat = a.mat,
                            hopeDD = a.hopeDD,
                            appNum = a.appNum,
                            appUnit = a.appUnit,
                            appDep = a.appDep,
                            appMan = a.appMan,
                            purMan = a.purMan,
                            purNo = a.purNo,
                            sup = p.sup,
                            price = p.price,
                            totalMoney = GetSum(a, p),
                            pName = p.prdName,
                            pOrderNo = p.orderNo,
                            pSpc = p.spc,
                            unit = p.unit,
                            viceNum = a.viceNum,
                            viceUnit = a.viceUnit
                        };
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            var data1 = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data1.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPurchaseDetailsList(Pagination pagination,string keyword,DateTime? BeginDate,DateTime? EndDate,string isPreMoney,string purIsTem,string caseTag)
        {
            var applyBills = applyBillApp.GetPurchaseDetailsList();
            var prices = priceApp.GetList();
            var query = from a in applyBills
                        join p in prices
                        on a.priNO equals p.priNO
                        select new
                        {
                            ID=a.ID,
                            purDD=a.purDD,
                            purWay=a.purWay,
                            purNo=a.purNo,
                            purSup=a.purSup,
                            purName=p.prdName,
                            purOrderNo=p.orderNo,
                            purSpc=p.spc,
                            appNum=a.appNum,
                            appUnit=a.appUnit,
                            viceNum=a.viceNum,
                            viceUnit=a.viceUnit,
                            inNum=a.inNum,
                            takeNum=a.takeNum,
                            price=p.price,
                            purUnit=p.unit,
                            yiJiaoNum=a.yiJiaoNum,
                            principal=p.principal,
                            priNo=p.priNO,
                            purMan=a.purMan,
                            retDate=a.retDate,
                            prtDate=a.prtDate,
                            date=a.date,
                            prdName=a.prdName,
                            orderNo=a.orderNo,
                            spc=a.spc,
                            mat=a.mat,
                            hopeDD=a.hopeDD,
                            appDep=a.appDep,
                            appMan=a.appMan,
                            isPreMoney = a.isPreMoney,
                            money = GetSum(a,p),
                            chkTag = p.chkTag,
                            purIsTem =a.purIsTem,
                            caseTag=a.caseTag
                        };
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(q=>q.purName.Contains(keyword)|| q.purSup.Contains(keyword)|| q.purMan.Contains(keyword)|| q.priNo.Contains(keyword) || q.purNo.Contains(keyword));
            }
            if (BeginDate != null && EndDate != null)
            {
                var endDate = EndDate.Value.AddDays(1);
                query = query.Where(q => q.purDD >= BeginDate && q.purDD < endDate);
            }
            if(BeginDate == null && EndDate == null&& string.IsNullOrEmpty(keyword)&& string.IsNullOrEmpty(isPreMoney)&& string.IsNullOrEmpty(purIsTem)&& string.IsNullOrEmpty(caseTag))
            {
                query = query.Where(t=>t.purDD.Value.Year == DateTime.Now.Year && t.purDD.Value.Month == DateTime.Now.Month);
            }
            if (!string.IsNullOrEmpty(isPreMoney))
            {
                query = query.Where(q => q.isPreMoney==isPreMoney);
            }
            if (!string.IsNullOrEmpty(purIsTem))
            {
                query = query.Where(q => q.purIsTem==purIsTem);
            }
            if (!string.IsNullOrEmpty(caseTag))
            {
                query = query.Where(q => q.caseTag==caseTag);
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.purDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        private decimal GetSum(ApplyBillEntity apply,PriceEntity price)
        {
            if (apply.appUnit == price.unit)
            {
                return Math.Round((apply.appNum ?? 0) * (price.price ?? 0), 2);
            }
            else {
                return Math.Round((apply.viceNum ?? 0) * (price.price ?? 0), 2);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult PreMoneyPrice(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                int[] keys =  Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var item in keys)
                {
                    var apply = applyBillApp.GetForm(item);
                    if (apply.isPreMoney == "Y")
                    {
                        apply.isPreMoney = "N";
                    }
                    else { apply.isPreMoney = "Y"; }
                    applyBillApp.SubmitForm(apply, item);
                }
                
                return Success("操作成功！");
            }
            catch (Exception)
            {

                return Error("操作失败！");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult BackCheck(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                int[] keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var item in keys)
                {
                    var apply = applyBillApp.GetForm(item);
                    if((apply.purMan.Equals(OperatorProvider.Provider.GetCurrent().UserName)&&(apply.inNum==null||apply.inNum==0)&&apply.isPreMoney=="N"))
                    {
                        apply.purAuthTag = "M";
                        apply.purExaTag = "M";
                        apply.purAuthDate = null;
                        apply.purAuthIdea = null;
                        apply.purExaDate = null;
                        apply.purExaIdea = null;
                        apply.purAuthMan = null;
                        apply.purExaMan = null;
                        apply.purWay = 0;
                        apply.prtTag = "F";
                        apply.prtDate = null;
                        applyBillApp.SubmitForm(apply, item);
                    }
                }

                return Success("操作成功！");
            }
            catch (Exception)
            {

                return Error("操作失败！");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CasePurchase(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                int[] keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var item in keys)
                {
                    var apply = applyBillApp.GetForm(item);
                    if ((apply.yiJiaoNum??0) == (apply.takeNum??0)&&apply.isPreMoney!="Y")
                    {
                        apply.caseTag = "T";
                        applyBillApp.SubmitForm(apply, item);
                    }
                }

                return Success("操作成功！");
            }
            catch (Exception)
            {

                return Error("操作失败！");
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPurchasePrintList(Pagination pagination,string keyword,string prtTag)
        {
            var purchases = applyBillApp.GetPurchasePrintList();
            var prices = priceApp.GetList();
            var query = from h in purchases
                        join p in prices
                        on h.priNO equals p.priNO
                        select new {
                            ID=h.ID,
                            purNo=h.purNo,
                            priNo=h.purNo,
                            purMan=h.purMan,
                            prdName=p.prdName,
                            orderNo=p.orderNo,
                            appNum=h.appNum,
                            appMan=h.appMan,
                            appUnit=h.appUnit,
                            viceNum=h.viceNum,
                            viceUnit=h.viceUnit,
                            unit=p.unit,
                            spc=p.spc,
                            purDD=h.purDD,
                            hopeDD=h.hopeDD,
                            mat =h.mat,
                            appDep=h.appDep,
                            purSup=p.sup,
                            prtTag=h.prtTag
                        };
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t=>t.purSup.Contains(keyword)||t.purNo.Contains(keyword)||t.prdName.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(prtTag)&&!prtTag.Equals("ALL"))
            {
                query = query.Where(t => t.prtTag.Equals(prtTag));
            }
            if (string.IsNullOrEmpty(prtTag)&& string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.prtTag.Equals("F"));
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.purDD).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        public ActionResult ExportExcel(string keyValue,string txt_keyword, int? purWay, string purIsTem,string status)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT  CONVERT(varchar(100), [date], 23) '申购日期',prdName '名称',orderNo '牌号', spc '规格',
                                               mat '材料要求',CONVERT(varchar(100), hopeDD, 23) '期望交期',appNum '数量',appUnit '单位',rem '用途',appDep '申购部门',
                                               appMan '申购人',purMan '采购员',purNo '采购单号',purSup '供应商',purPrice '采购单价',
                                               (CASE prtTag
                                               WHEN 'F' THEN '否'
                                               WHEN 'T' THEN '是' END) '是否打印',
                                               '' AS '报价单位',viceNum '副数量', viceUnit '副单位'
                                                FROM Sys_ApplyBill WHERE appExaTag='T' AND appAuthTag='T' AND caseTag!='T' AND purAuthTag!='T' ");
                    StringBuilder sb = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(txt_keyword))
                    {
                        var ap = string.Format(" AND ( appMan LIKE '%{0}%' OR prdName LIKE '%{0}%' OR purMan LIKE '%{0}%' OR purSup LIKE '%{0}%') ", txt_keyword);
                        sb.Append(ap);
                    }
                    if (purWay != null)
                    {
                        var ap = string.Format(" AND purWay = {0}", purWay);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(purIsTem))
                    {
                        var ap = string.Format(" AND purIsTem = '{0}' ", purIsTem);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        var ap = "";
                        if (status == "F")
                            ap = string.Format(" AND purNo IS NULL ");
                        else if (status == "T")
                            ap = string.Format(" AND purNo IS NOT NULL ");
                        else if (status == "N")
                            ap = string.Format(" AND (purExaTag = 'F' OR purAuthTag == 'F')");
                       
                        sb.Append(ap);
                    }
                    sb.Append(" ORDER BY date DESC");
                    command.CommandText = sb.ToString();
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            string path = HttpContext.Server.MapPath("/Excel/");

            string fileName = "";

            if (!string.IsNullOrEmpty(keyValue))
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + keyValue;
            }
            else
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }


            //设置新建文件路径及名称
            string savePath = path + fileName;

            new NPOIExcel().Export(dt, fileName, savePath);

            return Content(savePath);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        public ActionResult ExportPurchaseDetailExcel(string keyValue, string keyword, DateTime? BeginDate, DateTime? EndDate, string isPreMoney, string purIsTem, string caseTag)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT 
                                               ROW_NUMBER() OVER (ORDER BY a.purDD ASC) AS '序号',
                                               CONVERT(varchar(100), a.purDD, 23) '采购日期',
                                               (CASE a.purWay
                                               WHEN 3 THEN '国际采购'
                                               ELSE '国内采购'  
                                               END) '采购方式', a.purNo '采购单号',
                                               a.purSup '供应商',p.prdName '采购名称',
                                               p.orderNo '采购牌号',p.spc '采购规格',
                                               Convert(decimal(18,2),a.appNum) '申购数量',a.appUnit '申购单位',
                                               Convert(decimal(18,2),a.viceNum) '副数量', a.viceUnit '副单位',
                                               Convert(decimal(18,2),a.yiJiaoNum) '已交数量',
                                               Convert(decimal(18,2),b.viceNum) '已交副数量',
                                               CONVERT(VARCHAR,Convert(decimal(18,2),p.principal*100))+'%' '税率',
                                               p.priNO '报价单号',purMan '采购员',
                                               CONVERT(varchar(100), a.retDate, 23) '回货日期',
                                               CONVERT(varchar(100), a.prtDate, 23) '打印日期',
                                               CONVERT(varchar(100), a.[date], 23) '申购日期',a.prdName '申购名称',a.orderNo '申购牌号', 
                                               Convert(decimal(18,2),p.price) '单价',p.unit '报价单位',
                                               a.spc '申购规格',a.mat '材料要求',CONVERT(varchar(100), a.hopeDD, 23)'期望交期',a.appDep '申购部门',
                                               a.appMan '申购人',
                                               (CASE a.caseTag
                                               WHEN 'T' THEN '已结案'
                                               ELSE '未结案'
                                               END) '结案',
                                               (CASE a.isPreMoney
                                               WHEN 'Y' THEN '已预付'
                                               ELSE '未预付'
                                               END) '预付',
                                               (CASE
                                               WHEN a.appUnit=p.unit THEN Convert(decimal(18,2),a.appNum*p.price)
                                               ELSE Convert(decimal(18,2),a.viceNum*p.price)
                                               END) '金额',
                                               (CASE p.chkTag
                                               WHEN 'Y' THEN '已审核'
                                               ELSE '未审核' END) '报价审核'
                                               FROM Sys_ApplyBill a join Sys_Price p
                                               on a.priNO = p.priNO 
                                               LEFT JOIN (SELECT b.fromID,SUM(b.viceNum) 'viceNum' FROM Sys_Bill b JOIN Sys_ApplyBill s on b.fromID = s.ID WHERE b.billType='C' GROUP BY b.fromID) As b
                                               on a.ID = b.fromID WHERE a.purAuthTag='T' ");

                    StringBuilder sb = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var ap = string.Format(" AND ( p.prdName LIKE '%{0}%' OR a.purSup LIKE '%{0}%' OR a.purMan LIKE '%{0}%' OR a.priNO LIKE '%{0}%' OR a.purNo LIKE '%{0}%') ", keyword);
                        sb.Append(ap);
                    }
                    if(BeginDate!=null&&EndDate!=null)
                    {
                        var ap = string.Format(" AND a.purDD >= '{0}' AND a.purDD < '{1}'", BeginDate.Value.Date,EndDate.Value.Date.AddDays(1));
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(isPreMoney))
                    {
                        var ap = string.Format(" AND isPreMoney = '{0}'", isPreMoney);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(purIsTem))
                    {
                        var ap = string.Format(" AND purIsTem = '{0}' ", purIsTem);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(caseTag))
                    {
                        var ap = string.Format(" AND caseTag = '{0}' ", caseTag);
                        sb.Append(ap);
                    }
                    if (string.IsNullOrEmpty(keyword)&& BeginDate == null&& string.IsNullOrEmpty(caseTag) && string.IsNullOrEmpty(isPreMoney) && string.IsNullOrEmpty(purIsTem))
                    {
                        var ap = string.Format(" AND DATENAME(YY,a.purDD)={0} AND DATENAME(mm,a.purDD)={1}", DateTime.Now.Year,DateTime.Now.Month);
                        sb.Append(ap);
                    }
                    command.CommandText = sb.ToString();
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            string path = HttpContext.Server.MapPath("/Excel/");

            string fileName = "";

            if (!string.IsNullOrEmpty(keyValue))
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + keyValue;
            }
            else
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }


            //设置新建文件路径及名称
            string savePath = path + fileName;

            new NPOIExcel().Export(dt, fileName, savePath);

            return Content(savePath);
        }


        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        public ActionResult ExportBillExcel(string keyValue, string keyword, DateTime? dateBeginDate, DateTime? dateEndDate)

        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"(SELECT (case a.purWay
                                                when 1 then '网购'  
                                                when 2 then '采购'
                                                when 3 then '国际采购'  END) AS '采购方式',
                                              CONVERT(varchar(100), b.chkDate, 23) '检验日期',
                                              p.priNO '报价单号',a.prdName '申购名称',a.orderNo '申购牌号',
                                              a.spc '申购规格',a.purSup '供应商',p.prdName '采购名称',
                                              p.orderNo '采购牌号',p.spc '采购规格', b.num '数量',
                                              a.appUnit '单位',p.price '采购单价',p.rem '报价备注',
                                              b.remark '进货备注',a.purMan '采购员',b.billNo '进货单号',
                                              a.appDep '申购部门',a.rem '申购备注',(case b.prtTag
                                              WHEN 'D' THEN '已交单'
                                              ELSE '未交单' 
                                              END) AS '状态',CONVERT(varchar(100), a.prtDate, 23) '打印日期',
                                              b.viceNum '副数量',a.viceUnit '副单位',p.unit '报价单位',
                                              a.useGroup '使用组别',(case p.chkTag
                                              WHEN 'Y' THEN '已审核'
                                              ELSE '未审核' 
                                              END) AS '报价审核',
                                              (CASE a.isPreMoney
                                               WHEN 'Y' THEN '已预付'
                                               ELSE '未预付'
                                               END) '预付',
                                              b.FINBILLDATE '收单日期'
                                               FROM Sys_Bill b LEFT JOIN Sys_ApplyBill a
                                               on b.fromID = a.ID JOIN Sys_Price p
                                              on a.priNO = p.priNO WHERE b.billType = 'C') ");

                    string sql1= string.Format(@"(SELECT (case a.purWay
                                                when 1 then '网购'  
                                                when 2 then '采购'
                                                when 3 then '国际采购'  END) AS '采购方式',
                                              CONVERT(varchar(100), b.chkDate, 23) '检验日期',
                                              p.priNO '报价单号',a.prdName '申购名称',a.orderNo '申购牌号',
                                              a.spc '申购规格',a.purSup '供应商',p.prdName '采购名称',
                                              p.orderNo '采购牌号',p.spc '采购规格', b.num '数量',
                                              a.appUnit '单位',p.price '采购单价',p.rem '报价备注',
                                              b.remark '进货备注',a.purMan '采购员',b.billNo '进货单号',
                                              a.appDep '申购部门',a.rem '申购备注',(case b.prtTag
                                              WHEN 'D' THEN '已交单'
                                              ELSE '未交单' 
                                              END) AS '状态',CONVERT(varchar(100), a.prtDate, 23) '打印日期',
                                              b.viceNum '副数量',a.viceUnit '副单位',p.unit '报价单位',
                                              a.useGroup '使用组别',(case p.chkTag
                                              WHEN 'Y' THEN '已审核'
                                              ELSE '未审核' 
                                              END) AS '报价审核',
                                              (CASE a.isPreMoney
                                               WHEN 'Y' THEN '已预付'
                                               ELSE '未预付'
                                               END) '预付',
                                              b.FINBILLDATE '收单日期'
                                              FROM Sys_Bill b LEFT JOIN Sys_Bill bi
                                              on b.fromID = bi.ID
                                              JOIN Sys_ApplyBill a 
                                              ON bi.fromID = a.ID
                                              JOIN Sys_Price p
                                              on a.priNO = p.priNO WHERE b.billType = 'E') ");
                    string sql2 = "SELECT * FROM("+ sql + "UNION ALL" + sql1+") AS a WHERE 1=1 ";
                    StringBuilder sb = new StringBuilder(sql2);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var ap = string.Format(" AND ( a.采购员 LIKE '%{0}%' OR a.进货单号 LIKE '%{0}%' OR a.供应商 LIKE '%{0}%') ", keyword);
                        sb.Append(ap);
                    }
                    if (dateBeginDate != null)
                    {
                        var ap = string.Format(" And a.检验日期 >='{0}'", dateBeginDate.ToDateString());
                        sb.Append(ap);
                    }
                    if (dateEndDate!=null)
                    {
                        var ap = string.Format(" And a.检验日期 <='{0}'", dateEndDate.ToDateString());
                        sb.Append(ap);
                    }
                    sb.Append(" ORDER BY a.检验日期 DESC ");
                    command.CommandText = sb.ToString();
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            string path = HttpContext.Server.MapPath("/Excel/");

            string fileName = "";

            if (!string.IsNullOrEmpty(keyValue))
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + keyValue;
            }
            else
            {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }


            //设置新建文件路径及名称
            string savePath = path + fileName;

            new NPOIExcel().Export(dt, fileName, savePath);

            return Content(savePath);
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCheckPurchaseForm(ApplyBillEntity applyBillEntity, string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                List<string> authList = new List<string>();
                List<string> purList = new List<string>();
                foreach (var Id in Ids)
                {
                    int ID = Convert.ToInt32(Id);
                    var apply = applyBillApp.GetForm(ID);
                    apply.purExaDate = DateTime.Now;
                    apply.purExaIdea = applyBillEntity.purExaIdea;
                    PriceEntity price = priceApp.GetFormJson(apply.priNO);
                    decimal sumMoney = 0;
                    if (price.unit == apply.appUnit)
                    {
                        sumMoney = (price.price ?? 0) * (apply.appNum ?? 0);
                    }
                    else {
                        sumMoney = (price.price ?? 0) * (apply.viceNum ?? 0);
                    }
                    if (applyBillEntity.purExaTag == "T")//审核通过
                    {
                        apply.purExaTag = applyBillEntity.purExaTag;
                        if(sumMoney <= 3000)
                        {
                            apply.purAuthMan = apply.purExaMan;
                            apply.purAuthTag = "T";
                            apply.purAuthDate = DateTime.Now;
                            applyBillApp.SubmitForm(apply, ID);
                        }
                        else
                        {
                            if(!authList.Contains(apply.purAuthMan))
                            {
                                authList.Add(apply.purAuthMan);
                            }
                            applyBillApp.SubmitForm(apply, ID);
                        }

                    }
                    else
                    {
                        apply.purExaTag = applyBillEntity.purExaTag;
                        if (!purList.Contains(apply.purMan))
                        {
                            purList.Add(apply.purMan);
                        }
                        applyBillApp.SubmitForm(apply, ID);
                    }
                }
                if (applyBillEntity.purExaTag == "T")
                {
                    foreach (var item in authList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "采购单审批", "你好,有采购单需要你登录OA去做审批,请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }
                }
                else {
                    foreach (var item in purList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "采购单审核不通过", "你好,你的采购单审核不通过,详情查看请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }
                }
                
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                return Error("操作失败。");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitApprolvalPurchaseForm(ApplyBillEntity applyBillEntity, string keyValue)
        {
            try
            {
                List<string> purList = new List<string>();
                string[] Ids = keyValue.Split(',');
                foreach (var Id in Ids)
                {
                    int ID = Convert.ToInt32(Id);
                    var apply = applyBillApp.GetForm(ID);
                    apply.purAuthDate = DateTime.Now;
                    apply.purAuthIdea = applyBillEntity.purAuthIdea;
                    apply.purAuthTag = applyBillEntity.purAuthTag;
                    applyBillApp.SubmitForm(apply, ID);
                    if (apply.purAuthTag == "F")
                    {
                        if (!purList.Contains(apply.purMan))
                        {
                            purList.Add(apply.purMan);
                        }
                    }
                  
                }
                if(applyBillEntity.purAuthTag=="F")
                {
                    foreach (var item in purList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "采购单审核不通过", "你好,你的采购单审核不通过,详情查看请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }
                }
                return Success("操作成功。");
            }
            catch (Exception)
            {

                return Error("操作失败。");
            }
        }


        [HttpGet]
        public ActionResult CheckPurchaseIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ApprovalPurchaseIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PurchasePrintIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CheckPurchaseForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ApprovalPurchaseForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PurchaseDeatailsIndex()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Print(string keyValue)
        {
            string[] IDs = keyValue.Split(',');
            int[] IDArray = Array.ConvertAll<string, int>(IDs, s => int.Parse(s));
            List<Apply> applyList = new List<Apply>();
            foreach (var ID in IDArray)
            {
                var applyBill = applyBillApp.GetForm(ID);
                applyBill.prtTag = "T";
                applyBill.prtDate = DateTime.Now;
                applyBillApp.SubmitForm(applyBill,applyBill.ID);
                var price = priceApp.GetFormJson(applyBill.priNO);
                var apply = applyList.Where(t => t.purNo == applyBill.purNo).FirstOrDefault();
                if (apply != null)
                {
                    var list = apply.applys;
                    ApplyDetail app = new ApplyDetail();
                    app.prdName = price.prdName;
                    app.orderNo = price.orderNo;
                    app.spc = price.spc;
                    app.price = price.price;
                    app.Remark = price.rem;
                    if (price.unit == applyBill.appUnit)
                    {
                        app.num = applyBill.appNum;
                        app.viceNum = (applyBill.viceNum??0);
                        app.viceUnit = applyBill.viceUnit;
                    }
                    else
                    {
                        app.num = (applyBill.viceNum ?? 0);
                        app.viceNum = applyBill.appNum;
                        app.viceUnit = applyBill.appUnit;
                    }
                    app.unit = price.unit;
                    app.total = Math.Round(app.price.Value * app.num.Value, 2);
                    if (price.currency.HasValue)
                    {
                        var ItemId = new ItemsApp().GetItemFormByName("币别").F_Id;
                        app.currency = new ItemsDetailApp().GetItemByItemIdAndCode(ItemId, price.currency.Value.ToString()).F_Description;
                    }
                    else
                    {
                        app.currency = "￥";
                    }
                    list.Add(app);
                    apply.TotalMoney = Math.Round(list.Sum(t => t.total).Value, 2);
                    apply.currency = app.currency;
                }
                else {
                    Apply app = new Apply();
                    app.telPhone = price.tel;
                    app.Fax = price.fax;
                    app.date = applyBill.date.ToDateString();
                    app.purAuthMan = applyBill.purAuthMan;
                    app.hopeDD = applyBill.hopeDD.ToDateString();
                    app.purExaMan = applyBill.purExaMan;
                    app.purMan = applyBill.purMan;
                    app.purNo = applyBill.purNo;
                    app.purSup = applyBill.purSup;
                    app.remark = price.rem;
                    var list = new List<ApplyDetail>();
                    ApplyDetail appDetail = new ApplyDetail();
                    appDetail.prdName = price.prdName;
                    appDetail.orderNo = price.orderNo;
                    appDetail.spc = price.spc;
                    appDetail.price = price.price;
                    if (price.unit == applyBill.appUnit)
                    {
                        appDetail.num = applyBill.appNum;
                        appDetail.viceNum = (applyBill.viceNum ?? 0);
                        appDetail.viceUnit = applyBill.viceUnit;
                    }
                    else
                    {
                        appDetail.num = (applyBill.viceNum ?? 0);
                        appDetail.viceNum = applyBill.appNum;
                        appDetail.viceUnit = applyBill.appUnit;
                    }
                    appDetail.unit = price.unit;
                    appDetail.total = Math.Round(appDetail.price.Value * appDetail.num.Value, 2);
                    if (price.currency.HasValue)
                    {
                        var ItemId = new ItemsApp().GetItemFormByName("币别").F_Id;
                        appDetail.currency = new ItemsDetailApp().GetItemByItemIdAndCode(ItemId, price.currency.Value.ToString()).F_Description;
                    }
                    else {
                        appDetail.currency = "￥";
                    }
                    list.Add(appDetail);
                    app.applys = list;
                    app.TotalMoney = Math.Round(list.Sum(t=>t.total).Value,2);
                    app.currency = appDetail.currency;
                    applyList.Add(app);
                }
            }
           
            return View(applyList);
        }
    }
}