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
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using XZOA.Code;

namespace XZOA.Application.SystemManage
{
    public class TempChargeApp
    {
        private ITempChargeRepository service = new TempChargeRepository();

        public TempChargeEntity GetForm(int keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public TempChargeEntity GetForm(string keyValue)
        {
            return service.IQueryable(t=>t.RATIFY_MAN.Equals(keyValue)).FirstOrDefault();
        }

        public List<TempChargeEntity> GetTempCheckList()
        {
            try
            {
                var userName = OperatorProvider.Provider.GetCurrent().UserName;
                var query = service.IQueryable();
                return query.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void SubmitForm(TempChargeEntity templateEntity, int? keyValue = null)
        {
            try
            {
                if (keyValue!=null)
                {
                    templateEntity.ID = keyValue.Value;
                    service.Update(templateEntity);
                }
                else
                {
                    service.Insert(templateEntity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void DeleteForm(int keyValue)
        {
            service.Delete(t => t.ID == keyValue);
        }
    }
}
