using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Code;
using System.IO;

namespace XZOA.Web.Areas.PurchaseManage.Controllers
{
    [HandlerLogin]
    public class ApplyController : ControllerBase
    {
        ApplyBillApp applyBillApp = new ApplyBillApp();
        UserApp userApp = new UserApp();
        OrganizeApp organizeApp = new OrganizeApp();
        DutyApp dutyApp = new DutyApp();
        MailHelper mHelper = new MailHelper();
        AppointManApp appManApp = new AppointManApp();
        BillApp billApp = new BillApp();
        PriceApp priceApp = new PriceApp();
        TypeApp typeApp = new TypeApp();
        public ActionResult ApplyHistoryIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ApplyConvertPurchaseForm()
        {
            return View();
        }

        /// <summary>
        /// 获取本部门的申购列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="keyword"></param>
        /// <param name="dateBeginTime"></param>
        /// <param name="dateEndTime"></param>
        /// <param name="hopeDDBeginTime"></param>
        /// <param name="hopeDDEndTime"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetApplyList(Pagination pagination,string keyword, DateTime? dateBeginTime, DateTime? dateEndTime, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
            var departName = organizeApp.GetForm(departId).F_FullName;
            var list = applyBillApp.GetApplyList(pagination, departName, keyword, dateBeginTime, dateEndTime, hopeDDBeginTime, hopeDDEndTime);
            var data = new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetApplyHistoryList(Pagination pagination, string keyword, DateTime? dateBeginTime, DateTime? dateEndTime, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            var list = applyBillApp.GetApplyHistoryList(pagination, keyword, dateBeginTime, dateEndTime);
            var data = new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }


        /// <summary>
        /// 退单
        /// </summary>
        /// <param name="applyBillEntity"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult BackApply(ApplyBillEntity applyBillEntity,int? keyValue)
        {
            if(keyValue!=null)
            {
                var apply = applyBillApp.GetForm(keyValue.Value);
                apply.backReason = applyBillEntity.backReason;
                apply.appAuthDate = null;
                apply.appAuthIdea = null;
                apply.appAuthMan = null;
                apply.appAuthTag = "M";
                apply.appExaDate = null;
                apply.appExaIdea = null;
                apply.appExaMan = null;
                apply.appExaTag = "M";
                applyBillApp.SubmitForm(apply,keyValue);
                return Success("操作成功。");
            }
            return Error("操作失败。");

        }

