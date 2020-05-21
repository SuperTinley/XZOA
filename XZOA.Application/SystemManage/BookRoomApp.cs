/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.OleDb;
using System.Data;
using XZOA.Code;
using XZOA.Data;
using XZOA.Domain.Entity;

namespace XZOA.Application.SystemManage
{
    public class BookRoomApp
    {
        IBookMeetingRepository service = new BookMeetingRepository();
        IMeetRoomRepository room = new MeetRoomRepository();

        public BookMeetingEntity GetForm(int keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public List<MeetRoomEntity> GetRoomList(Pagination pagination=null)
        {
            try
            {
                var query = room.IQueryable();
                if(pagination!=null)
                {
                    pagination.records = query.Count();
                    query = query.OrderBy(t => t.CreateTime).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                }
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<MeetingTime> GetMeetingTimeList(string RoomName)
        {
            try
            {
                List<MeetingTime> meetingTimes = new List<MeetingTime>();
                var query = service.IQueryable(t => (t.PreResult != 0 && t.PreResult != 2) && t.RoomName.Equals(RoomName) && t.MeetingDate.Value.Year==DateTime.Now.Year&& t.MeetingDate.Value.Month == DateTime.Now.Month && t.MeetingDate.Value.Day == DateTime.Now.Day)
                    .OrderByDescending(q => q.MeetingDate).ToList();//除审核不通过之外
                foreach (var l in query)
                {
                    MeetingTime time = new MeetingTime();
                    time.ID = l.ID;
                    time.BeginTime = l.BeginTime.Value;
                    time.PreEndTime = l.PreEndTime.Value;
                    time.Subject = l.Subject;
                    time.AppMan = l.AppMan;
                    time.AuditTag = l.AuditTag;
                    meetingTimes.Add(time);
                }
                return meetingTimes.OrderBy(t=>t.BeginTime).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BookMeetingEntity> GetList(Pagination pagination)
        {
            try
            {
                var query = service.IQueryable();
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.ID).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BookMeetingEntity> GetHandleList(Pagination pagination)
        {
            try
            {
                var query = service.IQueryable(t=>t.AuditTag==0);
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.ID).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 获取会议室预定情况
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="BeginDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public List<MeetingRoom> GetBookMeetingList(Pagination pagination,DateTime BeginDate, DateTime EndDate)
        {
            try
            {
                List<MeetingRoom> meetings = new List<MeetingRoom>();
                var query = service.IQueryable(t=> (t.PreResult != 0 && t.PreResult != 2) && t.MeetingDate >= BeginDate && t.MeetingDate<=EndDate).OrderByDescending(q => q.MeetingDate).ToList();//除审核不通过之外
                meetings = (from q in query
                            group q by new { q.RoomID, q.RoomName, q.MeetingDate } into g
                            select new MeetingRoom
                            {
                                RoomID = g.Key.RoomID ?? 0,
                                RoomName = g.Key.RoomName,
                                MeetingDate = g.Key.MeetingDate.Value
                            }).ToList();
                foreach (var m in meetings)
                {
                    List<MeetingTime> meetingTimes= new List<MeetingTime>();
                    var list = query.Where(t=>t.RoomID==m.RoomID&&t.MeetingDate==m.MeetingDate).OrderBy(t=>t.BeginTime);
                    foreach (var l in list)
                    {
                        MeetingTime time = new MeetingTime();
                        time.ID = l.ID;
                        time.BeginTime = l.BeginTime.Value;
                        time.PreEndTime = l.PreEndTime.Value;
                        time.Subject = l.Subject;
                        time.AuditTag = l.AuditTag;
                        meetingTimes.Add(time);
                    }
                    m.meetingTimes = meetingTimes;
                }
                pagination.records = meetings.Count();
                meetings = meetings.Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows).ToList();
                return meetings;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<MeetRoomEntity> GetTreeList()
        {
            try
            {
                var query = room.IQueryable();
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        
        public MeetRoomEntity GetRoomForm(int keyValue)
        {
            try
            {
                var query = room.FindEntity(keyValue);
                return query;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BookMeetingEntity> GetMeetingTime(DateTime time,int roomID)
        {
            try
            {
                var query = service.IQueryable(t=>t.MeetingDate==time&&t.RoomID == roomID&&t.PreResult!=2);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void SubmitForm(BookMeetingEntity bookMeetingEntity,int? keyValue=null)
        {
            try
            {
                if (keyValue == null)
                {
                    service.Insert(bookMeetingEntity);
                }
                else {
                    bookMeetingEntity.ID = keyValue.Value;
                    service.Update(bookMeetingEntity);
                }
              
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void SubmitRoomForm(MeetRoomEntity meetRoomEntity, int? keyValue = null)
        {
            try
            {
                if (keyValue == null)
                {
                    room.Insert(meetRoomEntity);
                }
                else
                {
                    meetRoomEntity.ID = keyValue.Value;
                    room.Update(meetRoomEntity);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void DeleteForm(int keyValue)
        {
            service.Delete(t => t.ID == keyValue);
        }

        public void DeleteRoomForm(int keyValue)
        {
            room.Delete(t => t.ID == keyValue);
        }
    }
}
