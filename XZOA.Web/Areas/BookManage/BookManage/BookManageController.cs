using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Web.Areas.BookManage.Controllers
{
    [HandlerLogin]
    public class BookManageController : ControllerBase
    {
        BookApp bookApp = new BookApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTypeFormJson(int keyValue)
        {
            var data = bookApp.GetTypeForm(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(int keyValue)
        {
            var data = bookApp.GetForm(keyValue);
            return Content(data.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult IsCreateMan(int keyValue)
        {
            var data = bookApp.GetForm(keyValue);
            if(data.CreateMan.Equals(OperatorProvider.Provider.GetCurrent().UserName))
            {
                return Json(true,JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeJson()
        {
            var data = bookApp.GetBookTypeList();
            var treeList = new List<TreeViewModel>();
            foreach (BookTypeEntity item in data)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = data.Count(t => t.ParentID == item.ID) == 0 ? false : true;
                tree.id = item.ID.ToString();
                tree.text = item.Name;
                tree.value = item.ID.ToString();
                tree.parentId = item.ParentID.ToString();
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeGridJson()
        {
            var data = bookApp.GetBookTypeList();
            var treeList = new List<TreeGridModel>();
            foreach (BookTypeEntity item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                bool hasChildren = data.Count(t => t.ParentID == item.ID) == 0 ? false : true;
                treeModel.id = item.ID.ToString();
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.ParentID.ToString();
                treeModel.expanded = hasChildren;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var data = bookApp.GetBookTypeList();
            var treeList = new List<TreeSelectModel>();
            foreach (BookTypeEntity item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.ID.ToString();
                treeModel.text = item.Name;
                treeModel.parentId = item.ParentID.ToString();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination,int? itemId, string keyword)
        {
            var query = bookApp.GetList(pagination,itemId, keyword);
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
        public ActionResult GetUserGridJson(Pagination pagination, int? itemId, string keyword)
        {
            var query = bookApp.GetUserList(pagination, itemId, keyword);
            var data = new
            {
                rows = query,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(BookEntity book, int? keyValue,int ItemId)
        {
            try
            {
                if (keyValue != null)
                {
                    var bookEntity = bookApp.GetForm(keyValue.Value);
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files["FullName"];
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
                                bookEntity.FullName = file.FileName;
                                var flag = "B";
                                double size = file.ContentLength;//B
                                if (size >= 1024)
                                {
                                    size = size / 1024;//KB
                                    flag = "KB";
                                    if (size >= 1024)
                                    {
                                        size = size / 1024;//MB
                                        flag = "MB";
                                        if (size >= 1024)
                                        {
                                            size = size / 1024;//GB
                                            flag = "GB";
                                        }
                                    }
                                }
                                bookEntity.Size = Math.Round(size, 2) + flag;
                            }
                        }
                    }
                    bookEntity.NickName = book.NickName;
                    bookEntity.Remark = book.Remark;
                    bookEntity.Subject = book.Subject;
                    bookApp.SubmitForm(bookEntity,keyValue);
                }
                else {
                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files["FullName"];
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
                                book.FullName = fileName;
                                var flag = "B";
                                double size = file.ContentLength;//B
                                if (size >= 1024)
                                {
                                    size = size / 1024;//KB
                                    flag = "KB";
                                    if (size >= 1024)
                                    {
                                        size = size / 1024;//MB
                                        flag = "MB";
                                        if (size >= 1024)
                                        {
                                            size = size / 1024;//GB
                                            flag = "GB";
                                        }
                                    }
                                }
                                book.Size = Math.Round(size, 2) + flag;
                            }
                        }
                    }
                    book.TypeID = ItemId;
                    book.UploadTime = DateTime.Now;
                    book.CreateMan = OperatorProvider.Provider.GetCurrent().UserName;
                    bookApp.SubmitForm(book);
                }
                
                
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        public ActionResult SubmitMultiForm(int ItemId)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        BookEntity bookEntity = new BookEntity();
                        var file = Request.Files[i];
                        if (!string.IsNullOrEmpty(file.FileName))
                        {
                            var fileName = file.FileName;
                            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                            {
                                fileName = fileName.Replace(invalidChar.ToString(), "_");
                            }
                            fileName = fileName.Replace(" ", "");
                            var filePath = Path.Combine(HttpContext.Server.MapPath("/Uploads/Book/"), fileName);
                            file.SaveAs(filePath);
                            bookEntity.TypeID = ItemId;
                            bookEntity.FullName = file.FileName;
                            bookEntity.NickName = file.FileName.Remove(file.FileName.LastIndexOf("."));
                            bookEntity.Subject= file.FileName.Remove(file.FileName.LastIndexOf("."));
                            var flag = "B";
                            double size = file.ContentLength;//B
                            if(size>=1024)
                            {
                                size = size / 1024;//KB
                                flag = "KB";
                                if (size >= 1024)
                                {
                                    size = size / 1024;//MB
                                    flag = "MB";
                                    if (size >= 1024)
                                    {
                                        size = size / 1024;//GB
                                        flag = "GB";
                                    }
                                }
                            }
                            bookEntity.Size = Math.Round(size, 2) + flag;
                            bookEntity.UploadTime = DateTime.Now;
                            bookEntity.CreateMan = OperatorProvider.Provider.GetCurrent().UserName;
                            bookApp.SubmitForm(bookEntity);
                        }
                    }
                }
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitTypeForm(BookTypeEntity bookType, int? keyValue)
        {
            try
            {
                if (keyValue != null)
                {
                    var bookTypeEntity = bookApp.GetTypeForm(keyValue.Value);
                    bookTypeEntity.ParentID = bookType.ParentID;
                    bookTypeEntity.Name = bookType.Name;
                    bookTypeEntity.SortCode = bookType.SortCode;
                    bookTypeEntity.EnabledMark = bookType.EnabledMark;
                    bookTypeEntity.Remark = bookType.Remark;
                    bookApp.SubmitTypeForm(bookTypeEntity, keyValue);
                }
                else
                {
                    bookType.CreateTime = DateTime.Now;
                    bookType.CreateMan = OperatorProvider.Provider.GetCurrent().UserName;
                    bookApp.SubmitTypeForm(bookType);
                }


                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Success("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                int[] keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var key in keys)
                {
                    bookApp.DeleteForm(key);
                }
                return Success("删除成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTypeForm(int keyValue)
        {
            try
            {
                var query = bookApp.GetBookListByType(keyValue);
                if(query.Count==0)
                {
                    var types = bookApp.GetTypeListByParentID(keyValue);
                    if(types.Count>0)
                    {
                        foreach (var item in types)
                        {
                            bookApp.DeleteTypeForm(item.ID);
                        }
                    }
                    bookApp.DeleteTypeForm(keyValue);
                    return Success("删除成功。");
                }
                return Error("已有该分类书籍，不能删除。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult TypeIndex()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult MultiForm()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult TypeForm()
        {
            return View();
        }

        [HttpGet]
        [HandlerAuthorize]
        public ActionResult MyBookIndex()
        {
            return View();
        }
    }
}