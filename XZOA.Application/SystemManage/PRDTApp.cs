using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Application.SystemManage
{
   public class PRDTApp
    {
        private IPRDTRepository service = new Repository.SystemManage.PRDTRepository();

        public List<PRDTEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }
    }
}
