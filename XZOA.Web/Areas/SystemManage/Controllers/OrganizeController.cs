using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XZOA.Web.Areas.SystemManage.Controllers
{
    [HandlerLogin]
    public class OrganizeController : ControllerBase
    {
        private OrganizeApp organizeApp = new OrganizeApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var data = organizeApp.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (OrganizeEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_FullName;
                treeModel.parentId = item.F_ParentId;
                treeModel.data = item;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeJson()
        {
            var data = organizeApp.GetList();
            var treeList = new List<TreeViewModel>();
            foreach (OrganizeEntity item in data)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = data.Count(t => t.F_ParentId == item.F_Id) == 0 ? false : true;
                tree.id = item.F_Id;
                tree.text = item.F_FullName;
                tree.value = item.F_EnCode;
                tree.parentId = item.F_ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeGridJson(string keyword)
        {
            var data = organizeApp.GetList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_FullName.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (OrganizeEntity item in data)
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
        public ActionResult GetList()
        {
            var data = organizeApp.GetDepartList();
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = organizeApp.GetForm(keyValue);
            if(!string.IsNullOrEmpty(data.F_ManagerId))
            {
                var user = new UserApp().GetForm(data.F_ManagerId);
                data.F_ManagerId = user.F_RealName;
            }
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(OrganizeEntity organizeEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var organ = organizeApp.GetForm(keyValue);
                organ.F_ParentId = organizeEntity.F_ParentId;
                organ.F_CategoryId = organizeEntity.F_CategoryId;
                organ.F_FullName = organizeEntity.F_FullName;
                organ.F_EnCode = organizeEntity.F_EnCode;
                organ.F_ManagerId = organizeEntity.F_ManagerId;
                organ.F_MobilePhone = organizeEntity.F_MobilePhone;
                organ.F_WeChat = organizeEntity.F_WeChat;
                organ.F_TelePhone = organizeEntity.F_TelePhone;
                organ.F_Email = organizeEntity.F_Email;
                organ.F_Fax = organizeEntity.F_Fax;
                organ.F_Address = organizeEntity.F_Address;
                organ.F_DepartGroupId = organizeEntity.F_DepartGroupId;
                organ.F_EnabledMark = organizeEntity.F_EnabledMark;
                organ.F_Description = organizeEntity.F_Description;
                organizeApp.SubmitForm(organ, keyValue);
            }
            else {
                organizeApp.SubmitForm(organizeEntity, keyValue);
            }
            
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            organizeApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
