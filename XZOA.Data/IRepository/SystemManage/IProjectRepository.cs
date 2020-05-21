/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using System;
using XZOA.Data;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Domain.IRepository.SystemManage
{
    public interface IProjectRepository : IRepositoryBase<ProjectEntity>
    {
        void DeleteForm(string keyValue);
        void SubmitForm(ProjectEntity projectEntity,  string keyValue);
    }
}
