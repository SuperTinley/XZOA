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
    public class RoleApp
    {
        private IRoleRepository service = new RoleRepository();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public List<RoleEntity> GetList(string keyword = "")
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_OrganizeId.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 1);
            var query = service.IQueryable(expression).OrderBy(t => t.F_SortCode);
            return query.ToList();
        }

        public List<string> GetManList()
        {
            var expression = ExtLinq.True<RoleEntity>();
            expression = expression.And(t => t.F_Category == 1);
            expression = expression.And(t => t.F_FullName.Contains("主任") || t.F_FullName.Contains("IE工程师") || t.F_FullName.Contains("主管") || t.F_FullName.Contains("项目经理") || t.F_FullName.Contains("助理") || t.F_FullName.Contains("厂长"));
            return service.IQueryable(expression).Select(t => t.F_Id).ToList();
        }

        public List<string> GetKeyGridJson(string keyword)
        {
            return service.IQueryable(t => t.F_FullName==keyword).Select(t => t.F_Id).ToList();
        }

        public List<string> GetRoleUser(string Name)
        {
            var roles = service.IQueryable(t=>t.F_FullName.Contains(Name)).Select(t=>t.F_Id).ToList();
            return roles;
        }

        public List<RoleEntity> GetPaginationList(Pagination pagination, string keyword = "")
        {
            var expression = ExtLinq.True<RoleEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_OrganizeId.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 1);
            var query=service.IQueryable(expression).OrderBy(t => t.F_SortCode);
            pagination.records = query.Count();
            return query.ToList();
        }

        public RoleEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }

        public bool IsEmployee(string keyValue)
        {
           var role= service.FindEntity(keyValue);
            if(role!=null&&role.F_FullName!="员工")
            {
                return true;
            }
            return false;
        }

        public void SubmitForm(RoleEntity roleEntity, string[] permissionIds, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.F_Id = keyValue;
            }
            else
            {
                roleEntity.F_Id = Common.GuId();
            }
            var moduledata = moduleApp.GetList();
            var buttondata = moduleButtonApp.GetList();
            List<RoleAuthorizeEntity> roleAuthorizeEntitys = new List<RoleAuthorizeEntity>();
            foreach (var itemId in permissionIds)
            {
                RoleAuthorizeEntity roleAuthorizeEntity = new RoleAuthorizeEntity();
                roleAuthorizeEntity.F_Id = Common.GuId();
                roleAuthorizeEntity.F_ObjectType = 1;
                roleAuthorizeEntity.F_ObjectId = roleEntity.F_Id;
                roleAuthorizeEntity.F_ItemId = itemId;
                if (moduledata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 1;
                }
                if (buttondata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 2;
                }
                roleAuthorizeEntitys.Add(roleAuthorizeEntity);
            }
            service.SubmitForm(roleEntity, roleAuthorizeEntitys, keyValue);
        }
    }
}
