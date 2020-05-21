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

namespace XZOA.Web.Areas.LeaveManage.Controllers
{
    [HandlerLogin]
    public class HistoryController : ControllerBase
    {
        // GET: LeaveManage/History
        public ActionResult PersonalLeaveHistoryIndex()
        {
            return View();
        }

        public ActionResult LeaveHistoryIndex()
        {
            return View();
        }
        
        public ActionResult UploadForm()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportExcel(string keyValue,string keyword, string depart, string beginDate, string endDate)
        {
            
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {
                   
                    string sql = string.Format(@"SELECT F_UserName '姓名',F_Account '工号',F_Department '部门',F_Duty '职位',F_Sex '性别',
                    CONVERT(VARCHAR,F_CreateTime,23) '填写日期',
                    CONVERT(VARCHAR,F_BeginTime,120) '开始日期',
                    CONVERT(VARCHAR,F_EndTime,120) '结束日期',
                    CONVERT(VARCHAR,F_ApprovalTime,120) '批准日期',
                     convert(varchar, F_TimeLength_Day) + '天' + convert(varchar, F_TimeLength_Hour) + '小时' + convert(varchar, F_TimeLength_Minute) + '分钟' '时长',
                     (CASE F_LeaveTypeId
                     WHEN 0 THEN '正常请假'
                     WHEN 1 THEN '特殊情况（补办请假手续）' END) AS '请假类型',
                     (CASE F_VacationTypeId
                     WHEN 0 THEN '年假'
                     WHEN 1 THEN '病假'
                     WHEN 2 THEN '事假' END) AS '假期类型',   
                     F_LeaveReason '请假原因',
                     (CASE F_IsApproval
                     WHEN 1 THEN '是'
                     ELSE '否' END) AS '是否批准',
                     F_CheckUserName '审核人',
                     F_ApprovalUserName '批准人',
                     (CASE F_IsOffLine
                     WHEN 0 THEN '在线审批'
                     WHEN 1 THEN '线下审批' END) AS '审批类型',
                     (CASE
                     WHEN F_FileId IS NULL THEN '否'
                     ELSE '是' END) AS '是否含附件'
                     FROM Sys_Leave WHERE F_IsApproval=1 ");

                    StringBuilder sb = new StringBuilder(sql);
                    if (!string.IsNullOrEmpty(keyValue)&&keyValue== "getPersonal")
                    {
                        var ap = string.Format(" And F_UserId='{0}'", UserId);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        var ap = string.Format(" And (F_UserName LIKE '%{0}%' OR F_Account LIKE '%{0}%')", keyword);
                        sb.Append(ap);
                    }
                    if (!string.IsNullOrEmpty(depart))
                    {
                        if(depart!= "5AB270C0-5D33-4203-A54F-4552699FDA3C")
                        {
                            var ap = string.Format(" And F_DepartmentId='{0}'", depart);
                            sb.Append(ap);
                        }
                        
                    }
                    if (!string.IsNullOrEmpty(beginDate))
                    {
                        DateTime eDate = Convert.ToDateTime(endDate).AddDays(1);
                        var ap = string.Format(" And ((F_BeginTime >= '{0}' AND F_BeginTime < '{1}') OR (F_EndTime >= '{0}' AND F_EndTime < '{1}')) ", beginDate,eDate);
                        sb.Append(ap);
                    }
                    sb.Append(" ORDER BY F_CreateTime DESC");
                    command.CommandText = sb.ToString();
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            string path = HttpContext.Server.MapPath("/Excel/");

            string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";

            //设置新建文件路径及名称
            string savePath = path + fileName;

            if (dt != null && dt.Rows.Count > 0)
            {
                new NPOIExcel().Export(dt, fileName, savePath);

                return Content(savePath);
            }
            else {
                return Error("数据为空");
            }
            
           
        }

        /// <summary>
        /// 汇总
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GatherExcel()
        {

            var UserId = OperatorProvider.Provider.GetCurrent().UserId;

            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand command = conn.CreateCommand())
                {

                    string sql = string.Format(@"SELECT
                    	(DATEPART(MONTH, GETDATE()) - 1) '月份',
                    	CONVERT (VARCHAR (100),F_CreateTime,23) '填写日期',
                    	F_Department '部门',
                    	F_Account '工号',
                    	F_Duty '职位',
                    	F_UserName '请假人姓名',
                    	CONVERT (VARCHAR (100),F_BeginTime,20) + '-' + CONVERT (VARCHAR(100), F_EndTime, 20) '请假日期',
                    	CONVERT (
                    		DECIMAL (18, 3),
                    		(
                    			DATEDIFF(DAY, F_BeginTime, F_EndTime) * 8 + CONVERT (
                    				FLOAT,
                    				DATEDIFF(
                                        MINUTE,
                                        CONVERT (VARCHAR (100),F_BeginTime,24),
                    					CONVERT (VARCHAR(100), F_EndTime, 24)
                    				)
                    			) / 60 
                    		) / 8
                    	) AS '天数',
                    	CONVERT (
                    		DECIMAL (18, 3),
                    		DATEDIFF(DAY, F_BeginTime, F_EndTime) * 8 + CONVERT (
                    			FLOAT,
                    			DATEDIFF(
                    				MINUTE,
                    				CONVERT (
                    					VARCHAR (100),
                    					F_BeginTime,
                    					24
                    				),
                    				CONVERT (VARCHAR(100), F_EndTime, 24)
                    			)
                    		) / 60
                    	) '请假合计（H）'
                    FROM
                    	Sys_Leave
                    WHERE
                    	F_LeaveStatus = 2
                    AND DATEPART(MONTH, F_BeginTime) = (
                    	DATEPART(MONTH, GETDATE()) - 1
                    ) ORDER BY F_Account DESC");

                    command.CommandText = sql;
                    using (SqlDataAdapter adp = new SqlDataAdapter(command))
                    {
                        adp.Fill(dt);
                    }
                }
            }
            string path = HttpContext.Server.MapPath("/Excel/");

            string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";

            //设置新建文件路径及名称
            string savePath = path + fileName;
            if (dt != null && dt.Rows.Count > 0)
            {
                new NPOIExcel().GatherExport(dt, fileName, savePath);

                return Content(savePath);
            }
            else
            {
                return Error("数据为空");
            }
        }

        [HttpPost]
        public void DownloadFile(string keyValue)
        {
            string filePath = Server.MapPath("~/Uploads/") + keyValue; //文件路径

            if (System.IO.File.Exists(filePath))//判断文件是否存在
            {
                FileDownHelper.DownLoadold(filePath, keyValue);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitUploadForm(string keyValue)
        {
            try
            {
                if(Request.Files.Count>0)
                {
                    var leaveEntity = new LeaveApp().GetForm(keyValue);
                    var file = Request.Files[0];
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        Guid FileID = Guid.NewGuid();
                        var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), FileID.ToString() + Path.GetExtension(file.FileName));
                        file.SaveAs(filePath);
                        leaveEntity.F_FileId = FileID;
                        leaveEntity.F_FileName = file.FileName;
                        leaveEntity.F_SuffixName = Path.GetExtension(file.FileName);
                        new LeaveApp().SubmitForm(leaveEntity, keyValue);
                    }
                }
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error("操作失败。");
            }
        }
    }

}