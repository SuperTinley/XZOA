using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Application.SystemManage
{
   public class AppointManApp
    {
        private IAppointManRepository service = new Repository.SystemManage.AppointManRepository();

        public AppointManEntity GetForm()
        {
            return service.IQueryable(t => t.tag.Equals("Y")).FirstOrDefault();
        }

        public List<AppointManEntity> GetList()
        {
            return service.IQueryable(t => t.tag.Equals("Y")).ToList();
        }

        public List<AppointManEntity> GetALLList()
        {
            return service.IQueryable().ToList();
        }

        public AppointManEntity SubmitForm(int ID)
        {
            var old = service.FindEntity(t=>t.tag=="Y");
            old.tag = "N";
            var man = service.FindEntity(t=>t.ID==ID);
            man.tag = "Y";
            service.Update(old);
            service.Update(man);
            return man;
        }
    }
}
