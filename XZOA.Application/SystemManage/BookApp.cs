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
    public class BookApp
    {
        IBookRepository service = new BookRepository();
        IBookTypeRepository type = new BookTypeRepository();

        public BookEntity GetForm(int keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public BookTypeEntity GetTypeForm(int keyValue)
        {
            return type.FindEntity(keyValue);
        }

        public List<BookTypeEntity> GetBookTypeList()
        {
            try
            {
                var query = type.IQueryable();
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BookEntity> GetList(Pagination pagination, int? itemId, string keyword)
        {
            try
            {
                var query = service.IQueryable();
                if(itemId!=null)
                {
                    query = query.Where(t => t.TypeID == itemId);
                }
                if(!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.FullName.Contains(keyword)||t.NickName.Contains(keyword));
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.ID).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<BookEntity> GetUserList(Pagination pagination, int? itemId, string keyword)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable(t=>t.CreateMan.Equals(userName));
                if (itemId != null)
                {
                    query = query.Where(t => t.TypeID == itemId);
                }
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.FullName.Contains(keyword) || t.NickName.Contains(keyword));
                }
                
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.ID).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BookEntity> GetBookListByType(int keyValue)
        {
            try
            {
                var query = service.IQueryable(t=>t.TypeID==keyValue);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<BookTypeEntity> GetTypeListByParentID(int keyValue)
        {
            try
            {
                var query = type.IQueryable(t => t.ParentID == keyValue);
                return query.ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public List<BookMeetingEntity> GetMeetingTime(DateTime time,int roomID)
        //{
        //    try
        //    {
        //        var query = service.IQueryable(t=>t.MeetingDate==time&&t.RoomID == roomID&&t.PreResult!=2&&t.AuditTag!=0);
        //        return query.ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        public void SubmitForm(BookEntity bookEntity, int? keyValue = null)
        {
            try
            {
                if (keyValue == null)
                {
                    service.Insert(bookEntity);
                }
                else
                {
                    bookEntity.ID = keyValue.Value;
                    service.Update(bookEntity);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void SubmitTypeForm(BookTypeEntity bookTypeEntity, int? keyValue = null)
        {
            try
            {
                if (keyValue == null)
                {
                    type.Insert(bookTypeEntity);
                }
                else
                {
                    bookTypeEntity.ID = keyValue.Value;
                    type.Update(bookTypeEntity);
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

        public void DeleteTypeForm(int keyValue)
        {
            type.Delete(t => t.ID == keyValue);
        }

    }
}
