using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Application.SystemManage
{
   public class TF_PSSApp
    {
        private ITF_PSSRepository service = new Repository.SystemManage.TF_PSSRepository();

        public List<TF_PSSEntity> GetList()
        {
            var query = service.IQueryable();
            return query.ToList();
        }

        public TF_PSSEntity GetForm(string PS_NO, int? preItem)
        {
            var query = service.IQueryable(t=>t.PRE_ITM==preItem&&t.PS_NO.Equals(PS_NO)).FirstOrDefault();
            return query;
        }

        public int Update(TF_PSSEntity tF_PSSEntity,int value)
        {
            if (value == 0)
            {
                return service.TOOLUpdate(tF_PSSEntity);
            }
            else { return service.G_WSUpdate(tF_PSSEntity); }
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PS_ID"></param>
        /// <param name="billNo"></param>
        /// <param name="WH"></param>
        /// <param name="ITM"></param>
        /// <param name="NAME"></param>
        /// <param name="applyBillEntity"></param>
        /// <param name="billEntity"></param>
        /// <param name="priceEntity"></param>
        /// <returns></returns>
        public TF_PSSEntity SubmitForm(string PS_ID,string billNo, string WH, BillEntity bill,string NAME, ApplyBillEntity applyBillEntity,BillEntity billEntity,PriceEntity priceEntity)
        {
            try
            {
                TF_PSSEntity tF_PSSEntity = new TF_PSSEntity();
                TF_PSSEntity entity = null;
                TF_PSSEntity entity1 = null;
                if (applyBillEntity.WAREWAY == 0)//工具仓
                {
                    entity = service.TOOLIQueryable(t => t.PS_NO.Equals(billNo)).OrderByDescending(t => t.ITM).FirstOrDefault();
                    entity1 = service.TOOLIQueryable(t => t.PS_NO.Equals(billNo)).OrderByDescending(t => t.PRE_ITM).FirstOrDefault();
                }
                else
                {
                    entity = service.G_WSIQueryable(t => t.PS_NO.Equals(billNo)).OrderByDescending(t => t.ITM).FirstOrDefault();
                    entity1 = service.G_WSIQueryable(t => t.PS_NO.Equals(billNo)).OrderByDescending(t => t.PRE_ITM).FirstOrDefault();
                }

                if (entity != null)
                {
                    tF_PSSEntity.ITM = entity.ITM + 1;
                }
                else { tF_PSSEntity.ITM = 1; }
                if (entity1 != null)
                {
                    tF_PSSEntity.PRE_ITM = entity1.PRE_ITM + 1;
                }
                else { tF_PSSEntity.PRE_ITM = 1; }
                tF_PSSEntity.PS_ID = PS_ID;
                tF_PSSEntity.PS_NO = billNo;
                tF_PSSEntity.PS_DD = DateTime.Now.Date;
                tF_PSSEntity.WH = WH;
                tF_PSSEntity.PRD_NO =applyBillEntity.prdNo;
                tF_PSSEntity.PRD_NAME = NAME;
                tF_PSSEntity.PRD_MARK = "";
                tF_PSSEntity.UNIT = "1";
                tF_PSSEntity.QTY = Math.Abs(billEntity.num??0);
                #region 单位及数量转换
                if (priceEntity.unit == applyBillEntity.appUnit)
                {
                    tF_PSSEntity.UP = priceEntity.price;
                    tF_PSSEntity.AMT = decimal.Round(applyBillEntity.purPrice.Value * Math.Abs(billEntity.num.Value), 2);
                    tF_PSSEntity.AMTN_NET = decimal.Round((applyBillEntity.purPrice.Value * Math.Abs(billEntity.num.Value)) / (1 + priceEntity.principal.Value), 2);
                    tF_PSSEntity.CSTN_SAL = tF_PSSEntity.AMTN_NET;
                    tF_PSSEntity.TAX = tF_PSSEntity.AMT - tF_PSSEntity.AMTN_NET;
                }
                else
                {
                    var up = Math.Round((billEntity.viceNum ?? 0) * (priceEntity.price ?? 0) / (Math.Abs(billEntity.num ?? 0)), 2);
                    tF_PSSEntity.UP = up;
                    if (PS_ID.Equals("PB"))
                    {
                        tF_PSSEntity.UP_QTY1 = billEntity.num;
                    }
                    tF_PSSEntity.AMT = decimal.Round(up * Math.Abs(billEntity.num.Value), 2);
                    tF_PSSEntity.AMTN_NET = decimal.Round((up * Math.Abs(billEntity.num.Value)) / (1 + priceEntity.principal.Value), 2);
                    tF_PSSEntity.CSTN_SAL = tF_PSSEntity.AMTN_NET;
                    tF_PSSEntity.TAX = tF_PSSEntity.AMT - tF_PSSEntity.AMTN_NET;
                }
                #endregion
                if (PS_ID.Equals("PB"))//退货单
                {
                    tF_PSSEntity.OS_ID = "PC";//来源单ID
                    tF_PSSEntity.OS_NO = bill.billNo;
                    tF_PSSEntity.OTH_ITM = bill.erpItm;
                    tF_PSSEntity.CUS_OS_NO = applyBillEntity.purNo;
                    tF_PSSEntity.SL_ITM = 0;
                    tF_PSSEntity.BL_OS_ITM = 0;
                }else if(PS_ID.Equals("PC"))
                {
                    tF_PSSEntity.FREE_ID_DEF = "F";
                }
                
                tF_PSSEntity.TAX_RTO = priceEntity.principal * 100;
                tF_PSSEntity.QTY1 = billEntity.viceNum;
                tF_PSSEntity.REM = billEntity.remark;
                tF_PSSEntity.PAK_WEIGHT_UNIT = applyBillEntity.viceUnit;
                tF_PSSEntity.CUS_OS_NO = applyBillEntity.purNo;

                if (applyBillEntity.WAREWAY == 0)
                {
                    service.TOOLInsert(tF_PSSEntity);
                }
                else
                {
                    service.G_WSInsert(tF_PSSEntity);
                }
                return tF_PSSEntity;
            }
            catch (Exception ex)
            {

                new ErrorLogApp().SubmitForm(ex);
                return null;
            }
            
        }
    }
}
