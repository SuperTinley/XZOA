using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XZOA.Web.Areas.SystemManage.Controllers
{
    [HandlerLogin]
    public class ItemsDataController : ControllerBase
    {
        private ItemsDetailApp itemsDetailApp = new ItemsDetailApp();
        private ItemsApp itemsApp = new ItemsApp();
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(string itemId, string keyword)
        {
            var data = itemsDetailApp.GetList(itemId, keyword);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJson(string enCode)
        {
            var item = itemsApp.GetItemForm(enCode);
            if (item != null)
            {
                var data = itemsDetailApp.GetItemDataList(item.F_Id);
                List<object> list = new List<object>();
                foreach (ItemsDetailEntity detail in data)
                {
                    list.Add(new { id = detail.F_ItemCode, text = detail.F_ItemName });
                }
                return Content(list.ToJson());
            }
            return null;
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectJsonByType(string enCode)
        {
            var item = itemsApp.GetItemForm(enCode);
            if(item!=null)
            {
                var data = itemsDetailApp.GetItemDataList(item.F_Id);
                List<object> list = new List<object>();
                foreach (ItemsDetailEntity detail in data)
                {
                    list.Add(new { id = detail.F_ItemCode, text = detail.F_ItemName });
                }
                return Content(list.ToJson());
            }
            return null;
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson(string enCode)
        {
            var data = itemsDetailApp.GetItemList(enCode);
            List<object> list = new List<object>();
            foreach (ItemsDetailEntity item in data)
            {
                list.Add(new { id = item.F_ItemCode, text = item.F_ItemName });
            }
            return Content(list.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = itemsDetailApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(ItemsDetailEntity itemsDetailEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var itemDetail = itemsDetailApp.GetForm(keyValue);
                itemDetail.F_ItemName = itemsDetailEntity.F_ItemName;
                itemDetail.F_ItemCode = itemsDetailEntity.F_ItemCode;
                itemDetail.F_SortCode = itemsDetailEntity.F_SortCode;
                itemDetail.F_IsDefault = itemsDetailEntity.F_IsDefault;
                itemDetail.F_EnabledMark = itemsDetailEntity.F_EnabledMark;
                itemDetail.F_Description = itemsDetailEntity.F_Description;
                itemsDetailApp.SubmitForm(itemDetail, keyValue);
            }
            else {
                itemsDetailApp.SubmitForm(itemsDetailEntity, keyValue);
            }
            
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            itemsDetailApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
    }
}
