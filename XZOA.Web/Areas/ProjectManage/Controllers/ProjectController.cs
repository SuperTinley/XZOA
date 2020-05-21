using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult BuyForm()
        {
            return View();
        }

        public ActionResult GetList(Pagination pagination)
        {
            var query = projectApp.GetList(pagination);
            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
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
            else
            {
                projectApp.SubmitForm(projectEntity);
            }

            return Success("操作成功。");
        }

        
    }
}