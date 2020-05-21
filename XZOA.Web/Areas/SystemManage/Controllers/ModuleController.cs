using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XZOA.Web.Areas.SystemManage.Controllers
{
    [HandlerLogin]
    public class ModuleController : ControllerBase
    {
        private ModuleApp moduleApp = new ModuleApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var data = moduleApp.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (ModuleEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_FullName;
                treeModel.parentId = item.F_ParentId;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeGridJson(string keyword)
        {
            var data = moduleApp.GetList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_FullName.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (ModuleEntity item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                bool hasChildren = data.Count(t => t.F_ParentId == item.F_Id) == 0 ? false : true;
                treeModel.id = item.F_Id;
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.F_ParentId;
                treeModel.expanded = hasChildren;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = moduleApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ModuleEntity moduleEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var role = moduleApp.GetForm(keyValue);
                role.F_ParentId = moduleEntity.F_ParentId;
                role.F_FullName = moduleEntity.F_FullName;
                role.F_EnCode = moduleEntity.F_EnCode;
                role.F_UrlAddress = moduleEntity.F_UrlAddress;
                role.F_SortCode = moduleEntity.F_SortCode;
                role.F_EnabledMark = moduleEntity.F_EnabledMark;
                role.F_AllowEdit = moduleEntity.F_AllowEdit;
                role.F_AllowDelete = moduleEntity.F_AllowDelete;
                role.F_Description = moduleEntity.F_Description;
                role.F_IsMenu = moduleEntity.F_IsMenu;
                role.F_IsPublic = moduleEntity.F_IsPublic;
                role.F_IsExpand = moduleEntity.F_IsExpand;
                role.F_Target = moduleEntity.F_Target;
                role.F_Icon = moduleEntity.F_Icon;
                moduleApp.SubmitForm(role, keyValue);
            }
            else {
                moduleEntity.F_DeleteMark = false;
                moduleApp.SubmitForm(moduleEntity, keyValue);
            }
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            moduleApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
