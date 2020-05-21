/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using System;
using XZOA.Code;
using XZOA.Data;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;

namespace XZOA.Repository.SystemManage
{
    public class ProjectRepository : RepositoryBase<ProjectEntity>, IProjectRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<ProjectEntity>(t => t.F_Id == keyValue);
                db.Commit();
            }
        }
        public void SubmitForm(ProjectEntity projectEntity,string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    projectEntity.F_Id = keyValue;
                    db.Update(projectEntity);
                }
                else
                {
                    db.Insert(projectEntity);
                }
                db.Commit();
            }
        }
    }
}
