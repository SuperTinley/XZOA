using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.Enum;
using XZOA.Web;

namespace XZOA.Web.Areas.LeaveManage.Controllers
{
    [HandlerLogin]
    public class CheckController : ControllerBase
    {
        LeaveApp leaveApp = new LeaveApp();
        UserApp userApp = new UserApp();
        OrganizeApp organizeApp = new OrganizeApp();
        RoleApp roleApp = new RoleApp();
        MailHelper mHelper = new MailHelper();

        // GET: LeaveManage/Check
        public ActionResult CheckIndex()
        {
            return View();
        }
        [HttpGet]
        public ActionResult CheckForm()
        {
            return View();
        }

        public ActionResult GetCheckList(Pagination pagination)
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where (l.F_CheckLeaderId == UserId&& l.F_LeaveStatus==(int)LeaveStatusEnum.UnChecked) || (l.F_ApprovalLeaderId == UserId && l.F_LeaveStatus==(int)LeaveStatusEnum.UnApproved) 
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
                         });
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.F_CreateTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        
        public ActionResult GetSellOffCheckList(Pagination pagination)
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var leave = leaveApp.GetList();
            var user = userApp.GetUserList();
            var query = (from l in leave
                         join u in user
                         on l.F_UserId equals u.F_Id
                         where (l.F_CheckLeaderId == UserId && l.F_LeaveStatus == (int)LeaveStatusEnum.UnCheckedSellOff)
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
                             F_SellOffTimeLength_Day = l.F_SellOffTimeLength_Day,
                             F_SellOffTimeLength_Hour = l.F_SellOffTimeLength_Hour,
                             F_SellOffTimeLength_Minute = l.F_SellOffTimeLength_Minute,
                             F_ResumptionBeginTime = l.F_ResumptionBeginTime,
                             F_ResumptionLeaveTime = l.F_ResumptionLeaveTime
                         });
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.F_CreateTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        
        public ActionResult SellOffForm()
        {
            return View();
        }

        public ActionResult UnderLineCheckIndex()
        {
            return View();
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(LeaveEntity leaveEntity, string keyValue)
        {
            string[] Ids = keyValue.Split(',');
            foreach (var Id in Ids)
            {
                var leave = leaveApp.GetForm(Id);
                if (leave.F_IsCheck == null && leave.F_LeaveStatus == (int)LeaveStatusEnum.UnChecked)//未审核
                {
                    leave.F_IsCheck = leaveEntity.F_IsCheck != null ? leaveEntity.F_IsCheck : null;
                    leave.F_CheckOpinion = leaveEntity.F_CheckOpinion != "" ? leaveEntity.F_CheckOpinion : "";
                    if (leave.F_IsCheck.HasValue && leave.F_IsCheck == true)
                    {

                        leave.F_LeaveStatus = (int)LeaveStatusEnum.UnApproved;//状态改为未批准
                        if (leave.F_CheckLeaderId.Equals(leave.F_ApprovalLeaderId))//审核人和批准人一样
                        {
                            leave.F_IsApproval = true;
                            leave.F_LeaveStatus = (int)LeaveStatusEnum.Success;//状态改为审批通过
                            leave.F_ApprovalTime = DateTime.Now;
                        }
                        else {
                            var ApprovalLeader = userApp.GetForm(leave.F_ApprovalLeaderId);
                            var user = userApp.GetForm(leave.F_UserId);
                            if (!string.IsNullOrEmpty(ApprovalLeader.F_Email))
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(ApprovalLeader.F_Email, "OA请假单", "你好," + user.F_RealName + "有请假单需要你登录OA去做批准,请点击链接<a>http://10.110.120.6:8090/</a>");
                            }
                        }

                        
                    }
                    else
                    {
                        leave.F_LeaveStatus = (int)LeaveStatusEnum.Fail;//状态改为审核失败
                        leave.F_ApprovalTime = DateTime.Now;
                    }
                }
                else {
                    if (leave.F_IsApproval == null && leave.F_LeaveStatus == (int)LeaveStatusEnum.UnApproved)//未批准
                    {
                        leave.F_IsApproval = leaveEntity.F_IsApproval != null ? leaveEntity.F_IsApproval : null;
                        leave.F_ApprovalOpinion = leaveEntity.F_ApprovalOpinion != "" ? leaveEntity.F_ApprovalOpinion : "";
                        if (leave.F_IsApproval.HasValue && leave.F_IsApproval == true)
                        {
                            leave.F_LeaveStatus = (int)LeaveStatusEnum.Success;//状态改为审核通过
                            leave.F_ApprovalTime = DateTime.Now;
                        }
                        else
                        {
                            leave.F_LeaveStatus = (int)LeaveStatusEnum.Fail;//状态改为审核通过
                            leave.F_ApprovalTime = DateTime.Now;
                        }
                    }
                }
                
                leaveApp.SubmitForm(leave, Id);
            }
            return Success("操作成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SellOffSubmitForm(LeaveEntity leaveEntity, string keyValue)
        {
            try
            {
                var leave = leaveApp.GetForm(keyValue);
                if (leave.F_IsSellOff == null && leave.F_LeaveStatus == (int)LeaveStatusEnum.UnCheckedSellOff)//未审核
                {
                    leave.F_IsSellOff = leaveEntity.F_IsSellOff != null ? leaveEntity.F_IsSellOff : null;
                    leave.F_SellOffCheckOpinion = leaveEntity.F_SellOffCheckOpinion != "" ? leaveEntity.F_SellOffCheckOpinion : "";
                    if (leave.F_IsSellOff.HasValue && leave.F_IsSellOff == true)
                    {
                        leave.F_LeaveStatus = (int)LeaveStatusEnum.SuccessSellOff;//状态改为销假成功
                        leave.F_IsSellOff = true;//销假成功
                        leave.F_TimeLength_Day = leave.F_SellOffTimeLength_Day != null ? leave.F_SellOffTimeLength_Day.Value : leave.F_TimeLength_Day;
                        leave.F_TimeLength_Hour = leave.F_SellOffTimeLength_Hour != null ? leave.F_SellOffTimeLength_Hour.Value : leave.F_TimeLength_Hour;
                        leave.F_TimeLength_Minute = leave.F_SellOffTimeLength_Minute != null ? leave.F_SellOffTimeLength_Minute.Value : leave.F_TimeLength_Minute;
                        leave.F_EndTime = leave.F_ResumptionLeaveTime.Value;//将结束时间调整为销假时间
                        leave.F_BeginTime = leave.F_ResumptionBeginTime??leave.F_BeginTime;
                    }
                    else
                    {
                        leave.F_LeaveStatus = (int)LeaveStatusEnum.FailSellOff;//状态改为销假失败
                        leave.F_IsSellOff = false;//销假失败
                    }
                }
                //if (leave.F_SellOffIsApproval == null && leave.F_LeaveStatus == (int)LeaveStatusEnum.UnApprovedSellOff)//未批准
                //{
                //    leave.F_SellOffIsApproval = leaveEntity.F_SellOffIsApproval != null ? leaveEntity.F_SellOffIsApproval : null;
                //    leave.F_SellOffApprovalOpinion = leaveEntity.F_SellOffApprovalOpinion != "" ? leaveEntity.F_SellOffApprovalOpinion : "";
                //    if (leave.F_SellOffIsApproval.HasValue && leave.F_SellOffIsApproval == true)
                //    {
                //        leave.F_LeaveStatus = (int)LeaveStatusEnum.SuccessSellOff;//状态改为销假成功
                //        leave.F_ResumptionLeaveTime = leaveEntity.F_ResumptionLeaveTime;//加入销假时间
                //    }
                //    else
                //    {
                //        leave.F_LeaveStatus = (int)LeaveStatusEnum.FailSellOff;//状态改为销假失败
                //        leave.F_ResumptionLeaveTime = null;
                //    }
                //}
                leaveApp.SubmitForm(leave, keyValue);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
                return null;
            }
        }
        
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult UnderLineCheck(string keyValue)
        {
            var leave = leaveApp.GetForm(keyValue);
            leave.F_IsOffLine = true;
            leave.F_LeaveStatus = (int)LeaveStatusEnum.UnderLine;
            leaveApp.SubmitForm(leave, keyValue);
            return Success("操作成功。");
        }

        [HttpGet]
        public ActionResult SellOffCheckIndex()
        {
            return View();
        }

    }
}