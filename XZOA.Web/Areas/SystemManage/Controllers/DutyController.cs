using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XZOA.Web.Areas.SystemManage.Controllers
{
    [HandlerLogin]
    public class DutyController : ControllerBase
    {
        private DutyApp dutyApp = new DutyApp();


        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var query = dutyApp.GetList(keyword);
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridList(Pagination pagination, string keyword)
        {
            var query = dutyApp.GetPaginationList(pagination,keyword);
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
        public ActionResult GetFormJson(string keyValue)
        {
            var data = dutyApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(RoleEntity roleEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var organ = dutyApp.GetForm(keyValue);
                organ.F_OrganizeId = roleEntity.F_OrganizeId;
                organ.F_FullName = roleEntity.F_FullName;
                organ.F_FullName = roleEntity.F_FullName;
                organ.F_EnCode = roleEntity.F_EnCode;
                organ.F_SortCode = roleEntity.F_SortCode;
                organ.F_AllowEdit = roleEntity.F_AllowEdit;
                organ.F_AllowDelete = roleEntity.F_AllowDelete;
                organ.F_EnabledMark = roleEntity.F_EnabledMark;
                organ.F_Description = roleEntity.F_Description;
                dutyApp.SubmitForm(organ, keyValue);
            }
            else {
                dutyApp.SubmitForm(roleEntity, keyValue);
            }
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            dutyApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
