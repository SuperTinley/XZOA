/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System;

namespace XZOA.Web.Controllers
{
    [HandlerLogin]
    public class ClientsDataController : ControllerBase
    {
        LeaveApp leaveApp = new LeaveApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetClientsDataJson()
        {
            try
            {
                var data = new
                {
                    dataItems = this.GetDataItemList(),
                    organize = this.GetOrganizeList(),
                    role = this.GetRoleList(),
                    duty = this.GetDutyList(),
                    authorizeMenu = this.GetMenuList(),
                    authorizeButton = this.GetMenuButtonList(),
                    type = this.GetTypeList()
                };
                return Content(data.ToJson());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private object GetTypeList()
        {
            try
            {
                TypeApp typeApp = new TypeApp();
                var data = typeApp.GetList();
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (TypeEntity item in data)
                {
                    var fieldItem = new
                    {
                        encode = item.ID,
                        fullname = item.TypeName
                    };
                    dictionary.Add(item.ID.ToString(), fieldItem);
                }
                return dictionary;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private object GetDataItemList()
        {
            try
            {
                var itemdata = new ItemsDetailApp().GetList();
                Dictionary<string, object> dictionaryItem = new Dictionary<string, object>();
                foreach (var item in new ItemsApp().GetList())
                {
                    var dataItemList = itemdata.FindAll(t => t.F_ItemId.Equals(item.F_Id));
                    Dictionary<string, string> dictionaryItemList = new Dictionary<string, string>();
                    foreach (var itemList in dataItemList)
                    {
                        dictionaryItemList.Add(itemList.F_ItemCode, itemList.F_ItemName);
                    }
                    dictionaryItem.Add(item.F_EnCode, dictionaryItemList);
                }
                return dictionaryItem;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private object GetOrganizeList()
        {
            OrganizeApp organizeApp = new OrganizeApp();
            var data = organizeApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (OrganizeEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_EnCode,
                    fullname = item.F_FullName,
                    DepartGroupId=item.F_DepartGroupId
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            return dictionary;
        }
        private object GetRoleList()
        {
            RoleApp roleApp = new RoleApp();
            var data = roleApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_EnCode,
                    fullname = item.F_FullName
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            return dictionary;
        }
        private object GetDutyList()
        {
            DutyApp dutyApp = new DutyApp();
            var data = dutyApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (RoleEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_EnCode,
                    fullname = item.F_FullName
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            return dictionary;
        }
        private object GetUserList()
        {
            UserApp userApp = new UserApp();
            var data = userApp.GetUserList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (UserEntity item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_Id,
                    fullname = item.F_RealName,
                    jobno=item.F_Account
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            return dictionary;
        }
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        private object GetMenuList()
        {
            var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
            return ToMenuJson(new RoleAuthorizeApp().GetMenuList(roleId), "0");
        }
        private string ToMenuJson(List<ModuleEntity> data, string parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<ModuleEntity> entitys = data.FindAll(t => t.F_ParentId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"ChildNodes\":" + ToMenuJson(data, item.F_Id) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }
        private object GetMenuButtonList()
        {
            var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var data = new RoleAuthorizeApp().GetButtonList(roleId);
            var dataModuleId = data.Distinct(new ExtList<ModuleButtonEntity>("F_ModuleId"));
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (ModuleButtonEntity item in dataModuleId)
            {
                var buttonList = data.Where(t => t.F_ModuleId.Equals(item.F_ModuleId));
                dictionary.Add(item.F_ModuleId, buttonList);
            }
            return dictionary;
        }

    }
}
