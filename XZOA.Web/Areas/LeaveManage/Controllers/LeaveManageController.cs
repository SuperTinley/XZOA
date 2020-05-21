using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Web;
using System.IO;
using System.Drawing.Printing;
using XZOA.Domain.Enum;
using XZOA.Domain.ViewModel;
using System.Net;

namespace XZOA.Web.Areas.LeaveManage.Controllers
{
    [HandlerLogin]
    public class LeaveManageController : ControllerBase
    {
        LeaveApp leaveApp = new LeaveApp();
        UserApp userApp = new UserApp();
        OrganizeApp organizeApp = new OrganizeApp();
        RoleApp roleApp = new RoleApp();
        MailHelper mHelper = new MailHelper();
        DutyApp dutyApp = new DutyApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetCount()
        {
            try
            {
                var checkCount = leaveApp.GetCheckCount();
                var approvalCount = leaveApp.GetApprovalCount();
                var tempCheckCount = new TemplateApp().GetTempCheckCount();
                var tempAcceptCount = new TemplateApp().GetTempAcceptCount();
                var applyCheckCount = new ApplyBillApp().GetApplyCheckCount();
                var applyApprovalCount = new ApplyBillApp().GetApplyApprovalCount();
                var purchaseCheckCount = new ApplyBillApp().GetPurchaseCheckCount();
                var purchaseApprovalCount = new ApplyBillApp().GetPurchaseApprovalCount();
                var data = new {
                    checkCount= checkCount,
                    approvalCount= approvalCount,
                    tempCheckCount = tempCheckCount,
                    tempAcceptCount= tempAcceptCount,
                    applyCheckCount= applyCheckCount,
                    applyApprovalCount= applyApprovalCount,
                    purchaseCheckCount= purchaseCheckCount,
                    purchaseApprovalCount= purchaseApprovalCount
                };
                return Content(data.ToJson());
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetStatusList(Pagination pagination,LeaveStatusEnum statusEnum)
         {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where l.F_UserId==UserId && (l.F_LeaveStatus==0|| l.F_LeaveStatus == 1||l.F_IsCheck!=true||l.F_IsApproval!=true)
                         select new {
                             F_Id=l.F_Id,
                             F_UserName=u.F_RealName,
                             F_Account=u.F_Account,
                             F_CreateTime=l.F_CreateTime,
                             F_BeginTime=l.F_BeginTime,
                             F_EndTime=l.F_EndTime,
                             F_TimeLength_Day=l.F_TimeLength_Day,
                             F_TimeLength_Hour = l.F_TimeLength_Hour,
                             F_TimeLength_Minute = l.F_TimeLength_Minute,
                             F_VacationTypeId = l.F_VacationTypeId,
                             F_LeaveTypeId = l.F_LeaveTypeId,
                             F_LeaveStatus = l.F_LeaveStatus,
                             F_FileId = l.F_FileId,
                             F_FileName = l.F_FileName,
                             F_SuffixName = l.F_SuffixName,
                             F_CheckUserName = l.F_CheckUserName,
                             F_ApprovalUserName = l.F_ApprovalUserName
                         }).OrderByDescending(i=>i.F_CreateTime).ToList();
            pagination.records = query.Count();
            query = query.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
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
        public ActionResult GetUnderLineList()
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where l.F_LeaveStatus < (int)LeaveStatusEnum.Success
                         select new
                         {
                             F_Id = l.F_Id,
                             F_UserName = u.F_RealName,
                             F_Account = u.F_Account,
                             F_CreateTime = l.F_CreateTime,
                             F_BeginTime = l.F_BeginTime,
                             F_EndTime = l.F_EndTime,
                             F_TimeLength_Day = l.F_TimeLength_Day,
                             F_TimeLength_Hour = l.F_TimeLength_Hour,
                             F_TimeLength_Minute = l.F_TimeLength_Minute,
                             F_VacationTypeId = l.F_VacationTypeId,
                             F_LeaveTypeId = l.F_LeaveTypeId,
                             F_LeaveStatus = l.F_LeaveStatus,
                             F_FileId = l.F_FileId,
                             F_FileName = l.F_FileName,
                             F_SuffixName = l.F_SuffixName,
                         }).OrderByDescending(i => i.F_CreateTime).ToList();
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetHistoryList(Pagination pagination,string keyValue,string keyword,string depart,string beginDate,string endDate)
        {
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where l.F_IsApproval==true
                         select new
                         {
                             F_Id = l.F_Id,
                             F_UserId=l.F_UserId,
                             F_UserName = u.F_RealName,
                             F_Account = u.F_Account,
                             F_CreateTime = l.F_CreateTime,
                             F_BeginTime = l.F_BeginTime,
                             F_EndTime = l.F_EndTime,
                             F_TimeLength_Day = l.F_TimeLength_Day,
                             F_TimeLength_Hour = l.F_TimeLength_Hour,
                             F_TimeLength_Minute = l.F_TimeLength_Minute,
                             F_VacationTypeId = l.F_VacationTypeId,
                             F_LeaveTypeId = l.F_LeaveTypeId,
                             F_LeaveStatus = l.F_LeaveStatus,
                             F_FileId = l.F_FileId,
                             F_FileName = l.F_FileName,
                             F_SuffixName = l.F_SuffixName,
                             F_IsOffLine = l.F_IsOffLine,
                             F_IsApproval=l.F_IsApproval,
                             F_DepartmentId=u.F_DepartmentId
                         }).OrderByDescending(i => i.F_CreateTime).ToList();
            if (keyValue == "getPersonal")
            {
                var UserId = OperatorProvider.Provider.GetCurrent().UserId;
                query = query.Where(l => l.F_UserId == UserId).ToList();
            }
            else {
                var RoleId = OperatorProvider.Provider.GetCurrent().RoleId;
                var DepartId = OperatorProvider.Provider.GetCurrent().DepartmentId;
                if (!organizeApp.IsAdministration(DepartId))//非人事行政部
                {
                    query = query.Where(l => l.F_DepartmentId == DepartId).ToList();
                }
            }
            if(!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(l => l.F_UserName.Contains(keyword) || l.F_Account.ToUpper().Contains(keyword.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(depart))
            {
                var departParent = organizeApp.GetForm(depart);
                if (departParent.F_ParentId!="0")
                {
                    query = query.Where(l => l.F_DepartmentId == depart).ToList();
                }
            }
            if (!string.IsNullOrEmpty(beginDate))
            {
                DateTime bDate = Convert.ToDateTime(beginDate);
                DateTime eDate = Convert.ToDateTime(endDate).AddDays(1);
                var  query1 = query.Where(l => (l.F_BeginTime >= bDate&&l.F_BeginTime < eDate)).ToList();
                var query2 = query.Where(l => (l.F_EndTime >= bDate && l.F_EndTime < eDate)).ToList();
                query = query1.Union(query2).ToList();
            }
            //if (!string.IsNullOrEmpty(endDate))
            //{
            //    DateTime Date = Convert.ToDateTime(endDate);
            //    query = query.Where(l => l.F_EndTime <= Date).ToList();
            //}
            pagination.records = query.Count();
            query = query.Skip((pagination.page-1)*pagination.rows).Take(pagination.rows).ToList();
            var data = new {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取历史记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSellOffHistoryList()
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where l.F_LeaveStatus >= (int)LeaveStatusEnum.UnCheckedSellOff && l.F_UserId==UserId
                         select new
                         {
                             F_Id = l.F_Id,
                             F_UserId = l.F_UserId,
                             F_UserName = u.F_RealName,
                             F_Account = u.F_Account,
                             F_CreateTime = l.F_CreateTime,
                             F_BeginTime = l.F_BeginTime,
                             F_EndTime = l.F_EndTime,
                             F_TimeLength_Day = l.F_TimeLength_Day,
                             F_TimeLength_Hour = l.F_TimeLength_Hour,
                             F_TimeLength_Minute = l.F_TimeLength_Minute,
                             F_VacationTypeId = l.F_VacationTypeId,
                             F_LeaveTypeId = l.F_LeaveTypeId,
                             F_LeaveStatus = l.F_LeaveStatus,
                             F_FileId = l.F_FileId,
                             F_FileName = l.F_FileName,
                             F_SuffixName = l.F_SuffixName,
                             F_IsOffLine = l.F_IsOffLine,
                             F_IsSellOff = l.F_IsSellOff,
                             F_SellOffTimeLength_Day = l.F_SellOffTimeLength_Day,
                             F_SellOffTimeLength_Hour = l.F_SellOffTimeLength_Hour,
                             F_SellOffTimeLength_Minute = l.F_SellOffTimeLength_Minute,
                             F_ResumptionLeaveTime = l.F_ResumptionLeaveTime
                         }).OrderByDescending(i => i.F_CreateTime).ToList();
            return Content(query.ToJson());
        }


        public ActionResult GetSellOffList()
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where l.F_UserId == UserId && l.F_EndTime > DateTime.Now && (l.F_LeaveStatus == (int)LeaveStatusEnum.Success || l.F_LeaveStatus == (int)LeaveStatusEnum.UnderLine)
                         select new
                         {
                             F_Id = l.F_Id,
                             F_UserName = u.F_RealName,
                             F_Account = u.F_Account,
                             F_CreateTime = l.F_CreateTime,
                             F_BeginTime = l.F_BeginTime,
                             F_EndTime = l.F_EndTime,
                             F_TimeLength_Day = l.F_TimeLength_Day,
                             F_TimeLength_Hour = l.F_TimeLength_Hour,
                             F_TimeLength_Minute = l.F_TimeLength_Minute,
                             F_VacationTypeId = l.F_VacationTypeId,
                             F_LeaveTypeId = l.F_LeaveTypeId,
                             F_LeaveStatus = l.F_LeaveStatus,
                             F_FileId = l.F_FileId,
                             F_FileName = l.F_FileName,
                             F_SuffixName = l.F_SuffixName
                         }).OrderByDescending(i => i.F_CreateTime).ToList();
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAllList()
        {
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         select new
                         {
                             F_Id = l.F_Id,
                             F_UserName = u.F_RealName,
                             F_Account = u.F_Account,
                             F_CreateTime = l.F_CreateTime,
                             F_BeginTime = l.F_BeginTime,
                             F_EndTime = l.F_EndTime,
                             F_TimeLength_Day = l.F_TimeLength_Day,
                             F_TimeLength_Hour = l.F_TimeLength_Hour,
                             F_TimeLength_Minute = l.F_TimeLength_Minute,
                             F_VacationTypeId = l.F_VacationTypeId,
                             F_LeaveTypeId = l.F_LeaveTypeId,
                             F_LeaveStatus = l.F_LeaveStatus,
                             F_FileId = l.F_FileId,
                             F_FileName = l.F_FileName,
                             F_SuffixName = l.F_SuffixName
                         }).OrderByDescending(i => i.F_CreateTime).ToList();
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetUserFormJson()
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var data = userApp.GetForm(UserId);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            if(!string.IsNullOrEmpty(keyValue))
            {
                LeaveEntity leave = leaveApp.GetForm(keyValue);
                return Content(leave.ToJson());
            }
            return null;
        }
        