        /// <summary>
        /// 获取初审列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetApplyFirstCheckList(Pagination pagination)
        {
            var UserId = OperatorProvider.Provider.GetCurrent().UserId;
            var applys = applyBillApp.GetApplyFirstCheckList();
            var users = userApp.GetUserList();
            var query = from a in applys
                        join u in users
                        on a.appMan equals u.F_RealName
                        where u.F_ManagerId == UserId
                        select a;
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.date).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
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
        public ActionResult GetApplyCheckList(Pagination pagination)
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = applyBillApp.GetApplyCheckList(userName, pagination);
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
        public ActionResult GetApplyApprovalList(Pagination pagination)
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = applyBillApp.GetApplyApprovalList(userName,pagination);
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
        public ActionResult GetApplyResultList(Pagination pagination,string keyword,string purIsTem,int? purWay,string status,int? TypeID)
       {
            var list = applyBillApp.GetApplyResultList(pagination,keyword,purIsTem,purWay, status,TypeID);
            var data = new
            {
                rows = list,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        /// <summary>
        /// 获取审核领导
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
            var checkRoleIds = dutyApp.GetApplyCheckList();
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
            else
            {//审核领导为空则加入角色为厂长的领导
                var Ids = dutyApp.GetApprovalList();
                var checkList = userApp.GetUserCheckList(groupDepart, Ids);
                foreach (var check in checkList)
                {
                    if (check != null)
                    {
                        TreeSelectModel treeModel = new TreeSelectModel();
                        treeModel.id = check.F_RealName;
                        treeModel.text = check.F_RealName;
                        treeList.Add(treeModel);
                    }
                }
            }
            treeList = treeList.ToList();
            treeList.Reverse();
            return Content(treeList.ToJson());
        }

        /// <summary>
        /// 获取种类类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTypeJson()
        {
            var treeList = new List<TreeSelectModel>();
            var data = typeApp.GetList();
            foreach (var t in data)
            {
                if (t != null)
                {
                    TreeSelectModel treeModel = new TreeSelectModel();
                    treeModel.id = t.ID.ToString();
                    treeModel.text = t.TypeName;
                    treeList.Add(treeModel);
                }
            }
            return Content(treeList.ToJson());
        }


        /// <summary>
        /// 获取批准领导
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJsonApproval()
        {
            var treeList = new List<TreeSelectModel>();

            var data = appManApp.GetList();
            if (data != null)
            {
                foreach (var user in data)
                {
                    if (user != null)
                    {
                        TreeSelectModel treeModel = new TreeSelectModel();
                        treeModel.id = user.man;
                        treeModel.text = user.man;
                        treeList.Add(treeModel);
                    }
                }
            }
            return Content(treeList.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(int keyValue)
        {
            ApplyBillEntity applyBillEntity = applyBillApp.GetForm(keyValue);
             return Content(applyBillEntity.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormDetail(int keyValue)
        {
            ApplyBillEntity applyBillEntity = applyBillApp.GetForm(keyValue);
            if (!string.IsNullOrEmpty(applyBillEntity.priNO))
            {
                PriceEntity price = priceApp.GetFormJson(applyBillEntity.priNO);
              
                return Json(new {
                    prdName =price.prdName,
                    spc =price.spc,
                    mat =applyBillEntity.mat,
                    orderNo =price.orderNo,
                    appUnit =applyBillEntity.appUnit,
                    viceNum = applyBillEntity.viceNum
                },JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content(applyBillEntity.ToJson());
            } 

        }
        /// <summary>
        /// 申购单 修改、增加
        /// </summary>
        /// <param name="applyBillEntity"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ApplyBillEntity applyBillEntity,int? keyValue)
        {
            try
            {
                var userId = OperatorProvider.Provider.GetCurrent().UserId;
                var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
                var role = new RoleApp().GetForm(roleId);
                var appMan = userApp.GetForm(userId);
                UserEntity firstExaMan = null;
                if (applyBillEntity != null)
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
                            applyBillEntity.annex = fileName;
                        }

                    }
                    if (keyValue != null)//修改
                    {
                        var apply = applyBillApp.GetForm(keyValue.Value);
                        var userName = OperatorProvider.Provider.GetCurrent().UserName;
                        if (!apply.appMan.Equals(userName))
                        {
                            return Error("非本人不能修改申购单。");
                        }
                        if (apply.appExaTag == "T")
                        {
                            return Error("该申购单已审核，不能修改。");
                        }
                        if (apply.appExaTag == "F" || apply.appExaMan == null)
                        {
                            apply.appExaTag = "M";
                        }

                        apply.appExaDate = null;
                        apply.appExaIdea = null;
                        apply.backReason = null;
                        apply.prdName = applyBillEntity.prdName;
                        apply.spc = applyBillEntity.spc;
                        apply.rem = applyBillEntity.rem;
                        apply.useGroup = applyBillEntity.useGroup;
                        apply.viceNum = applyBillEntity.viceNum;
                        apply.annex = applyBillEntity.annex;
                        apply.TypeID = applyBillEntity.TypeID;
                        if(!string.IsNullOrEmpty(applyBillEntity.viceUnit))
                        {
                            apply.viceUnit = applyBillEntity.viceUnit.Trim().ToUpper();
                        }
                        apply.appNum = applyBillEntity.appNum;
                        apply.appUnit = applyBillEntity.appUnit.Trim().ToUpper();
                        apply.purIsTem = applyBillEntity.purIsTem;
                        apply.WAREWAY = applyBillEntity.WAREWAY;
                        apply.hopeDD = applyBillEntity.hopeDD;
                        apply.mat = applyBillEntity.mat;
                        apply.orderNo = applyBillEntity.orderNo;
                        if (apply.appExaMan != applyBillEntity.appExaMan)
                        {
                            apply.appExaMan = applyBillEntity.appExaMan;
                            var appExaMan = userApp.GetFormByName(applyBillEntity.appExaMan);
                            if (!string.IsNullOrEmpty(appExaMan.F_Email))
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(appExaMan.F_Email, "申购单审核", "你好," + appMan.F_RealName + "有申购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                            }
                        }
                        apply.appAuthMan = applyBillEntity.appAuthMan;
                        applyBillApp.SubmitForm(apply, keyValue);
                    }
                    else
                    {
                        applyBillEntity.appUnit = applyBillEntity.appUnit.Trim().ToUpper();
                        if (!string.IsNullOrEmpty(applyBillEntity.viceUnit))
                        {
                            applyBillEntity.viceUnit = applyBillEntity.viceUnit.Trim().ToUpper();
                        }
                        applyBillEntity.appAuthTag = "D";
                        if (role != null)
                        {
                            if (role.F_FullName.Equals("工程师"))
                            {
                                applyBillEntity.appExaTag = "A";
                                firstExaMan = userApp.GetForm(appMan.F_ManagerId);
                                applyBillEntity.FirstExaMan = firstExaMan.F_RealName;
                            }
                            else {
                                applyBillEntity.appExaTag = "D";
                            }
                        }
                        
                        applyBillEntity.purAuthTag = "D";
                        applyBillEntity.purExaTag = "D";
                        applyBillEntity.prtTag = "F";
                        applyBillEntity.caseTag = "F";
                        applyBillEntity.isPreMoney = "N";
                        if (applyBillEntity.purIsTem == null)
                        {
                            applyBillEntity.purIsTem = "F";
                        }
                        var appExaMan = userApp.GetFormByName(applyBillEntity.appExaMan);
                        var appManDepart = organizeApp.GetForm(appMan.F_DepartmentId);
                        applyBillEntity.appMan = appMan.F_RealName;
                        applyBillEntity.appDep = appManDepart.F_FullName;
                        applyBillEntity.date = DateTime.Now;
                        applyBillApp.SubmitForm(applyBillEntity, keyValue);
                        if (role!=null&&!role.F_FullName.Equals("工程师"))
                        {
                            if (!string.IsNullOrEmpty(appExaMan.F_Email))
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(appExaMan.F_Email, "申购单审核", "你好," + appMan.F_RealName + "有申购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                            }
                        }
                        else {
                            if (!string.IsNullOrEmpty(firstExaMan.F_Email))
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(firstExaMan.F_Email, "申购单初审", "你好," + firstExaMan.F_RealName + "有申购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                            }
                        }
                    }
                    return Success("操作成功。");
                }
            }
            catch (Exception ex)
            {
                new ErrorLogApp().SubmitForm(ex);
                return Error("操作失败。");
            }
            return Success("操作成功。");
        }

        /// <summary>
        /// 申购单 修改、增加
        /// </summary>
        /// <param name="applyBillEntity"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitAuthorizeForm(int man)
        {
            try
            {
                AppointManApp appointMan=new AppointManApp();
                var authorize= appointMan.GetForm();
                var queryList = applyBillApp.GetPointList(authorize.man);
                var pointMan = appointMan.SubmitForm(man);
                foreach (var item in queryList)
                {
                    if (item.appExaMan.Equals(authorize.man))
                    {
                        item.appExaMan = pointMan.man;
                    }
                    else if (item.appAuthMan.Equals(authorize.man))
                    {
                        item.appAuthMan = pointMan.man;
                    }
                    else if (item.purExaMan.Equals(authorize.man))
                    {
                        item.purExaMan = pointMan.man;
                    }
                    else
                    {
                        item.purAuthMan = pointMan.man;
                    }
                    applyBillApp.SubmitForm(item,item.ID);
                }
                return Success("操作成功。");
            }
            catch (Exception)
            {

                return Error("操作失败。");
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAuthorize()
        {
            var man = new AppointManApp().GetForm();
            return Content(man.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetAuthorizeList()
        {
            var list = new AppointManApp().GetALLList();
            return Content(list.ToJson());
        }

        /// <summary>
        /// 项目经理初审
        /// </summary>
        /// <param name="applyBillEntity"></param>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitFirstCheckApplyForm(ApplyBillEntity applyBillEntity, string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                List<string> exaList = new List<string>();
                List<string> appList = new List<string>();
                foreach (var Id in Ids)
                {
                    int ID = Convert.ToInt32(Id);
                    var apply = applyBillApp.GetForm(ID);
                    var TimeSpan = DateTime.Now - apply.date;
                    apply.appExaDate = DateTime.Now;
                    apply.appExaIdea = applyBillEntity.appExaIdea;
                    if (applyBillEntity.appExaTag == "T")//初审通过
                    {
                        apply.appExaTag = "D";
                        if (!exaList.Contains(apply.appExaMan))
                        {
                            exaList.Add(apply.appExaMan);
                        }
                    }
                    else
                    {
                        apply.appExaTag = "B";
                        if (!appList.Contains(apply.appMan))
                        {
                            appList.Add(apply.appMan);
                        }
                    }
                    applyBillApp.SubmitForm(apply, ID);
                    
                }
                if (applyBillEntity.appExaTag == "T")
                {
                    foreach (var item in exaList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "申购单审核", "你好,有申购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }

                }
                else
                {
                    foreach (var item in appList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "申购单初审不通过", "你好,你的申购单初审不通过,详情查看请点击链接<a>http://10.110.120.6:8090/</a>");
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
        [ValidateAntiForgeryToken] 
        public ActionResult SubmitCheckApplyForm(ApplyBillEntity applyBillEntity, string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                List<string> authList = new List<string>();
                List<string> appList = new List<string>();
                foreach (var Id in Ids)
                {
                    int ID = Convert.ToInt32(Id);
                    var apply = applyBillApp.GetForm(ID);
                    var TimeSpan = DateTime.Now - apply.date;
                    apply.appExaDate = DateTime.Now;
                    apply.appExaIdea = applyBillEntity.appExaIdea;
                    if (applyBillEntity.appExaTag == "T")//审核通过
                    {
                        apply.appExaTag = applyBillEntity.appExaTag;
                        if (apply.appExaMan == apply.appAuthMan)
                        {
                             apply.appAuthTag = "T";
                            applyBillApp.SubmitForm(apply, ID);
                        }
                        else { applyBillApp.SubmitForm(apply, ID); }
                        if(!authList.Contains(apply.appAuthMan))
                        {
                            authList.Add(apply.appAuthMan);
                        }
                        if (!appList.Contains(apply.appMan))
                        {
                            appList.Add(apply.appMan);
                        }
                    }
                    else
                    {
                        apply.appExaTag = applyBillEntity.appExaTag;
                        applyBillApp.SubmitForm(apply, ID);
                        if (!appList.Contains(apply.appMan))
                        {
                            appList.Add(apply.appMan);
                        }
                    }
                    
                }
                if (applyBillEntity.appExaTag == "T")
                {
                    foreach (var item in authList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "申购单审批", "你好,有申购单需要你登录OA去做审批,请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }
                    
                }
                else {
                    foreach (var item in appList)
                    {
                        var user = userApp.GetFormByName(item);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "申购单审核不通过", "你好,你的申购单审核不通过,详情查看请点击链接<a>http://10.110.120.6:8090/</a>");
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
        public ActionResult PickingForm(string keyValue)
        {
            try
            {
                var dep = OperatorProvider.Provider.GetCurrent().DepartmentId;
                var depName = organizeApp.GetForm(dep).F_FullName;
                string[] Ids = keyValue.Split(',');
                if (Ids.Length == 1)
                {
                    int ID = Convert.ToInt32(Ids[0]);
                    var apply = applyBillApp.GetForm(ID);
                    if (((apply.yiJiaoNum ?? 0) - (apply.takeNum ?? 0)) <= 0)
                    {
                        return Error("暂时不能领料。");
                    }
                }
                foreach (var Id in Ids)
                {
                    int ID = Convert.ToInt32(Id);
                    var apply = applyBillApp.GetForm(ID);
                    if (((apply.yiJiaoNum ?? 0) - (apply.takeNum ?? 0)) > 0)
                    {
                        BillEntity billEntity = new BillEntity();
                        billEntity.billType = "D";
                        billEntity.num = (apply.yiJiaoNum ?? 0) - (apply.takeNum ?? 0);
                        billEntity.prtTag = "F";
                        billEntity.purSup = apply.purSup;
                        var price = priceApp.GetFormJson(apply.priNO);
                        billEntity.billNo = billApp.ProduceBillNO(apply.purSup, "pick", price.currency.Value,depName);
                        billEntity.caseTag = "F";
                        billEntity.chkDate = DateTime.Now;
                        billEntity.fromID = apply.ID;
                        billEntity.viceNum = billApp.GetViceNum(billEntity.fromID);
                        billApp.SubmitForm(billEntity);
                        apply.takeNum = (apply.takeNum ?? 0) + billEntity.num;
                        if (apply.takeNum == apply.appNum)
                        {
                            apply.caseTag = "T";
                        }
                        applyBillApp.SubmitForm(apply, apply.ID);
                    }
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
        [ValidateAntiForgeryToken] 
        public ActionResult SubmitApprolvalApplyForm(ApplyBillEntity applyBillEntity, string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                foreach (var Id in Ids)
                {
                    int ID = Convert.ToInt32(Id);
                    var apply = applyBillApp.GetForm(ID);
                    apply.appAuthDate = DateTime.Now;
                    apply.appAuthIdea = applyBillEntity.appAuthIdea;
                    apply.appAuthTag = applyBillEntity.appAuthTag;
                    if (apply.appAuthTag == "F")//不批准
                    {
                        apply.caseTag = "T";
                        var user = userApp.GetFormByName(apply.appMan);
                        if (!string.IsNullOrEmpty(user.F_Email))
                        {
                            mHelper.MailServer = "10.110.120.2";
                            mHelper.Send(user.F_Email, "申购单批准不通过", "你好,你的申购单批准不通过,详情查看请点击链接<a>http://10.110.120.6:8090/</a>");
                        }
                    }
                    applyBillApp.SubmitForm(apply, ID);
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
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var num = 0;
                foreach (var id in Ids)
                {
                    var key = Convert.ToInt32(id);
                    var apply = applyBillApp.GetForm(key);
                    if (string.IsNullOrEmpty(apply.purNo)&&userName.Equals(apply.appMan))
                    {
                        applyBillApp.DeleteForm(key);
                        num++;
                    }
                }
                return Success("已删除"+num+"条记录。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
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
                var appMan = OperatorProvider.Provider.GetCurrent().UserName;
                var departId = OperatorProvider.Provider.GetCurrent().DepartmentId;
                var depart = organizeApp.GetForm(departId);
                var groupDepart = organizeApp.getDepartByGroup(depart.F_DepartGroupId);
                groupDepart.Add(departId);
                var checkRoleIds = dutyApp.GetApplyCheckList();
                var appDep = organizeApp.GetForm(departId).F_FullName;
                var data = userApp.GetUserCheckList(groupDepart, checkRoleIds);
                data.Reverse();
                var da = data.First();
                var appExaMan = da.F_RealName;
                var appAuthMan = appManApp.GetForm().man;
                if (applyBillApp.ImportExcel(path, System.Configuration.ConfigurationManager.ConnectionStrings["XZOADbContext"].ConnectionString, appMan, appDep, appExaMan, appAuthMan))
                {
                    if(!string.IsNullOrEmpty(da.F_Email))
                    {
                        mHelper.MailServer = "10.110.120.2";
                        mHelper.Send(da.F_Email, "申购单审核", "你好," + appMan + "有申购单需要你登录OA去做审核,请点击链接<a>http://10.110.120.6:8090/</a>");
                    }
                    return Success("操作成功");
                }
                   
                

                return Error("导入文件格式错误！");

            }
            catch (Exception ex)
            {
             //   new ErrorLogApp().SubmitForm(ex);
                return Error(ex.Message);
            }
        }

        
        /// <summary>
        /// 申购单审核页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult CheckApplyIndex()
        {
            return View();
        }
        /// <summary>
        /// 审核弹框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CheckApplyForm()
        {
            return View();
        }

        /// <summary>
        /// 申购批准页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ApprovalApplyIndex()
        {
            return View();
        }

        /// <summary>
        /// 申购审批弹框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ApprovalApplyForm()
        {
            return View();
        }
        
        /// <summary>
        /// 申购单转采购单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ApplyToPurchaseIndex()
        {
            return View();
        }

        /// <summary>
        /// 转单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize]
        public ActionResult ApplyConvertFrom()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult Authorize()
        {
            return View();
        }

        [HttpGet]
        //[HandlerAuthorize]
        public ActionResult BackApply()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult FirstCheckApplyIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult FirstCheckApplyForm()
        {
            return View();
        }
    }
}