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

namespace XZOA.Web.Areas.TemplateManage.Controllers
{
    [HandlerLogin]
    public class TemplateController : ControllerBase
    {
        TemplateApp templateApp = new TemplateApp();
        OrganizeApp organizeApp = new OrganizeApp();
        DutyApp dutyApp = new DutyApp();
        MailHelper mHelper = new MailHelper();
        UserApp userApp = new UserApp();
        TempChargeApp tempChargeApp = new TempChargeApp();
        private readonly static object _MyLock = new object();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            TemplateEntity templateEntity = templateApp.GetForm(keyValue);
            return Content(templateEntity.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormDownload(string keyValue)
        {
            TemplateEntity templateEntity = templateApp.GetForm(keyValue);
            var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
            if(roleId== "2d032b54-1d1e-4990-908e-0343131b6d04")
            {
                templateEntity.PRT_IMG = "T";//已下载
                templateApp.SubmitForm(templateEntity,keyValue);
            }
            return Content(templateEntity.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormDetail(string keyValue)
        {
            List<TreeGridModel> treeGrids = new List<TreeGridModel>();
            TemplateEntity templateEntity = templateApp.GetForm(keyValue);
            List<TemplateEntity> list = templateApp.GetList(keyValue);
            foreach (var item in list)
            {
                TreeGridModel tree = new TreeGridModel();
                if(item.SOURCE_ID==null)
                {
                    tree.id = item.TEM_ID;
                    tree.text = item.PROCESS;
                    treeGrids.Add(tree);
                }
            }
            GetTreeList(treeGrids[0].id, treeGrids);
            return Json(new { data = templateEntity,list= treeGrids },JsonRequestBehavior.AllowGet);
        }

        private void  GetTreeList(string keyValue, List<TreeGridModel> treeGrids)
        {
            var temp = templateApp.GetFormBySourceID(keyValue);
            if (temp != null)
            {
                TreeGridModel tree = new TreeGridModel();
                tree.id = temp.TEM_ID;
                tree.text = temp.PROCESS;
                treeGrids.Add(tree);
                GetTreeList(temp.TEM_ID, treeGrids);
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var treeList = new List<TreeSelectModel>();
            var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var depart = organizeApp.GetForm(departId);
            var groupDepart = organizeApp.getDepartByGroup(depart.F_DepartGroupId);
            groupDepart.Add(departId);
            var checkRoleIds = dutyApp.GetTempCheckList();
            List<string> approvalRoleIds = null;
            var data = userApp.GetUserCheckList(groupDepart, checkRoleIds, approvalRoleIds);
            if (data != null)
            {
                foreach (var user in data)
                {
                    if (user != null)
                    {
                        TreeSelectModel treeModel = new TreeSelectModel();
                        treeModel.id = user.F_RealName;
                        treeModel.text = user.F_RealName;
                        treeList.Add(treeModel);
                    }
                }
            }
            treeList = treeList.ToList();
            treeList.Reverse();
            return Content(treeList.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetRATIFY_MANJson()
        {
            var treeList = new List<TreeSelectModel>();
            var data = tempChargeApp.GetTempCheckList();
            if (data != null)
            {
                foreach (var user in data)
                {
                    if (user != null)
                    {
                        TreeSelectModel treeModel = new TreeSelectModel();
                        treeModel.id = user.RATIFY_MAN;
                        treeModel.text = user.RATIFY_MAN;
                        treeList.Add(treeModel);
                    }
                }
            }
            treeList = treeList.ToList();
            return Content(treeList.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempApplyList(Pagination pagination,string keyword,DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var departName = organizeApp.GetForm(departId).F_FullName;
            var templates = templateApp.GetTempApplyList(pagination,keyword, departName,hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly] 
        public ActionResult GetTempCheckList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var templates = templateApp.GetTempCheckList(pagination, keyword, sidx, sord, hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempNoticeList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var templates = templateApp.GetTempNoticeList(pagination, keyword, sidx, sord, hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempConfirmList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var templates = templateApp.GetTempConfirmList(pagination, keyword, sidx, sord, hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempAllotList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var templates = templateApp.GetTempAllotList(pagination, keyword, sidx, sord, hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempAcceptList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var templates = templateApp.GetTempAcceptList(pagination, keyword, sidx, sord, hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempReportList(Pagination pagination, string keyword, DateTime? hopeDDBeginDate, DateTime? hopeDDEndDate, string closeID, string accept)
        {
            var templates = templateApp.GetTempReportList(pagination, keyword, hopeDDBeginDate, hopeDDEndDate,closeID,accept);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempExportList(Pagination pagination,DateTime? finishBeginDate, DateTime? finishEndDate)
        {
            var templates = templateApp.GetTempExportList(pagination, finishBeginDate, finishEndDate);
            var data = new
            {
                rows = templates,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTempChargeForm(string keyValue)
        {
            var data = tempChargeApp.GetForm(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMatFee(string keyValue)
        { 
            try
            {
                var data = new MaterialPriceApp().GetForm(keyValue);
                return Content(data.ToJson());
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
       // [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(TemplateEntity templateEntity, string keyValue)
        {
            var num = 0;
          
            lock(_MyLock)
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files["TEM_IMG"];
                        if (!string.IsNullOrEmpty(file.FileName))
                        {
                            num = 1;
                            var fileName = file.FileName;
                            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                            {
                                fileName = fileName.Replace(invalidChar.ToString(), "_");
                            }
                            fileName = fileName.Replace(" ", "");
                            var name = fileName.Remove(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));
                            fileName = name + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                            var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), fileName);
                            file.SaveAs(filePath);
                            templateEntity.TEM_IMG = fileName;
                        }
                        for (int i = num; i < Request.Files.Count; i++)
                        {
                            var file0 = Request.Files[i];
                            if (!string.IsNullOrEmpty(file0.FileName))
                            {
                                var fileName = file0.FileName;
                                foreach (char invalidChar in Path.GetInvalidFileNameChars())
                                {
                                    fileName = fileName.Replace(invalidChar.ToString(), "_");
                                }
                                fileName = fileName.Replace(" ", "");
                                var name = fileName.Remove(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf("."));
                                fileName = name + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                                var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), fileName);
                                file0.SaveAs(filePath);
                                if (string.IsNullOrEmpty(templateEntity.DRAWING))
                                {
                                    templateEntity.DRAWING = fileName;
                                }
                                else
                                { templateEntity.DRAWING += "/" + fileName; }

                            }
                        }
                    }
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        templateEntity.TEM_NO = templateApp.ProduceNo();
                        templateEntity.BACK_TAG = "F";
                        templateEntity.TEM_ACCEPT = "D";
                        templateEntity.NOTICE_GET = "F";
                        templateEntity.PRT_TAG = "F";
                        templateEntity.PRT_IMG = "F";
                        templateEntity.AUDIT_TAG = "F";
                        templateEntity.CLOSE_ID = "F";
                        templateEntity.RATIFY_TAG = "T";
                        templateEntity.TEM_DEP = templateEntity.RATIFY_MAN;
                        templateEntity.APP_DEP = organizeApp.GetForm(OperatorProvider.Provider.GetCurrent().DepartmentId).F_FullName;
                        templateEntity.CREATE_USER = OperatorProvider.Provider.GetCurrent().UserName;
                        templateEntity.CREATE_DATE = DateTime.Now;
                        templateEntity.RATIFY_DATE = templateEntity.CREATE_DATE;
                        templateEntity.CREATE_USER_ID = OperatorProvider.Provider.GetCurrent().UserCode;
                        templateEntity.TEM_IMG = templateEntity.TEM_IMG;
                        templateApp.SubmitForm(templateEntity);
                        var AUDIT_MAN = userApp.GetFormByName(templateEntity.AUDIT_MAN);
                        if (!string.IsNullOrEmpty(AUDIT_MAN.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(AUDIT_MAN.F_Email, "打样审核", "你好," + templateEntity.CREATE_USER + "有打样申请需要你登录OA去做批准,请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }
                    else
                    {
                        var temp = templateApp.GetForm(keyValue);
                        if (temp.AUDIT_MAN != templateEntity.AUDIT_MAN)
                        {
                            var AUDIT_MAN = userApp.GetFormByName(templateEntity.AUDIT_MAN);
                            if (!string.IsNullOrEmpty(AUDIT_MAN.F_Email))
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(AUDIT_MAN.F_Email, "打样审核", "你好," + templateEntity.CREATE_USER + "有打样申请需要你登录OA去做批准,请点击链接<a>http://10.110.120.6:8090/</a>");
                            }
                        }
                        temp.TEM_NAME = templateEntity.TEM_NAME;
                        temp.CUSTOMER = templateEntity.CUSTOMER;
                        temp.TEM_TYPE = templateEntity.TEM_TYPE;
                        temp.TEM_NUM = templateEntity.TEM_NUM;
                        temp.MAT_REQ = templateEntity.MAT_REQ;
                        temp.AUDIT_MAN = templateEntity.AUDIT_MAN;
                        temp.PROCESS = templateEntity.PROCESS;
                        temp.HOPE_DD = templateEntity.HOPE_DD;
                        temp.DRAWING = templateEntity.DRAWING;
                        temp.DRAWING_ALIAS = templateEntity.DRAWING_ALIAS;
                        temp.TEM_IMG = templateEntity.TEM_IMG;
                        temp.DRAWING = templateEntity.DRAWING;
                        temp.RATIFY_MAN = templateEntity.RATIFY_MAN;
                        temp.TEM_CHARGE = templateEntity.TEM_CHARGE;
                        temp.SIZE = templateEntity.SIZE;
                        temp.TEM_REMARK = templateEntity.TEM_REMARK;
                        templateApp.SubmitForm(temp, keyValue);
                    }
                }
                catch (Exception ex)
                {

                    new ErrorLogApp().SubmitForm(ex);
                }
            }
            return Success("操作成功！");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult UploadFile(string keyValue)
        {
            TemplateEntity temp = null;
            if (string.IsNullOrEmpty(keyValue))
            {
                temp = templateApp.GetForm(keyValue);
            }
            if (Request.Files.Count > 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        var fileName = file.FileName;
                        foreach (char invalidChar in Path.GetInvalidFileNameChars())
                        {
                            fileName = fileName.Replace(invalidChar.ToString(), "_");
                        }
                        fileName = fileName.Replace(" ", "");
                        var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), fileName);
                        file.SaveAs(filePath);
                        if (string.IsNullOrEmpty(temp.DRAWING))
                        {
                            temp.DRAWING = fileName;
                        }
                        else
                        { temp.DRAWING += "/" + fileName; }
                    }

                }
                templateApp.SubmitForm(temp, keyValue);
            }
            return Success("操作成功");
        }

        [HttpPost]
        [HandlerAjaxOnly]
       // [ValidateAntiForgeryToken]
        public ActionResult SubmitConfirmForm(TemplateEntity templateEntity, string keyValue,string type)
        {
            lock (_MyLock) {
                var temp = templateApp.GetForm(keyValue);
                temp.CLOSE_ID = "T";
                templateApp.SubmitForm(temp, keyValue);
                if (type != null && type.Equals("F"))
                {
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (!string.IsNullOrEmpty(file.FileName))
                        {
                            var fileName = file.FileName;
                            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                            {
                                fileName = fileName.Replace(invalidChar.ToString(), "_");
                            }
                            fileName = fileName.Replace(" ", "");
                            var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), fileName);
                            file.SaveAs(filePath);
                            templateEntity.DRAWING = fileName;
                        }

                    }
                    templateEntity.TEM_NO = templateApp.ProduceNo();
                    templateEntity.BACK_TAG = "F";
                    templateEntity.TEM_ACCEPT = "D";
                    templateEntity.NOTICE_GET = "F";
                    templateEntity.PRT_TAG = "F";
                    templateEntity.PRT_IMG = "F";
                    templateEntity.AUDIT_TAG = "F";
                    templateEntity.CLOSE_ID = "F";
                    templateEntity.RATIFY_TAG = "T";
                    templateEntity.SOURCE_ID = temp.TEM_ID;
                    templateEntity.PROCESS_STEP_NUM = temp.PROCESS_STEP_NUM + 1;
                    templateEntity.APP_DEP = organizeApp.GetForm(OperatorProvider.Provider.GetCurrent().DepartmentId).F_FullName;
                    templateEntity.CREATE_USER = OperatorProvider.Provider.GetCurrent().UserName;
                    templateEntity.CREATE_DATE = DateTime.Now;
                    templateEntity.RATIFY_DATE = templateEntity.CREATE_DATE;
                    templateEntity.CREATE_USER_ID = OperatorProvider.Provider.GetCurrent().UserCode;
                    templateApp.SubmitForm(templateEntity);
                    var user = userApp.GetFormByName(templateEntity.AUDIT_MAN);
                    if (user != null && !string.IsNullOrEmpty(user.F_Email))
                    {
                        mHelper.MailServer = "10.110.120.2";
                        mHelper.Send(user.F_Email, "样板审核", "你好,有样板需要你审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                    }
                }
            }
          
            return Success("操作成功！");
        }

        [HttpGet]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        public ActionResult ExportExcel(string keyValue,string keyword, DateTime? hopeDDBeginDate, DateTime? hopeDDEndDate, string closeID, string accept)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT ROW_NUMBER() 
                                 OVER (ORDER BY CREATE_DATE ASC) AS '序号',
                                 TEM_NO AS '打样编号',
                                 TEM_NAME AS '样板名称',
                                 CUSTOMER AS '客户',
                                 TEM_TYPE AS '样板类型',
                                 TEM_NUM AS '套数',
                                 MAT_REQ AS '材料要求',
                                 CONVERT(VARCHAR,HOPE_DD,23)  AS '期望交期',
                                 SIZE AS '关键尺寸',
                                 TEM_DEP AS '打样部门',
                                 TEM_CHARGE AS '负责人',
                                 APP_DEP AS '申请部门',
                                 CREATE_USER AS '申请人',
                                 (CASE AUDIT_TAG
                                 WHEN 'T' THEN '已审核'
                                 ELSE '未审核' END) AS '是否审核',
                                 AUDIT_MAN AS '审核人',
                                 AUDIT_DATE AS '审核日期',
                                 PROCESS AS '主工艺',
                                 (CASE TEM_ACCEPT
                                 WHEN 'T' THEN '已接收'
                                 WHEN 'F' THEN '已拒收'
                                 ELSE '待接收' END) AS '接受状态',
                                 BACK_REA AS '退回原因',
                                 (CASE NOTICE_GET
                                 WHEN 'T' THEN '已通知'
                                 ELSE '待通知' END) AS '是否通知',
                                 NOTICE_MAN AS '通知人',
                                 NOTICE_DATE AS '完成日期',
                                 MAT_FEE AS '材料费用',
                                 MDDE_FEE AS '制造费用',
                                 TEM_REMARK AS '备注',
                                 WEIGHT AS '重量(KG)',
                                 MDDE_FEE AS '材质',
                                 GROUP_ID AS '跟踪单号',
                                 GROUP_NAME AS '组别'
                                 FROM Sys_Template WHERE 1=1");

                    StringBuilder sb = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var ap = string.Format("AND ( TEM_NO LIKE '%{0}%' OR TEM_NAME LIKE '%{0}%' OR CUSTOMER LIKE '%{0}%' OR TEM_TYPE LIKE '%{0}%' OR TEM_CHARGE LIKE '%{0}%' OR TEM_NAME LIKE '%{0}%') ", keyword);
                        sb.Append(ap);
                    }
                    if (hopeDDBeginDate != null&& hopeDDEndDate!=null)
                    {
                        var ap = string.Format("AND HOPE_DD>= '{0}' AND HOPE_DD <= '{1}'", hopeDDBeginDate,hopeDDEndDate);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(closeID))
                    {
                        var ap = string.Format("AND CLOSE_ID = '{0}' ", closeID);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(accept))
                    {
                        var ap = string.Format("AND TEM_ACCEPT = '{0}' ", accept);
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
        public ActionResult ExportTempExcel(string keyValue, DateTime? hopeDDBeginDate, DateTime? hopeDDEndDate)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT ROW_NUMBER() 
                                 OVER (ORDER BY CREATE_DATE ASC) AS '序号',
                                 TEM_NAME '样板名称',
                                 CUSTOMER '客户',
                                 TEM_TYPE '样板类型',
                                 TEM_NUM '套数',
                                 CONVERT(VARCHAR,NOTICE_DATE,23) '完工日期',
                                 RATIFY_MAN '打样部门',
                                 GROUP_NAME '组别',
                                 APP_DEP '申请部门',
                                 TEM_CHARGE '负责人',
                                 AUDIT_MAN '申请人',
                                 MAT_FEE '材料费用',
                                 MDDE_FEE '制造成本',
                                 WEIGHT '重量(KG)',
                                 WORK_TIME '工时' 
                                 FROM Sys_Template WHERE 1=1");

                    StringBuilder sb = new StringBuilder(sql);
                    if (hopeDDBeginDate != null && hopeDDEndDate != null)
                    {
                        var ap = string.Format("AND NOTICE_DATE >= '{0}' AND NOTICE_DATE < '{1}'", hopeDDBeginDate, hopeDDEndDate.Value.AddDays(1));
                        sb.Append(ap);
                    }
                //    sb.Append(" ORDER BY CONVERT(VARCHAR,HOPE_DD,23)");
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
        // [ValidateAntiForgeryToken]
        public ActionResult SubmitFreeForm(TemplateEntity templateEntity, string keyValue)
        {
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    var temp = templateApp.GetForm(keyValue);
                    temp.TAKE_MAN = templateEntity.TAKE_MAN;
                    temp.FINISH_DATE = DateTime.Now;
                    temp.FINISH_NUM = templateEntity.FINISH_NUM;
                    temp.MAT_FEE = templateEntity.MAT_FEE;
                    temp.MDDE_FEE = templateEntity.MDDE_FEE;
                    temp.WEIGHT = templateEntity.WEIGHT;
                    temp.WORK_TIME = templateEntity.WORK_TIME;
                    temp.GROUP_NAME = templateEntity.GROUP_NAME;
                    temp.MATERIAL = templateEntity.MATERIAL;
                    templateApp.SubmitForm(temp, keyValue);
                }

                return Success("操作成功！");
            }
            catch (Exception ex)
            {

                return Error("操作失败！");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        // [ValidateAntiForgeryToken]
        public ActionResult SubmitNoticeForm(TemplateEntity templateEntity, string keyValue)
        {
            try
            {

                if (!string.IsNullOrEmpty(keyValue))
                {
                    var temp = templateApp.GetForm(keyValue);
                    temp.TAKE_MAN = templateEntity.TAKE_MAN;
                    temp.FINISH_DATE = DateTime.Now;
                    temp.FINISH_NUM = templateEntity.FINISH_NUM;
                    temp.MAT_FEE = templateEntity.MAT_FEE;
                    temp.MDDE_FEE = templateEntity.MDDE_FEE;
                    temp.WEIGHT = templateEntity.WEIGHT;
                    temp.WORK_TIME = templateEntity.WORK_TIME;
                    temp.GROUP_NAME = templateEntity.GROUP_NAME;
                    temp.MATERIAL = templateEntity.MATERIAL;
                    temp.NOTICE_GET = "T";//已通知
                    temp.NOTICE_MAN = OperatorProvider.Provider.GetCurrent().UserName;
                    temp.NOTICE_DATE = DateTime.Now;
                    templateApp.SubmitForm(temp, keyValue);
                    var appMan = userApp.GetFormByName(temp.CREATE_USER);
                    if (appMan!=null&&!string.IsNullOrEmpty(appMan.F_Email))
                    {
                        mHelper.MailServer = "10.110.120.2";
                        mHelper.Send(appMan.F_Email, "样板领取", "你好,有样板需要你登录OA领取,请点击链接<a>http://10.110.120.6:8090/</a>");
                    }
                }
                
            }
            catch (Exception ex)
            {

                return Error("操作失败！");
            }
            return Success("操作成功！");
        }


        [HttpPost]
        [HandlerAjaxOnly]
       // [ValidateAntiForgeryToken]
        public ActionResult SubmitCheckForm(TemplateEntity templateEntity,string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                List<string> authList = new List<string>();
                List<string> appList = new List<string>();
                foreach (var Id in Ids)
                {
                    var template = templateApp.GetForm(Id);
                    template.AUDIT_TAG = templateEntity.AUDIT_TAG;
                    template.AUDIT_DATE = DateTime.Now;
                    if (templateEntity.AUDIT_TAG == "N")
                    {
                        template.BACK_REA = templateEntity.BACK_REA;
                    }
                    templateApp.SubmitForm(template, Id);
                    if (!appList.Contains(template.CREATE_USER))
                    {
                        appList.Add(template.CREATE_USER);
                    }
                    if (!authList.Contains(template.TEM_CHARGE))
                    {
                        authList.Add(template.TEM_CHARGE);
                    }
                }
                if (templateEntity.AUDIT_TAG == "N")
                {
                    foreach (var item in appList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (user != null && !string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "样板审核不通过", "你好,你的样板审核不通过,详情查看请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }

                }
                else
                {
                    foreach (var item in authList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (user!=null&&!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "样板接收", "你好,有样板需要你接收,请点击链接<a>http://10.110.120.6:8090/</a>");
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

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                foreach (var key in Ids)
                {
                    templateApp.DeleteForm(key);
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
       // [HandlerAuthorize]
       // [ValidateAntiForgeryToken]
        public ActionResult ReceiveTemplate(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                foreach (var key in Ids)
                {
                    var temp = templateApp.GetForm(key);
                    temp.TEM_ACCEPT = "T";
                    templateApp.SubmitForm(temp,key);
                }
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
       // [HandlerAuthorize]
       // [ValidateAntiForgeryToken]
        public ActionResult RejectTemplate(string keyValue,string BACK_REA)
        {
            try
            {
                lock(_MyLock)
                {
                    string[] Ids = keyValue.Split(',');
                    foreach (var key in Ids)
                    {
                        var temp = templateApp.GetForm(key);
                        temp.TEM_ACCEPT = "F";
                        temp.BACK_REA = BACK_REA;
                        templateApp.SubmitForm(temp, key);
                    }
                }
                return Success("操作成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Print(string keyValue)
        {

            List<TemplateEntity> tempList = new List<TemplateEntity>();
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    string[] keys = keyValue.Split(',');
                    var GROUP_ID = "";
                    foreach (var k in keys)
                    {
                        TemplateEntity template = templateApp.GetForm(k);
                        template.PRT_TAG = "T";
                        if (!string.IsNullOrEmpty(GROUP_ID))
                        {
                            template.GROUP_ID = GROUP_ID;
                        }
                        else
                        {
                            GROUP_ID = templateApp.ProduceGroupID();
                            template.GROUP_ID = GROUP_ID;
                        }
                        templateApp.SubmitForm(template, k);

                        tempList.Add(template);
                    }
                 
                }
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
            }
            return View(tempList);
        }

        [HttpPost]
        public void DownloadTemplate(string keyValue)
        {
            
            try
            {
                string filename = Server.UrlDecode(keyValue);
                string filepath = Path.Combine(Server.MapPath("/Uploads/"), keyValue);
                if (FileDownHelper.FileExists(filepath))
                {
                    FileDownHelper.DownLoadold(filepath, filename);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult CheckIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ExportIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ReceiveIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult DistribuIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult NoticeIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ConfirmIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ReportIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult CheckForm()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult DownForm()
        {
            return View();
        }

        [HttpGet]
       // [HandlerAuthorize]
        public ActionResult NoticeForm()
        {
            return View();
        }
        
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ConfirmForm()
        {
            return View();
        }
        
        
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult rejectForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FreeForm()
        {
            return View();
        }
    }
}