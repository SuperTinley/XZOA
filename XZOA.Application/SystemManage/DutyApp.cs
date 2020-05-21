/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;
using System.Collections.Generic;
using System.Linq;

namespace XZOA.Application.SystemManage
{
    public class DutyApp
    {
        private IRoleRepository service = new RoleRepository();

        public List<RoleEntity> GetList(string keyword = "")
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 2);
            var query= service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
            return query.ToList();
        }

        public List<RoleEntity> GetPaginationList(Pagination pagination, string keyword = "")
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 2);
            var query = service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
            pagination.records = query.Count();
            return query.ToList();
        }

        public List<string> GetCheckList()
        {
            var expression = ExtLinq.True<RoleEntity>();
            expression = expression.And(t => t.F_Category == 1);
            expression = expression.And(t => t.F_FullName.Contains("主任") || t.F_FullName.Contains("主管") || t.F_FullName.Contains("助理") || t.F_FullName.Contains("厂长") || t.F_FullName.Contains("总经理"));
            return service.IQueryable(expression).Select(t=>t.F_Id).ToList();
        }

        public List<string> GetApplyCheckList()
        {
            var expression = ExtLinq.True<RoleEntity>();
            expression = expression.And(t => t.F_Category == 1);
            expression = expression.And(t => t.F_FullName.Contains("主任") || t.F_FullName.Contains("主管") || t.F_FullName.Contains("助理") || t.F_FullName.Contains("厂长") || t.F_FullName.Contains("总经理"));
            return service.IQueryable(expression).Select(t => t.F_Id).ToList();
        }

        public List<string> GetTempCheckList()
        {
            var expression = ExtLinq.True<RoleEntity>();
            expression = expression.And(t => t.F_Category == 1);
            expression = expression.And(t => t.F_FullName.Contains("主任") || t.F_FullName.Contains("主管") || t.F_FullName.Contains("IE工程师") || t.F_FullName.Contains("助理") || t.F_FullName.Contains("项目经理") || t.F_FullName.Contains("厂长") || t.F_FullName.Contains("总经理"));
            return service.IQueryable(expression).Select(t => t.F_Id).ToList();
        }

        public List<string> GetApprovalList(string keyValue=null)
        {
            var expression = ExtLinq.True<RoleEntity>();
            expression = expression.And(t => t.F_Category == 1);
            if (string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t=>t.F_FullName.Contains("厂长"));
            }
            else {
                expression = expression.And(t => t.F_FullName.Contains("人事主任") || t.F_FullName.Contains("厂长") || t.F_FullName.Contains("厂长助理"));
            }
           
            return service.IQueryable(expression).Select(t => t.F_Id).ToList();
        }

        public RoleEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.F_Id == keyValue);
        }
        public void SubmitForm(RoleEntity roleEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.Modify(keyValue);
                service.Update(roleEntity);
            }
            else
            {
                roleEntity.Create();
                roleEntity.F_Category = 2;
                service.Insert(roleEntity);
            }
        }
    }
}
