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
    public class ProposalApp
    {
        private IProposalRepository service = new ProposalRepository();
        
        public ProposalEntity GetForm(int keyValue)
        {
            return service.FindEntity(keyValue);
        }

        //public int GetFormByDepartName(string departName)
        //{
        //    var depart = service.IQueryable().FirstOrDefault();
        //}

        public ProposalEntity GetFirstForm()
        {
            return service.IQueryable().OrderByDescending(t=>t.pro_id).FirstOrDefault();
        }

        public List<ProposalEntity> GetList(Pagination pagination,string departName, string keyword, string pro_dep, string pro_send_dep, DateTime? planBeginDate, DateTime? planEndDate, DateTime? cheDDBeginDate, DateTime? cheDDEndDate,string status,bool isIE,bool isRevice)
        {
            var query = service.IQueryable();
            var userName = OperatorProvider.Provider.GetCurrent().UserName;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t=>t.pro_id.Contains(keyword)|| t.pro_man.Contains(keyword)|| t.imp_man.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(pro_dep))
            {
                query = query.Where(t => t.pro_dep.Equals(pro_dep));
            }
            if (!string.IsNullOrEmpty(pro_send_dep))
            {
                query = query.Where(t => t.pro_send_dep.Equals(pro_send_dep));
            }
            if(planBeginDate!=null&&planEndDate!=null)
            {
                query = query.Where(t=>t.plan_finish_date >= planBeginDate&&t.plan_finish_date <= planEndDate);
            }
            if (cheDDBeginDate != null && cheDDBeginDate != null)
            {
                query = query.Where(t => t.plan_che_date >= cheDDBeginDate && t.plan_che_date <= cheDDBeginDate);
            }
            if(!string.IsNullOrEmpty(status))
            {
                if (status.Equals("finish"))
                {
                    query = query.Where(t => t.is_pass.Equals("Y") && t.plan_che_date != null);
                }else if(status.Equals("revice"))
                {
                    if (!isIE)
                    {
                        query = query.Where(t => t.is_pass == null && t.pro_send_dep == departName);//待接收
                    }
                    else {
                        query = query.Where(t => t.is_pass == null);
                    }
                }
                else if (status.Equals("confirm"))
                {
                    query = query.Where(t => t.imp_finish_date != null);//已确认
                }
                else {
                    query = query.Where(t => t.is_pass.Equals("N"));
                }
            }
            
            if (!isIE)//非IE工程师
            {
                if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(pro_dep) && string.IsNullOrEmpty(pro_send_dep) && planBeginDate == null && cheDDBeginDate == null && string.IsNullOrEmpty(status))//非搜索状态
                {
                    if (isRevice)//接收人
                    {
                        query = query.Where(t => ((t.pro_send_dep == departName&&t.is_pass==null) || t.pro_man == userName) && t.imp_finish_date == null && t.plan_che_date == null);//&&t.is_pass=="Y"
                    }
                    else {
                        query = query.Where(t => t.pro_man == userName && t.imp_finish_date == null && t.plan_che_date == null);//&&t.is_pass=="Y"
                    }
                    
                }
            }
            else {
                if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(pro_dep) && string.IsNullOrEmpty(pro_send_dep) && planBeginDate == null && cheDDBeginDate == null && string.IsNullOrEmpty(status))
                {
                    query = query.Where(t => t.imp_finish_date == null && t.plan_che_date == null);//&&t.is_pass=="Y"
                }
            }
            pagination.records = query.Count();
            query = query.OrderByDescending(q => q.ID).Skip((pagination.page - 1) * pagination.rows).Take(pagination.rows);
            return query.ToList();
        }

        public void DeleteForm(int keyValue)
        {
            service.Delete(t => t.ID == keyValue);
        }

        public void SubmitForm(ProposalEntity proposal, int? keyValue=null)
        {
            try
            {
                if (keyValue != null)
                {
                    proposal.ID = keyValue.Value;
                    service.Update(proposal);
                }
                else
                {
                    service.Insert(proposal);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
