using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Application.SystemManage
{
   public class PRDT1App
    {
        private IPRDT1Repository service = new Repository.SystemManage.PRDT1Repository();

        public List<PRDT1Entity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public void Update(PRDT1Entity pRDT1Entity,int value)
        {
            if (value == 0)
            {
                service.TOOLUpdate(pRDT1Entity);
            }
            else {
                service.G_WSUpdate(pRDT1Entity);
            }
           
        }

        public void Insert(PRDT1Entity pRDT1Entity,int value)
        {
           
            try
            {
                if (value == 0)
                {
                    service.TOOLInsert(pRDT1Entity);
                }
                else
                {
                    service.G_WSInsert(pRDT1Entity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
