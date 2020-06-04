using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Repository.SystemManage;

namespace XZOA.Application.SystemManage
{
    public class ProjectApp
    {
        private IProjectRepository service = new ProjectRepository();

        public List<ProjectEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public ProjectEntity GetForm(string keyValue)
        {
            var query = service.IQueryable(t => t.F_Id == keyValue);
            return query.FirstOrDefault();
        }

        public void DeleteForm(string keyValue)
        {
            service.Delete(t => t.F_Id == keyValue);
        }

        public void SubmitForm(ProjectEntity projectEntity, string keyValue = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    projectEntity.F_Id = keyValue;
                    service.Update(projectEntity);
                }
                else
                {
                    projectEntity.F_Id = Guid.NewGuid().ToString();
                    service.Insert(projectEntity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
