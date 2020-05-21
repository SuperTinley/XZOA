using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XZOA.Code;
using XZOA.Application.SystemManage;
using XZOA.Domain.Entity.SystemManage;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using XZOA.Code.Excel;

namespace XZOA.Web.Areas.PurchaseManage.Controllers
{
    [HandlerLogin]
    public class PriceController : ControllerBase
    {
        PriceApp priceApp = new PriceApp();
        ApplyBillApp applyBillApp = new ApplyBillApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetPriceList(Pagination pagination,string keyword,DateTime? BeginDate,DateTime? EndDate,string chkTag)
        {
            var query = priceApp.GetPriceList(pagination,keyword,BeginDate,EndDate,chkTag);
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
        public ActionResult GetSearchPrice(Pagination pagination, string keyword)
        {
            var query = priceApp.GetSearchPrice(pagination,keyword);
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
        public ActionResult GetSearchList(string q)
        {
            var query = priceApp.GetSearchList(q);
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var query = priceApp.GetFormJson(keyValue);
            return Content(query.ToJson());
        }

       [HttpPost]
        public ActionResult UploadFile()
        {
            try
            {
                if (Request.Files.Count < 1)
                {
                    return Error("操作失败");
                }

                string filename = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
                string virtualPath = String.Format("~/File/{0}", filename);
                string path = Server.MapPath(virtualPath);
                Request.Files[0].SaveAs(path);//上传文件

                if (priceApp.ImportExcel(path,System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
                {
                    return Success("操作成功");
                }

                return Error("操作失败");

            }
            catch (Exception ex)
            {
                return Error("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckPrice(string keyValue)
        {
            try
            {
                string[] Nos = keyValue.Split(',');
                foreach (var No in Nos)
                {
                    var price = priceApp.GetFormJson(No);
                    if(price.chkTag=="T")
                    {
                        price.chkTag = "Y";
                        price.chkIdea = OperatorProvider.Provider.GetCurrent().UserName + "," + DateTime.Now.ToString();
                        priceApp.SubmitForm(price, No);
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
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                foreach (var id in Ids)
                {
                    
                    var query = applyBillApp.GetPriceList(id);
                    if (query.Count == 0)
                    {
                        priceApp.DeleteForm(id);
                    }
                }
                return Success("删除成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult SubmitForm(PriceEntity priceEntity,string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                priceEntity.priNO = priceApp.ProducePriNO();
                priceEntity.unit = priceEntity.unit.Trim().ToUpper();
                priceEntity.principal = priceEntity.principal / 100;
                priceApp.SubmitForm(priceEntity, keyValue);
            }
            else {
                var price = priceApp.GetFormJson(keyValue);
                price.endDD = priceEntity.endDD;
                price.tel = priceEntity.tel;
                price.fax = priceEntity.fax;
                price.quoter = priceEntity.quoter;
                price.rem = priceEntity.rem;
                price.conMan = priceEntity.conMan;
                priceApp.SubmitForm(price, keyValue);
            }
            
            return Success("操作成功。");
        }

        public ActionResult ExportExcel(string keyValue, string txt_keyword, DateTime? BeginDate, DateTime? EndDate, string chkTag)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT priNO '报价单号',sup '供应商',prdName '名称',orderNo '牌号',spc '规格',
                     price '单价',CONVERT(varchar(100), startDD, 23)  '启用日期', CONVERT(varchar(100), endDD, 23) '截止日期', rem '备注',
                     unit '单位',CONVERT(VARCHAR,Convert(decimal(18,2),principal*100))+'%' '税率'
                     FROM Sys_Price WHERE priNO IS NOT NULL ");

                    StringBuilder sb = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(txt_keyword))
                    {
                        var ap = string.Format(" AND ( sup LIKE '%{0}%' OR prdName LIKE '%{0}%' OR spc LIKE '%{0}%' OR priNO LIKE '%{0}%' ) ", txt_keyword);
                        sb.Append(ap);
                    }
                    if (BeginDate!=null&& EndDate!=null)
                    {
                        EndDate = EndDate.Value.AddDays(1);
                        var ap = string.Format(" AND startDD >= '{0}' AND  startDD <= '{1}'  ", BeginDate, EndDate);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(chkTag))
                    {
                        var ap = string.Format(" AND chkTag = '{0}' ", chkTag);
                        sb.Append(ap);
                    }
                    sb.Append(" ORDER BY startDD DESC");
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
            else {
                fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            }
            

            //设置新建文件路径及名称
            string savePath = path + fileName;

            new NPOIExcel().Export(dt, fileName, savePath);

            return Content(savePath);
        }
    }
}