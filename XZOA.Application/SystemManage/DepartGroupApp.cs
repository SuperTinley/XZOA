using ZFine.Code;
using ZFine.Domain.Entity.SystemManage;
using ZFine.Domain.IRepository.SystemManage;
using ZFine.Repository.SystemManage;
using System.Collections.Generic;
using System.Linq;

namespace ZFine.Application.SystemManage
{
   public class DepartGroupApp
    {
        private IDepartGroupRepository service = new DepartGroupRepository();

        public List<DepartGroupEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

    }
}
