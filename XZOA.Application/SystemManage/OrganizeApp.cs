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

namespace XZOA.Application.SystemManage
{
    public class OrganizeApp
    {
        private IOrganizeRepository service = new OrganizeRepository();
        
        public List<OrganizeEntity> GetList()
        {
            return service.IQueryable().OrderBy(t => t.F_CreatorTime).ToList();
        }

        public List<OrganizeEntity> GetDepartList()
        {
            return service.IQueryable(t=>t.F_CategoryId.Equals("Department")).ToList();//获取部门
        }

        public OrganizeEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public bool IsAdministration(string keyValue)
        {
            if(!string.IsNullOrEmpty(keyValue))
            {
                var data= service.FindEntity(keyValue);
                if(data.F_FullName.Contains("人事行政部"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取同分组的部门
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<string> getDepartByGroup(string groupId)
        {
            return service.IQueryable(t => t.F_DepartGroupId == groupId).Select(t => t.F_Id).ToList();
        }

        public List<string> getDepartByName(string keyValue)
        {
            return service.IQueryable(t => t.F_FullName.Contains(keyValue)).Select(t => t.F_Id).ToList();
        }

        public void DeleteForm(string keyValue)
        {
            if (service.IQueryable().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                service.Delete(t => t.F_Id == keyValue);
            }
        }
        public void SubmitForm(OrganizeEntity organizeEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                organizeEntity.Modify(keyValue);
                service.Update(organizeEntity);
            }
            else
            {
                organizeEntity.Create();
                service.Insert(organizeEntity);
            }
        }
    }
}
