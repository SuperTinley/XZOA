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
using XZOA.Code.Excel;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Web.Areas.ProposalManage.Controllers
{
    [HandlerLogin]
    public class ProposalManageController : ControllerBase
    {
        ProposalApp proposalApp = new ProposalApp();
        OrganizeApp organizeApp = new OrganizeApp();
        RoleApp roleApp = new RoleApp();

        [HttpGet]
        public ActionResult ReciveForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult IEForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DisAgreeForm()
        {
            return View();
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetDepart()
        {
            try
            {
                var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
                var departName = organizeApp.GetForm(departId).F_FullName;
                return Content(departName);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetUserRoleRight(int? keyValue)
        {
            try
            {
                var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
                if(keyValue!=null)
                {
                    var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
                    var departName = organizeApp.GetForm(departId).F_FullName;
                    var pro_send_dep = proposalApp.GetForm(keyValue.Value).pro_send_dep;
                    var roleList = roleApp.GetManList();
                    if (roleList.Contains(roleId)&& pro_send_dep.Equals(departName))
                    {
                        return Json(true,JsonRequestBehavior.AllowGet);//接收负责人
                    }
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetUserRole()
        {
            try
            {
                var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
                var roleIdList = roleApp.GetRoleUser("IE工程师");
                if(roleIdList.Contains(roleId))
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetForm(int keyValue)
        {
            try
            {
                var proposal = proposalApp.GetForm(keyValue);
                return Content(proposal.ToJson());
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // GET: ProposalManage/ProposalManage
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetProposalList(Pagination pagination,string keyword,string pro_dep,string pro_send_dep,DateTime? planBeginDate, DateTime? planEndDate, DateTime? cheDDBeginDate, DateTime? cheDDEndDate,string status)
        {
            try
            {
                bool isIE = false, isRevice = false;
                var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
                var departName = organizeApp.GetForm(departId).F_FullName;
                if (OperatorProvider.Provider.GetCurrent().UserName != "admin")
                {
                    var RoleId = OperatorProvider.Provider.GetCurrent().RoleId;
                    var roleList = roleApp.GetManList();
                    var role = roleApp.GetForm(RoleId);
                    if (roleList.Contains(RoleId))
                    {
                        isRevice = true;
                    }
                    if (role!=null&&(role.F_FullName.Equals("IE工程师") || role.F_FullName.Equals("厂长")))
                    {
                        isIE = true;
                    }
                }
                else {
                    isIE = true;
                }
                var query = proposalApp.GetList(pagination, departName, keyword,pro_dep,pro_send_dep,planBeginDate,planEndDate,cheDDBeginDate,cheDDEndDate, status, isIE ,isRevice);
                var data = new
                {
                    rows = query,
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
                return Content(ex.ToJson());
            }

        }

        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        public ActionResult ExportExcel(string keyValue, string keyword, string pro_dep, string pro_send_dep, DateTime? planBeginDate, DateTime? planEndDate, DateTime? cheDDBeginDate, DateTime? cheDDEndDate, string status)
        {

            DataTable dt = new DataTable();
            var RoleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var role = roleApp.GetForm(RoleId);
            var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var departName = organizeApp.GetForm(departId).F_FullName;
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            bool flag = false;var isRevice = false;
            if (role!=null&&(role.F_FullName.Equals("IE工程师") || role.F_FullName.Equals("厂长")))
            {
                flag = true;
            }
            var roleList = roleApp.GetManList();
            if (roleList.Contains(RoleId))
            {
                isRevice = true;
            }
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT pro_id '编号',CONVERT(VARCHAR,pro_date,23) '提案时间',pro_dep '提案部门',
                                               pro_man '提案者', pro_title '提案标题',pro_send_dep '发往部门',reasons '接收部门意见',
                                               (CASE is_pass  WHEN 'Y' THEN '是' WHEN 'N' THEN '否' ELSE ''  END) '是否同意',imp_man '实施负责人',
                                               CONVERT(VARCHAR,plan_finish_date,23) '计划完成日期',CONVERT(VARCHAR,imp_finish_date,23) '实施完成日期',
                                               CONVERT(VARCHAR,plan_che_date,23) '实施验证日期',reward '奖励'
                                                FROM Sys_Proposal WHERE 1=1");

                    StringBuilder sb = new StringBuilder(sql);
                    if (!flag)
                    {
                        var ap = string.Format(" AND ( pro_dep = '{0}' OR pro_send_dep = '{0}')", departName);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var ap = string.Format(" AND ( pro_man LIKE '%{0}%' OR pro_id LIKE '%{0}%' OR imp_man LIKE '%{0}%') ", keyword);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(pro_dep))
                    {
                        var ap = string.Format(" AND pro_dep = '{0}'", pro_dep);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(pro_send_dep))
                    {
                        var ap = string.Format(" AND pro_send_dep = '{0}'", pro_send_dep);
                        sb.Append(ap);
                    }
                    if (planBeginDate != null && planEndDate != null)
                    {
                        var ap = string.Format(" AND plan_finish_date >= '{0}' AND plan_finish_date <= '{1}' ", planEndDate, planEndDate);
                        sb.Append(ap);
                    }
                    if (cheDDBeginDate != null && cheDDBeginDate != null)
                    {
                        var ap = string.Format(" AND plan_che_date >= '{0}' AND plan_che_date <= '{1}' ", cheDDBeginDate, cheDDEndDate);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(status))
                    {
                        if (status.Equals("finish"))
                        {
                            var ap = string.Format(" AND is_pass = 'Y' AND plan_che_date IS NOT NULL");
                            sb.Append(ap);
                        }
                        else if (status.Equals("revice"))
                        {
                            var ap = string.Format(" AND is_pass IS NULL");
                            sb.Append(ap);
                        }
                        else if (status.Equals("confirm"))
                        {
                            var ap = string.Format(" AND imp_finish_date IS NOT NULL");//已确认
                            sb.Append(ap);
                        }
                        else
                        {
                            var ap = string.Format(" AND is_pass = 'N'");
                            sb.Append(ap);
                        }

                    }
                    if (!flag)//非IE工程师
                    {
                        if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(pro_dep) && string.IsNullOrEmpty(pro_send_dep) && planBeginDate == null && cheDDBeginDate == null && string.IsNullOrEmpty(status))//非搜索状态
                        {
                            if (isRevice)//接收人
                            {
                                var ap = string.Format(" AND ((is_pass IS NULL AND pro_send_dep ='{0}') OR pro_man='{1}') AND imp_finish_date IS NULL AND plan_che_date IS NULL ", departName, userName);
                                sb.Append(ap);
                            }
                            else
                            {
                                var ap = string.Format(" AND pro_man='{0}' AND imp_finish_date IS NULL AND plan_che_date IS NULL ",userName);
                                sb.Append(ap);
                            }

                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(pro_dep) && string.IsNullOrEmpty(pro_send_dep) && planBeginDate == null && cheDDBeginDate == null && string.IsNullOrEmpty(status))
                        {
                            var ap = string.Format("AND imp_finish_date IS NULL AND plan_che_date IS NULL ");
                            sb.Append(ap);
                        }
                    }
                    sb.Append(" ORDER BY ID DESC");
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
        public ActionResult GetTreeJson()
        {
            List<TreeSelectModel> treeGrids = new List<TreeSelectModel>();
            var query = organizeApp.GetDepartList();
            foreach (var item in query)
            {
                TreeSelectModel tree = new TreeSelectModel();
                tree.id = item.F_FullName;
                tree.text = item.F_FullName;
                treeGrids.Add(tree);
            }
            return Content(treeGrids.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMaxProId()
        {
            var pro_id = "";
            var old = 0;
            if (proposalApp.GetFirstForm()!=null)
            {
                old = Convert.ToInt32(proposalApp.GetFirstForm().pro_id);
            }
             var num = old + 1;
             for (int i = 0; i < 6 - (old + 1).ToString().Length; i++)
             {
                 pro_id += "0";
             }
                pro_id += num;
          
           
            return Content(pro_id);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetUser()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var DepartmentId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var departName = organizeApp.GetForm(DepartmentId).F_FullName;
            var data = new {
                userName= userName,
                departName = departName
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                int[] keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var key in keys)
                {
                    proposalApp.DeleteForm(key);
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
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ProposalEntity proposalEntity,int? keyValue)
        {
            try
            {
                if (keyValue == null)
                {
                    proposalEntity.make_bill_man = OperatorProvider.Provider.GetCurrent().UserName;
                    proposalEntity.make_bill_date = DateTime.Now;
                    proposalApp.SubmitForm(proposalEntity);
                }
                else {
                    var proposal = proposalApp.GetForm(keyValue.Value);
                    proposal.pro_title = proposalEntity.pro_title;
                    proposal.pro_id = proposalEntity.pro_id;
                    proposal.pro_man = proposalEntity.pro_man;
                    proposal.pro_date = proposalEntity.pro_date;
                    proposal.pro_dep = proposalEntity.pro_dep;
                    proposal.pro_send_dep = proposalEntity.pro_send_dep;
                    proposal.is_pass = null;
                    proposal.imp_man = null;
                    proposal.plan_finish_date = null;
                    proposal.reasons = null;
                    proposalApp.SubmitForm(proposal,keyValue);
                }
               
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitEditForm(ProposalEntity proposalEntity, int? keyValue)
        {
            try
            {
                if(keyValue != null){
                    var proposal = proposalApp.GetForm(keyValue.Value);
                    proposal.pro_send_dep = proposalEntity.pro_send_dep;
                    proposal.is_pass = proposalEntity.is_pass;
                    proposal.imp_man = proposalEntity.imp_man;
                    proposal.plan_finish_date = proposalEntity.plan_finish_date;
                    proposal.reasons = proposalEntity.reasons;
                    proposalApp.SubmitForm(proposal, keyValue);
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Success("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitReviceForm(ProposalEntity proposal,int keyValue)
        {
            try
            {
                var proposalEntity = proposalApp.GetForm(keyValue);
                proposalEntity.reasons = proposal.reasons;
                proposalEntity.is_pass = proposal.is_pass;
                proposalEntity.imp_man = proposal.imp_man;
                proposalEntity.plan_finish_date = proposal.plan_finish_date;
                proposalApp.SubmitForm(proposalEntity, keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitIEForm(ProposalEntity proposal, int keyValue)
        {
            try
            {
                var proposalEntity = proposalApp.GetForm(keyValue);
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files["annex"];
                        if (!string.IsNullOrEmpty(file.FileName))
                        {
                            var fileName = file.FileName;
                            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                            {
                                fileName = fileName.Replace(invalidChar.ToString(), "_");
                            }
                            fileName = fileName.Replace(" ", "");
                            var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/Proposal/"), fileName);
                            proposalEntity.annex = file.FileName;
                            file.SaveAs(filePath);
                        }
                    }
                }
                proposalEntity.plan_che_date = proposal.plan_che_date;
                proposalEntity.reward = proposal.reward;
                proposalApp.SubmitForm(proposalEntity, keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
                return Error("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
       // [ValidateAntiForgeryToken]
        public ActionResult SubmitConfirmForm(int keyValue)
        {
            try
            {
                var proposalEntity = proposalApp.GetForm(keyValue);
                proposalEntity.imp_finish_date = DateTime.Now;
                proposalApp.SubmitForm(proposalEntity, keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult SubmitDisAgreeForm(ProposalEntity proposal, int keyValue)
        {
            try
            {
                var proposalEntity = proposalApp.GetForm(keyValue);
                proposalEntity.reasons = proposal.reasons;
                proposalEntity.is_pass = proposal.is_pass;
                proposalEntity.imp_man = proposal.imp_man;
                proposalEntity.plan_finish_date = proposal.plan_finish_date;
                proposalApp.SubmitForm(proposalEntity, keyValue);
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        public void DownloadProposal(string keyValue)
        {

            try
            {
                string filename = Server.UrlDecode(keyValue);
                string filepath = Path.Combine(Server.MapPath("/Uploads/Proposal/"), keyValue);
                if (FileDownHelper.FileExists(filepath))
                {
                    FileDownHelper.DownLoadold(filepath,filename);
                }
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
                throw;
            }
        }

        
       
    }
}