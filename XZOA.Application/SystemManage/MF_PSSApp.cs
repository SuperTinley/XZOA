using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Application.SystemManage
{
   public class MF_PSSApp
    {
        private IMF_PSSRepository service = new Repository.SystemManage.MF_PSSRepository();

        public List<MF_PSSEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public MF_PSSEntity GetForm(string bill,int value)
        {
            MF_PSSEntity  mF= null;
            if (value == 0)
            {
                mF = service.TOOLIQueryable(t => t.PS_NO.Equals(bill)).FirstOrDefault();
            }
            else {
                mF = service.G_WSIQueryable(t => t.PS_NO.Equals(bill)).FirstOrDefault();
            }
            return mF;
        }

        public void SubmitForm(string PS_ID,string billNo,string purSup,int WAREWAY,BillEntity entity=null)
        {
            MF_PSSEntity mF_PSSEntity = new MF_PSSEntity();
            mF_PSSEntity.PS_ID = PS_ID;
            mF_PSSEntity.PS_NO = billNo;
            mF_PSSEntity.PS_DD = DateTime.Now.Date;
            mF_PSSEntity.PAY_DD = DateTime.Now.Date;
            mF_PSSEntity.CHK_DD = DateTime.Now.Date;
            mF_PSSEntity.CUS_NO = "XX1";
            mF_PSSEntity.DEP = "00000000";
            mF_PSSEntity.TAX_ID = "2";
            mF_PSSEntity.ZHANG_ID = "2";
            mF_PSSEntity.EXC_RTO = 1;
            mF_PSSEntity.REM = purSup;
            mF_PSSEntity.PAY_MTH = "5";
            mF_PSSEntity.PAY_DAYS = 1;
            mF_PSSEntity.CHK_DAYS = 30;
            mF_PSSEntity.INT_DAYS = 30;
            mF_PSSEntity.PRT_SW = "Y";
            mF_PSSEntity.USR = "CK06";
            mF_PSSEntity.CHK_MAN = "CK06";
            mF_PSSEntity.CLS_DATE = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            mF_PSSEntity.PRT_DATE = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            mF_PSSEntity.PRT_USR = "CK06";
            mF_PSSEntity.AMT_POI = 2;
            if (PS_ID.Equals("PB"))
            {
                mF_PSSEntity.OS_ID = "PC";
                mF_PSSEntity.OS_NO = entity.billNo;
                mF_PSSEntity.PO_ID = "F";
                mF_PSSEntity.SEND_WH = "00000";
            }
            else if(PS_ID.Equals("PC"))
            {
                mF_PSSEntity.SEND_WH = "0000";
            }
            mF_PSSEntity.LZ_CLS_ID = "F";
            mF_PSSEntity.SYS_DATE = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (WAREWAY == 0)
            {
                service.TOOLInsert(mF_PSSEntity);
            }
            else {
                service.G_WSInsert(mF_PSSEntity);
            }
        }
    }
}
