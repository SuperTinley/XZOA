using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Web.Areas.ProjectManage.Controllers
{
    public class ProjectController : ControllerBase
    {
        // GET: ProjectManage/Project

        ProjectApp projectApp = new ProjectApp();
        UserApp userApp = new UserApp();
        RoleApp roleApp = new RoleApp();

        public ActionResult ProjectForm()
        {
            return View();
        }

        public ActionResult CheckResult()
        {
            return View();
        }

        public ActionResult GetList(Pagination pagination)
        {
            var RoleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var buyer = new RoleApp().GetKeyGridJson("采购员");
            var clerk = new RoleApp().GetRoleUser("文员");
            int type = 0;
            var list = projectApp.GetList();
            if (buyer.Contains(RoleId)) type = 2;
            else if (clerk.Contains(RoleId)) type = 1;
            if(type!=0)
                list=list.Where(t => t.F_Choose == type).ToList();

            pagination.records = list.Count();
            list = list.OrderByDescending(q => q.F_Id).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            var data = new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        public ActionResult ExportExcel()
        {
            var RoleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var buyer = new RoleApp().GetKeyGridJson("采购员");
            var clerk = new RoleApp().GetRoleUser("文员");
            int type = 0;
            string sql = "";
            
            StringBuilder sb = new StringBuilder();
            
            if (buyer.Contains(RoleId)) {
                type = 2;
                sql = string.Format(@" SELECT F_ProjectEngineer '项目工程师', F_Customer '客户', F_Model '型号', F_Spc '名称规格', t.TypeName '类型',
                     F_SampleQty '样板数量', F_OrderNo '订单号', F_OrderItem '立项单',(CASE F_Choose when 1 then '自制' when 2 then '外协' ELSE '' end) '自制/外协', F_Buyer '采购员', F_Supplier '供应商',
                    CONVERT(varchar(100), F_SampleFinishDate, 23) '样板完成时间', F_FactFinishNum '实际完成数量',CONVERT(varchar(100), 
                    F_FactFinishDate, 23) '实际完成时间',F_BuyRemark '采购备注' FROM Sys_Project p
                    LEFT JOIN Sys_Type t ON p.F_TypeID =t.ID ");
            }
            else if (clerk.Contains(RoleId)) { type = 1;
                sql = string.Format(@" SELECT F_ProjectEngineer '项目工程师', F_Customer '客户', F_Model '型号', F_Spc '名称规格', t.TypeName '类型',
                     F_SampleQty '样板数量', F_OrderNo '订单号', F_OrderItem '立项单',(CASE F_Choose when 1 then '自制' when 2 then '外协' ELSE '' end) '自制/外协', F_Clerk '工程文员',F_Workshop '车间',
                    CONVERT(varchar(100), F_SampleFinishDate, 23) '样板完成时间', F_FactFinishNum '实际完成数量',CONVERT(varchar(100), 
                    F_FactFinishDate, 23) '实际完成时间',F_BuyRemark '采购备注' FROM Sys_Project p
                    LEFT JOIN Sys_Type t ON p.F_TypeID =t.ID ");
            }
            else {
                sql = string.Format(@" SELECT F_ProjectEngineer '项目工程师', F_Customer '客户', F_Model '型号', F_Spc '名称规格', t.TypeName '类型',
                     F_SampleQty '样板数量', F_OrderNo '订单号', F_OrderItem '立项单',CONVERT(varchar(100), F_OrderItemDate, 23)  '立项时间', 
                    CONVERT(varchar(100), F_PlanSendSampleDate, 23) '计划寄样时间',CONVERT(varchar(100), F_SampleReturnDate, 23) '样板回厂时间',
                    F_Remark '备注', (CASE F_Choose when 1 then '自制' when 2 then '外协' ELSE '' end) '自制/外协',CONVERT (VARCHAR (100), F_FactSendSampleDate, 23) '实际寄样时间', 
                    F_SampleOnTime '样板准时', (CASE F_IsAudit when 'F' then '是' when 'F' then '否' ELSE '' end) '是否审批', F_AccuracyRate '样板准确率',CONVERT(varchar(100), F_AccuracyDate, 23) '样板批准时间', 
                    CONVERT(varchar(100), F_PublishDate, 23) '产品发布时间',(CASE F_SignSample when 'F' then '有' when 'F' then '无' ELSE '' end)  '签板', F_ProjectRemark '审批备注' FROM Sys_Project p
                    LEFT JOIN Sys_Type t ON p.F_TypeID =t.ID  ");
            }
            sb.Append(sql);
            if (type != 0)
                sb.Append("where F_Choose=" + type);
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = sb.ToString();
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }

            string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";
            var savePath = Path.Combine(HttpContext.Server.MapPath("/Excel/"), fileName);
            new XZOA.Code.Excel.NPOIExcel().Export(dt, fileName, savePath);
            return Content(savePath);
        }

        public ActionResult CheckRole()
        {
            var roleType = 0;
            var RoleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var buyer = new RoleApp().GetKeyGridJson("采购员");
            var clerk = new RoleApp().GetRoleUser("文员");
            if (buyer.Contains(RoleId)) roleType = 2;
            else if (clerk.Contains(RoleId)) roleType = 1;
            return Json(roleType, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProjectEngineerList()
        {
            var user = userApp.GetUserList();
            var role = roleApp.GetRoleUser("工程师");
            var query = from u in user
                        join r in role
                        on u.F_RoleId equals r
                        select new TreeSelectModel
                        {
                            id = u.F_RealName,
                            text = u.F_RealName
                        };
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            ProjectEntity projectEntity = projectApp.GetForm(keyValue);
            
            return Content(projectEntity.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ProjectEntity projectEntity, string keyValue)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = file.FileName;
                if (!string.IsNullOrEmpty(fileName))
                {
                    foreach (char invalidChar in Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(invalidChar.ToString(), "_");
                    }
                    var name = fileName.Remove(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));
                    fileName = name + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                    var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/Project"), fileName);
                    file.SaveAs(filePath);
                    projectEntity.F_Annex = fileName;
                }
            }

            if (!string.IsNullOrEmpty(keyValue))
            {
                var Entity = projectApp.GetForm(keyValue);
                Entity.F_ProjectEngineer = projectEntity.F_ProjectEngineer;
                Entity.F_AccuracyDate = projectEntity.F_AccuracyDate;
                Entity.F_Choose = projectEntity.F_Choose;
                Entity.F_Customer = projectEntity.F_Customer;
                Entity.F_Model = projectEntity.F_Model;
                Entity.F_Spc = projectEntity.F_Spc;
                Entity.F_TypeID = projectEntity.F_TypeID;
                Entity.F_SampleQty = projectEntity.F_SampleQty;
                Entity.F_OrderNo = projectEntity.F_OrderNo;
                Entity.F_OrderItem = projectEntity.F_OrderItem;
                Entity.F_OrderItemDate = projectEntity.F_OrderItemDate;
                Entity.F_PlanSendSampleDate = projectEntity.F_PlanSendSampleDate;
                Entity.F_SampleReturnDate = projectEntity.F_SampleReturnDate;
                Entity.F_Remark = projectEntity.F_Remark;
                Entity.F_FactSendSampleDate = projectEntity.F_FactSendSampleDate;
                Entity.F_SampleOnTime = projectEntity.F_SampleOnTime;
                Entity.F_IsAudit = projectEntity.F_IsAudit;
                Entity.F_PublishDate = projectEntity.F_PublishDate;
                Entity.F_SignSample = projectEntity.F_SignSample;
                Entity.F_Annex = projectEntity.F_Annex;
                Entity.F_ProjectRemark = projectEntity.F_ProjectRemark;
                projectApp.SubmitForm(Entity, keyValue);
            }
            else
            {
                projectApp.SubmitForm(projectEntity);
            }

            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitBuyerForm(ProjectEntity projectEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var Entity = projectApp.GetForm(keyValue);
                Entity.F_Buyer = projectEntity.F_Buyer;
                Entity.F_Supplier = projectEntity.F_Supplier;
                Entity.F_Clerk = projectEntity.F_Clerk;
                Entity.F_Workshop = projectEntity.F_Workshop;
                Entity.F_SampleFinishDate = projectEntity.F_SampleFinishDate;
                Entity.F_FactFinishNum = projectEntity.F_FactFinishNum;
                Entity.F_FactFinishDate = projectEntity.F_FactFinishDate;
                Entity.F_BuyRemark = projectEntity.F_BuyRemark;
                projectApp.SubmitForm(Entity, keyValue);
            }

            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                projectApp.DeleteForm(keyValue);
                return Success("删除成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

    }
}