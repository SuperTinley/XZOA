using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XZOA.Web.Areas.SystemManage.Controllers
{
    [HandlerLogin]
    public class RoleController : ControllerBase
    {
        private RoleApp roleApp = new RoleApp();
        private RoleAuthorizeApp roleAuthorizeApp = new RoleAuthorizeApp();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string keyword)
        {
            var query = roleApp.GetList(keyword);
            return Content(query.ToJson());
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridList(Pagination pagination,string keyword)
        {
            var query = roleApp.GetPaginationList(pagination,keyword);
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
            var data = roleApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(RoleEntity roleEntity, string permissionIds, string keyValue)
        {

            if (!string.IsNullOrEmpty(keyValue))
            {
                var role = roleApp.GetForm(keyValue);
                role.F_OrganizeId = roleEntity.F_OrganizeId;
                role.F_FullName = roleEntity.F_FullName;
                role.F_EnCode = roleEntity.F_EnCode;
                role.F_Type = roleEntity.F_Type;
                role.F_SortCode = roleEntity.F_SortCode;
                role.F_EnabledMark = roleEntity.F_EnabledMark;
                role.F_AllowEdit = roleEntity.F_AllowEdit;
                role.F_AllowDelete = roleEntity.F_AllowDelete;
                role.F_Description = roleEntity.F_Description;
                roleApp.SubmitForm(role, permissionIds.Split(','), keyValue);
            }
            else {
                roleApp.SubmitForm(roleEntity, permissionIds.Split(','), keyValue);
            }
            
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            roleApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