        /// <summary>
        /// 获取审核获批准领导
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson(string type)
        {
            var treeList = new List<TreeSelectModel>();
            var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var depart = organizeApp.GetForm(departId);
            var groupDepart = organizeApp.getDepartByGroup(depart.F_DepartGroupId);
            groupDepart.Add(departId);
            var checkRoleIds = dutyApp.GetCheckList();
            List<string> approvalRoleIds = null;
            if(type.Equals("approval")){
                approvalRoleIds = dutyApp.GetApprovalList(type);
            }
            var data = userApp.GetUserCheckList(groupDepart, checkRoleIds, approvalRoleIds);
            if (data != null)
            {
                foreach (var user in data)
                {
                    if (user != null)
                    {
                        TreeSelectModel treeModel = new TreeSelectModel();
                        treeModel.id = user.F_Id;
                        treeModel.text = user.F_RealName;
                        treeList.Add(treeModel);
                    }
                }
            }
            else {//审核领导为空则加入角色为厂长的领导
                var Ids = dutyApp.GetApprovalList();
                var checkList= userApp.GetUserCheckList(groupDepart, Ids);
                foreach (var check in checkList)
                {
                    if (check != null)
                    {
                        TreeSelectModel treeModel = new TreeSelectModel();
                        treeModel.id = check.F_Id;
                        treeModel.text = check.F_RealName;
                        treeList.Add(treeModel);
                    }
                }
            }
            treeList=treeList.ToList();
            treeList.Reverse();
            return Content(treeList.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult ConvertToWord(string id,string printName)
        {
            LeaveEntity leave=leaveApp.GetForm(id);
            UserEntity user = userApp.GetForm(leave.F_UserId);
            UserEntity approvalLeader = userApp.GetForm(leave.F_ApprovalLeaderId);
            UserEntity checkLeader = userApp.GetForm(leave.F_CheckLeaderId);
            OrganizeEntity organ = organizeApp.GetForm(user.F_DepartmentId);
            RoleEntity role = roleApp.GetForm(user.F_RoleId);
            if(leaveApp.ConvertToWord(leave, user, checkLeader, approvalLeader, organ, role, printName))
            {
                return Success("操作成功。");
            }
            return Error("操作失败。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(LeaveEntity leaveEntity, string keyValue)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if(!string.IsNullOrEmpty(file.FileName))
                {
                    Guid FileID = Guid.NewGuid();
                    var fileName = file.FileName;
                    foreach (char invalidChar in Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(invalidChar.ToString(), "_");
                    }
                    fileName = fileName.Replace(" ", "");
                    var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/"), fileName);
                    file.SaveAs(filePath);
                    leaveEntity.F_FileId = FileID;
                    leaveEntity.F_FileName = file.FileName;
                    leaveEntity.F_SuffixName = Path.GetExtension(file.FileName);
                }
                
            }
            var man = userApp.GetForm(leaveEntity.F_UserId);
            leaveEntity.F_UserName = man.F_RealName;
            leaveEntity.F_Account = man.F_Account;
            leaveEntity.F_Sex = man.F_Gender != null ? (man.F_Gender == true ? "男" : "女") : null;
            var depart = organizeApp.GetForm(man.F_DepartmentId);
            var duty = roleApp.GetForm(man.F_DutyId);
            leaveEntity.F_Department = depart.F_FullName;
            leaveEntity.F_Duty = duty.F_FullName;
            var checkUser = userApp.GetForm(leaveEntity.F_CheckLeaderId);
            var approvalUser = userApp.GetForm(leaveEntity.F_ApprovalLeaderId);
            leaveEntity.F_CheckUserName = checkUser.F_RealName;
            leaveEntity.F_ApprovalUserName = approvalUser.F_RealName;
            leaveApp.SubmitForm(leaveEntity, keyValue);
            if(!string.IsNullOrEmpty(checkUser.F_Email))
            {
                mHelper.MailServer = "10.110.120.2";
                mHelper.Send(checkUser.F_Email, "OA请假单", "你好," + man.F_RealName + "有请假单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
            }
            return Success("操作成功。");
        }
        
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitSellOffForm(LeaveEntity leaveEntity, string keyValue)
        {
            LeaveEntity leave = leaveApp.GetForm(keyValue);
            if (leave.F_BeginTime > leaveEntity.F_ResumptionBeginTime)
            {
                return Error("销假开始时间小于假期开始时间！");
            }
            if (leave.F_EndTime<leaveEntity.F_ResumptionLeaveTime)//销假时间大于结束时间
            {
                return Error("销假结束时间大于假期结束时间！");
            }

            leave.F_LeaveStatus = (int)LeaveStatusEnum.UnCheckedSellOff;//销假申请
            leave.F_ResumptionBeginTime = leaveEntity.F_ResumptionBeginTime;
            leave.F_ResumptionLeaveTime = leaveEntity.F_ResumptionLeaveTime;
            leave.F_SellOffTimeLength_Day = leaveEntity.F_SellOffTimeLength_Day;
            leave.F_SellOffTimeLength_Hour = leaveEntity.F_SellOffTimeLength_Hour;
            leave.F_SellOffTimeLength_Minute = leaveEntity.F_SellOffTimeLength_Minute;
            leaveApp.SubmitForm(leave, keyValue);
            var checkUser = userApp.GetForm(leave.F_CheckLeaderId);
            var user = userApp.GetForm(leave.F_UserId);
            if (!string.IsNullOrEmpty(checkUser.F_Email))
            {
                mHelper.MailServer = "10.110.120.2";
                mHelper.Send(checkUser.F_Email, "OA销假单", "你好," + user.F_RealName + "有销假单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
            }
            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            string[] Ids = keyValue.Split(',');
            foreach (var key in Ids)
            {
                leaveApp.DeleteForm(key);
            }
            return Success("删除成功。");
        }

        /// <summary>
        /// http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <returns></returns>
        public bool HttpDownload(string url, string path)
        {
            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            System.IO.Directory.CreateDirectory(tempPath);  //创建临时文件目录
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; //临时文件
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            try
            {
                FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();
                //创建本地文件写入流
                //Stream stream = new FileStream(tempFile, FileMode.Create);
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();
                System.IO.File.Move(tempFile, path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary> 
        /// C#文件下载 
        /// </summary> 
        /// <param name="filename"></param> 
        //public void MyDownload(string filename)
        //{

        //    string path = Configs.GetValue("UploadFile") + filename;
        //    if (!)
        //    {
        //        Response.Write("对不起！文件不存在！！");
        //        return;
        //    }
        //    System.IO.FileInfo file = new System.IO.FileInfo(path);
        //    string fileFilt = "asp|aspx|php|jsp|ascx|config|asa|"; //不可下载的文件，务必要过滤干净 
        //    string fileExt = filename.Substring(filename.LastIndexOf("")).Trim().ToLower();
        //    if (fileFilt.IndexOf(fileExt) != -1)
        //    {
        //        Response.Write("对不起！该类文件禁止下载！！");
        //    }
        //    else
        //    {
        //        Response.Clear();
        //        Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename));
        //        Response.AddHeader("Content-Length", file.Length.ToString());
        //        Response.ContentType = GetContentType(HttpUtility.UrlEncode(fileExt));
        //        Response.WriteFile(fileFullName);
        //        Response.End();
        //    }
        //}

        /// <summary> 
        /// 获取下载类型 
        /// </summary> 
        /// <param name="fileExt"></param> 
        /// <returns></returns> 
        public string GetContentType(string fileExt)
        {
            string ContentType;
            switch (fileExt)
            {
                case "asf":
                    ContentType = "video/x-ms-asf"; break;
                case "avi":
                    ContentType = "video/avi"; break;
                case "doc":
                    ContentType = "application/msword"; break;
                case "zip":
                    ContentType = "application/zip"; break;
                case "xls":
                    ContentType = "application/vndms-excel"; break;
                case "gif":
                    ContentType = "image/gif"; break;
                case "jpg":
                    ContentType = "image/jpeg"; break;
                case "jpeg":
                    ContentType = "image/jpeg"; break;
                case "wav":
                    ContentType = "audio/wav"; break;
                case "mp3":
                    ContentType = "audio/mpeg3"; break;
                case "mpg":
                    ContentType = "video/mpeg"; break;
                case "mepg":
                    ContentType = "video/mpeg"; break;
                case "rtf":
                    ContentType = "application/rtf"; break;
                case "html":
                    ContentType = "text/html"; break;
                case "htm":
                    ContentType = "text/html"; break;
                case "txt":
                    ContentType = "text/plain"; break;
                default:
                    ContentType = "application/octet-stream";
                    break;
            }
            return ContentType;
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult  GetLocalPrinter()
        {
            PrintDocument print = new PrintDocument();
            List<string> fPrinters = new List<string>();
            fPrinters.Add(print.PrinterSettings.PrinterName);  //默认打印机出现在列表的第一项
            foreach (string fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            return Content(fPrinters.ToJson());
        }

        public ActionResult Print(string id)
        {
            LeaveEntity leave = leaveApp.GetForm(id);
            UserEntity user = userApp.GetForm(leave.F_UserId);
            OrganizeEntity organ = organizeApp.GetForm(user.F_DepartmentId);
            RoleEntity role = roleApp.GetForm(user.F_DutyId);
            UserLeaveEntity userLeave = new UserLeaveEntity(leave);
            userLeave.UserName = user.F_RealName;
            userLeave.Department = organ.F_FullName;
            userLeave.Position = role.F_FullName;
            userLeave.Account = user.F_Account;
            userLeave.StartTime = user.F_CreatorTime;
            return View(userLeave);
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult SellOffIndex()
        {
            return View();
        }

        public ActionResult SellOffForm()
        {
            return View();
        }
        

    }
}