using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XZOA.Application.SystemManage;
using XZOA.Code;
using XZOA.Domain.Entity;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Web.Areas.RoomManage.Controllers
{
    [HandlerLogin]
    public class RoomManageController : ControllerBase
    {
        private readonly static object _MyLock = new object();
        // GET: RoomManage/RoomManage
        BookRoomApp bookRoomApp = new BookRoomApp();
        MailHelper mHelper = new MailHelper();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(int keyValue)
        {
            BookMeetingEntity templateEntity = bookRoomApp.GetForm(keyValue);
            return Content(templateEntity.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetRoom(int keyValue)
        {
            MeetRoomEntity templateEntity = bookRoomApp.GetRoomForm(keyValue);
            return Content(templateEntity.ToJson());
        }
        
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetMeetingTime(DateTime date,int RoomNo)
        {
            var query = bookRoomApp.GetMeetingTime(date, RoomNo);
            return Content(query.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeJson()
        {
            List<TreeSelectModel> treeGrids = new List<TreeSelectModel>();
            var query = bookRoomApp.GetTreeList();
            foreach (var item in query)
            {
                TreeSelectModel tree = new TreeSelectModel();
                tree.id = item.ID.ToString();
                tree.text = item.Name;
                treeGrids.Add(tree);
            }
            return Content(treeGrids.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetRoomForm()
        {
            var query = bookRoomApp.GetTreeList();
            return Content(query.ToJson());
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult CancelMeet(string CancelRea, string keyValue)
        {
            try
            {
                var Ids = keyValue.Split(',');
                var keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var key in keys)
                {
                    var bookMeetingEntity = bookRoomApp.GetForm(key);
                    bookMeetingEntity.CancelRea = CancelRea;
                    bookMeetingEntity.PreResult = 2;//取消状态
                    bookMeetingEntity.AuditTag = 1;//已处理
                    bookRoomApp.SubmitForm(bookMeetingEntity, key);
                }
                return Success("操作成功");
            }
            catch (Exception)
            {

                return Error("操作失败");
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult CancelMeetOne(string CancelRea, int keyValue)
        {
            try
            {
                var bookMeetingEntity = bookRoomApp.GetForm(keyValue);
                bookMeetingEntity.CancelRea = CancelRea;
                bookMeetingEntity.PreResult = 2;//取消状态
                bookMeetingEntity.AuditTag = 1;//已处理
                bookRoomApp.SubmitForm(bookMeetingEntity, keyValue);
                return Success("操作成功");
            }
            catch (Exception)
            {

                return Error("操作成功");
            }
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetList(Pagination pagination)
        {
            var query = bookRoomApp.GetList(pagination);
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
        public ActionResult GetRoomList(Pagination pagination)
        {
            var query = bookRoomApp.GetRoomList(pagination);
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
        public ActionResult GetTodayRoomList()
        {
            var query = bookRoomApp.GetRoomList();
            List<MeetingRoom> meetings = new List<MeetingRoom>();
            foreach (var item in query)
            {
                MeetingRoom room = new MeetingRoom();
                room.RoomName = item.Name;
                var list = bookRoomApp.GetMeetingTimeList(item.Name);
                room.meetingTimes = list;
                meetings.Add(room);
            }
            return Content(meetings.ToJson());
        }

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetHandleList(Pagination pagination)
        {
            var query = bookRoomApp.GetHandleList(pagination);
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
        public ActionResult GetBookMeetingList(Pagination pagination,DateTime? BeginDate,DateTime? EndDate)
        {
            if(BeginDate == null)
            {
                BeginDate = DateTime.Now.Date;
                EndDate = DateTime.Now.Date;
            }
            var query = bookRoomApp.GetBookMeetingList(pagination, BeginDate.Value, EndDate.Value);
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
        public ActionResult SubmitForm(BookMeetingEntity bookMeetingEntity,int? keyValue)
        {
            
            try
            {
                if (bookMeetingEntity != null)
                {
                    if (keyValue == null)
                    {
                        if(bookMeetingEntity.RoomID!= null)
                        {
                            bookMeetingEntity.RoomName = bookRoomApp.GetRoomForm(bookMeetingEntity.RoomID.Value).Name;
                        }
                        bookMeetingEntity.AppMan = OperatorProvider.Provider.GetCurrent().UserName;
                        bookMeetingEntity.CreateTime = DateTime.Now;
                        bookMeetingEntity.AuditTag = 0;//未处理
                        bookRoomApp.SubmitForm(bookMeetingEntity);
                        var roleIds = new RoleApp().GetRoleUser("前台");
                        var list = new UserApp().GetUserList(roleIds);
                        foreach (var item in list)
                        {
                            if (item.F_Email != null)
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(item.F_Email, "会议室预定", "你好," + bookMeetingEntity.AppMan + "有会议室申请需要你处理,请点击链接<a>http://10.110.120.6:8090/</a>");
                            }
                        }
                        return Success("操作成功。");
                    }
                    else
                    {
                        lock (_MyLock)
                        {
                            var meet = bookRoomApp.GetForm(keyValue.Value);
                            meet.PreEndTime = bookMeetingEntity.PreEndTime;
                            meet.BeginTime = bookMeetingEntity.BeginTime;
                            meet.MeetingDate = bookMeetingEntity.MeetingDate;
                            meet.RoomID = bookMeetingEntity.RoomID;
                            if (bookMeetingEntity.RoomID != null)
                            {
                                meet.RoomName = bookRoomApp.GetRoomForm(bookMeetingEntity.RoomID.Value).Name;
                            }
                            meet.Subject = bookMeetingEntity.Subject;
                            meet.Remark = bookMeetingEntity.Remark;
                            bookRoomApp.SubmitForm(meet, keyValue);
                            return Success("操作成功。");
                        }

                    }

                }
                return Error("操作失败。");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRoomForm(MeetRoomEntity meetRoomEntity, int? keyValue)
        {

            try
            {
                if (meetRoomEntity != null)
                {
                    if (keyValue == null)
                    {
                        bookRoomApp.SubmitRoomForm(meetRoomEntity);
                        return Success("操作成功。");
                    }
                    else
                    {
                        var room = bookRoomApp.GetRoomForm(keyValue.Value);
                        room.HasNotebook = meetRoomEntity.HasNotebook;
                        room.HasProjector = meetRoomEntity.HasProjector;
                        room.Name = meetRoomEntity.Name;
                        room.Number = meetRoomEntity.Number;
                        room.Remark = meetRoomEntity.Remark;
                        room.Location = meetRoomEntity.Location;
                        room.Teleconferencing = meetRoomEntity.Teleconferencing;
                        room.Videoconferencing = meetRoomEntity.Videoconferencing;
                        bookRoomApp.SubmitRoomForm(room,keyValue);
                        return Success("操作成功。");
                    }

                }
                return Error("操作失败。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }

        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCheckForm(BookMeetingEntity bookMeetingEntity, string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
               
                if (bookMeetingEntity != null)
                {
                    int[] keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                    foreach (var key in keys) {
                        var meet = bookRoomApp.GetForm(key);
                        meet.AuditIdea = bookMeetingEntity.AuditIdea;
                        meet.PreResult = bookMeetingEntity.PreResult;
                        meet.AuditTag = 1;//已处理
                        bookRoomApp.SubmitForm(meet,key);
                        if(bookMeetingEntity.PreResult == 1)//预定成功
                        {
                            var user = new UserApp().GetFormByName(meet.AppMan);
                            if(user.F_Email!=null)
                            {
                                mHelper.MailServer = "10.110.120.2";
                                mHelper.Send(user.F_Email, "会议室预定成功", "会议地点:"+ meet.RoomName + "\n会议主题：" + meet.Subject +"\n开始时间："+ meet.BeginTime.ToDateTimeString()
                                    +"\n结束时间："+ meet.PreEndTime.ToDateTimeString()+"\n");
                            }
                        }
                        return Success("操作成功。");
                    }
                }
                return Error("操作失败。");
            }
            catch (Exception ex)
            {

                throw;
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
                    bookRoomApp.DeleteForm(key);
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
        public ActionResult DeleteRoomForm(string keyValue)
        {
            try
            {
                string[] Ids = keyValue.Split(',');
                int[] keys = Array.ConvertAll<string, int>(Ids, s => int.Parse(s));
                foreach (var key in keys)
                {
                    bookRoomApp.DeleteRoomForm(key);
                }
                return Success("删除成功。");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }
        }


        public ActionResult HandleIndex()
        {
            return View();
        }

        public ActionResult ManageIndex()
        {
            return View();
        }

        public ActionResult CancelIndex()
        {
            return View();
        }

        public ActionResult CancelPersonIndex()
        {
            return View();
        }
        
        public ActionResult Bookingndex()
        {
            return View();
        }
        
        public ActionResult CheckForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CancelForm()
        {
            return View();
        }
        

        [HttpGet]
        public ActionResult CancelOneForm()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddRoomForm()
        {
            return View();
        }
    }
}