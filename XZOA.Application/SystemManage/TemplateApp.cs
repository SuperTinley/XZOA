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
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using XZOA.Code;

namespace XZOA.Application.SystemManage
{
    public class TemplateApp
    {
        private ITemplateRepository service = new TemplateRepository();

        public TemplateEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public TemplateEntity GetFormBySourceID(string keyValue)
        {
            return service.IQueryable(t=>t.SOURCE_ID.Equals(keyValue)).FirstOrDefault();
        }

        public int GetTempCheckCount()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.AUDIT_MAN.Equals(userName) && t.AUDIT_TAG == "F");//未审核
            return query.Count();
        }

        public int GetTempAcceptCount()
        {
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            var query = service.IQueryable(t => t.TEM_CHARGE.Equals(userName) && t.AUDIT_TAG == "T" && t.TEM_ACCEPT == "D");//未接收
            return query.Count();
        }

        public List<TemplateEntity> GetList(string keyValue)
        {
            List<TemplateEntity> templateEntities = new List<TemplateEntity>();
            var temp = GetForm(keyValue);
            GetChildren(keyValue,templateEntities);
            GetSoure(keyValue, templateEntities);
            return templateEntities;
        }
        
        public void GetChildren(string keyValue, List<TemplateEntity> templateEntities)
        {
            var temp = service.IQueryable(t => t.SOURCE_ID.Equals(keyValue)).FirstOrDefault();
            if(temp!=null)
            {
                templateEntities.Add(temp);
                GetChildren(temp.TEM_ID, templateEntities);
            }
        }

        public void GetSoure(string keyValue, List<TemplateEntity> templateEntities)
        {
            var temp = service.IQueryable(t => t.TEM_ID.Equals(keyValue)).FirstOrDefault();
            if (temp != null)
            {
                templateEntities.Add(temp);
                GetSoure(temp.SOURCE_ID, templateEntities);
            }
        }

        public List<TemplateEntity> GetTempApplyList(Pagination pagination, string keyword, string departName, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                
                var query = service.IQueryable(t=>t.CREATE_USER.Equals(userName)&& t.APP_DEP.Equals(departName) &&(t.AUDIT_TAG=="F"||t.TEM_ACCEPT=="F"));//未审核
                if(!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t=>t.CREATE_USER.Contains(keyword)|| t.TEM_NAME.Contains(keyword)|| t.TEM_NO.Contains(keyword)|| t.CUSTOMER.Contains(keyword)|| t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if(hopeDDBeginTime!=null&&hopeDDEndTime!=null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t=>t.HOPE_DD>=hopeDDBeginTime&& t.HOPE_DD < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TemplateEntity> GetTempCheckList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable(t =>t.AUDIT_MAN.Equals(userName) && t.AUDIT_TAG == "F");//未审核
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.CREATE_USER.Contains(keyword) || t.TEM_NAME.Contains(keyword) || t.TEM_NO.Contains(keyword) || t.CUSTOMER.Contains(keyword) || t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if (hopeDDBeginTime != null && hopeDDEndTime != null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.HOPE_DD >= hopeDDBeginTime && t.HOPE_DD < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }

        public List<TemplateEntity> GetTempAllotList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable(t => t.TEM_ACCEPT=="F");//未审核
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.CREATE_USER.Contains(keyword) || t.TEM_NAME.Contains(keyword) || t.TEM_NO.Contains(keyword) || t.CUSTOMER.Contains(keyword) || t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if (hopeDDBeginTime != null && hopeDDEndTime != null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.HOPE_DD >= hopeDDBeginTime && t.HOPE_DD < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }

        public List<TemplateEntity> GetTempAcceptList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable(t =>t.TEM_CHARGE.Equals(userName) &&t.AUDIT_TAG=="T" && t.TEM_ACCEPT=="D");//未接收
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.CREATE_USER.Contains(keyword) || t.TEM_NAME.Contains(keyword) || t.TEM_NO.Contains(keyword) || t.CUSTOMER.Contains(keyword) || t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if (hopeDDBeginTime != null && hopeDDEndTime != null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.HOPE_DD >= hopeDDBeginTime && t.HOPE_DD < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }

        public List<TemplateEntity> GetTempNoticeList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable(t => t.NOTICE_GET=="F"&& t.CLOSE_ID=="F" && t.TEM_CHARGE.Equals(userName) && t.TEM_ACCEPT == "T");//
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.CREATE_USER.Contains(keyword) || t.TEM_NAME.Contains(keyword) || t.TEM_NO.Contains(keyword) || t.CUSTOMER.Contains(keyword) || t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if (hopeDDBeginTime != null && hopeDDEndTime != null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.HOPE_DD >= hopeDDBeginTime && t.HOPE_DD < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }

        public List<TemplateEntity> GetTempConfirmList(Pagination pagination, string keyword, string sidx, string sord, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable(t => t.NOTICE_GET == "T" && t.CLOSE_ID == "F" && t.CREATE_USER.Equals(userName));//
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.CREATE_USER.Contains(keyword) || t.TEM_NAME.Contains(keyword) || t.TEM_NO.Contains(keyword) || t.CUSTOMER.Contains(keyword) || t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if (hopeDDBeginTime != null && hopeDDEndTime != null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.HOPE_DD >= hopeDDBeginTime && t.HOPE_DD < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }

        public List<TemplateEntity> GetTempReportList(Pagination pagination, string keyword, DateTime? hopeDDBeginTime, DateTime? hopeDDEndTime, string closeID, string accept)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable();
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(t => t.CREATE_USER.Contains(keyword) || t.TEM_NAME.Contains(keyword) || t.TEM_NO.Contains(keyword) || t.CUSTOMER.Contains(keyword) || t.TEM_CHARGE.Contains(keyword) || t.TEM_TYPE.Contains(keyword));
                }
                if (hopeDDBeginTime != null && hopeDDEndTime != null)
                {
                    var endDate = DateTime.Parse(hopeDDEndTime.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.HOPE_DD >= hopeDDBeginTime && t.HOPE_DD < endDate);
                }
                if(!string.IsNullOrEmpty(closeID))
                {
                    query = query.Where(t=>t.CLOSE_ID.Equals(closeID));
                }
                if (!string.IsNullOrEmpty(accept))
                {
                    query = query.Where(t => t.TEM_ACCEPT.Equals(accept));
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }

        public List<TemplateEntity> GetTempExportList(Pagination pagination, DateTime? finishBeginTime, DateTime? finishEndDate)
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable();
                if (finishBeginTime != null && finishEndDate != null)
                {
                    var endDate = DateTime.Parse(finishEndDate.Value.AddDays(1).ToString("yyyy-MM-dd"));
                    query = query.Where(t => t.NOTICE_DATE >= finishBeginTime && t.NOTICE_DATE < endDate);
                }
                pagination.records = query.Count();
                query = query.OrderByDescending(q => q.CREATE_DATE).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
                return query.ToList();
            }
            catch (Exception ex)
            {

                Console.Write(ex.Message);
                return null;
            }
        }


        public string ProduceNo()
        {
            string TEM_NO;
            var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string handleDate = DateTime.Now.Year.ToString().Remove(0, 2) + month + day;
            object maxTodayObj = GetTodayMax(handleDate);
            if (object.Equals(maxTodayObj, 000))
            {
                TEM_NO = handleDate + "001";
            }
            else
            {
                int dbNum = int.Parse(maxTodayObj.ToString());
                int tempNum = dbNum + 1;
                string tempVal = string.Empty;
                if (dbNum < 9)
                    tempVal = "00" + tempNum.ToString();
                else if (dbNum == 9) tempVal = "010";
                else if (dbNum > 9 && dbNum < 99)
                    tempVal = "0" + tempNum.ToString();
                else if (dbNum == 99) tempVal = "100";
                else
                    tempVal =tempNum.ToString();
                TEM_NO = handleDate + tempVal;
            }

            return TEM_NO;

        }

        public string ProduceGroupID()
        {
            string GroupID;
            var month = DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var day = DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            string handleDate = DateTime.Now.Year.ToString().Remove(0, 2) + month + day;
            object maxTodayObj = GetTodayGroupMax(handleDate);
            if (object.Equals(maxTodayObj, 000))
            {
                GroupID = handleDate + "001";
            }
            else
            {
                int dbNum = int.Parse(maxTodayObj.ToString());
                int tempNum = dbNum + 1;
                string tempVal = string.Empty;
                if (dbNum < 9)
                    tempVal = "00" + tempNum.ToString();
                else if (dbNum == 9) tempVal = "010";
                else if (dbNum > 9 && dbNum < 99)
                    tempVal = "0" + tempNum.ToString();
                else if (dbNum == 99) tempVal = "100";
                else
                    tempVal = tempNum.ToString();
                GroupID = handleDate + tempVal;
            }

            return GroupID;

        }

        public object GetTodayMax(string date)
        {
            var temps = service.IQueryable(t => t.TEM_NO.Contains(date));
            int maxNum = 000;
            int todayNum = 0;
            if (temps.Count() > 0)
            {
                foreach (var p in temps)
                {
                    var num = Convert.ToInt32(p.TEM_NO.Remove(0, 6));
                    if (num > todayNum)
                    {
                        todayNum = num;//当天最大单号
                    }
                }

                maxNum = todayNum;
            }
            return maxNum;
        }

        public object GetTodayGroupMax(string date)
        {
            var temps = service.IQueryable(t => t.GROUP_ID.Contains(date));
            int maxNum = 000;
            int todayNum = 0;
            if (temps.Count() > 0)
            {
                foreach (var p in temps)
                {
                    var num = Convert.ToInt32(p.GROUP_ID.Remove(0, 6));
                    if (num > todayNum)
                    {
                        todayNum = num;//当天最大单号
                    }
                }

                maxNum = todayNum;
            }
            return maxNum;
        }

        public void SubmitForm(TemplateEntity templateEntity, string keyValue = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    templateEntity.TEM_ID = keyValue;
                    service.Update(templateEntity);
                }
                else
                {
                    templateEntity.TEM_ID = Guid.NewGuid().ToString();
                    service.Insert(templateEntity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.TEM_ID == keyValue);
        }
    }
}
