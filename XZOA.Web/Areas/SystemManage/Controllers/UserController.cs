using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace XZOA.Web.Areas.SystemManage.Controllers
{
    [HandlerLogin]
    public class UserController : ControllerBase
    {
        private UserApp userApp = new UserApp();
        private UserLogOnApp userLogOnApp = new UserLogOnApp();
        private OrganizeApp OrganizeApp = new OrganizeApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword, string itemId)
        {
            List<string> list = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                list = OrganizeApp.getDepartByName(keyword);
            }
            var query = userApp.GetList(pagination, keyword,itemId, list);
            query = query.OrderByDescending(q => q.F_Id).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
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
        public ActionResult GetKeyGridJson(string keyword)
        {
            var query = new RoleApp().GetKeyGridJson(keyword);
            var list = userApp.GetUserList(query);
            return Content(list.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = userApp.GetForm(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetForm(string keyValue)
        {
            var flag = false;
            var query = new RoleApp().GetKeyGridJson("工程师");
            var data = userApp.GetForm(keyValue);
            if(query.Contains(data.F_RoleId))
            {
                flag = true;
            }
            return Content(new { data = data, flag = flag }.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(UserEntity userEntity, UserLogOnEntity userLogOnEntity, string keyValue)
        {
            int Code=userApp.SubmitForm(userEntity, userLogOnEntity, keyValue);
            if (Code == 200)
            {
                return Success("创建成功！");
            }
            else if (Code ==100)
            {
                return Error("账户已存在！");
            }
            else {
                return Error("创建失败！");
            }
        }

        public ActionResult ChangePwd(string oldPwd,string newPwd)
        {
            var userId=OperatorProvider.Provider.GetCurrent().UserId;
            if (userApp.EqualOldPwd(oldPwd, userId))
            {
                if(userApp.ChangePwd(newPwd,userId))
                {
                    return Success("修改成功！");
                }
                return Error("修改失败！");
            }
            else {
                return Error("旧密码错误！");
            }
        }

        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            userApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpGet]
        public ActionResult RevisePassword()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRevisePassword(string userPassword, string keyValue)
        {
            userLogOnApp.RevisePassword(userPassword, keyValue);
            return Success("重置密码成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DisabledAccount(string keyValue)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.F_Id = keyValue;
            userEntity.F_EnabledMark = false;
            userApp.UpdateForm(userEntity);
            return Success("账户禁用成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult EnabledAccount(string keyValue)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.F_Id = keyValue;
            userEntity.F_EnabledMark = true;
            userApp.UpdateForm(userEntity);
            return Success("账户启用成功。");
        }

        [HttpGet]
        public ActionResult ChangePwd()
        {
            return View();
        }
    }
}
