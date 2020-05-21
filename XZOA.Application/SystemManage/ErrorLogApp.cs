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

namespace XZOA.Application.SystemManage
{
    public class ErrorLogApp
    {
        private IErrorLogRepository service = new ErrorLogRepository();

        public void SubmitForm(Exception ex)
        {
            try
            {
                ErrorLogEntity errorLogEntity = new ErrorLogEntity();
                errorLogEntity.Date = DateTime.Now;
                errorLogEntity.Message = ex.Message;
                errorLogEntity.Source = ex.Source;
                errorLogEntity.TargetSite = ex.TargetSite.Name;
                errorLogEntity.StackTrace = ex.StackTrace;
                service.Insert(errorLogEntity);
            }
            catch (Exception ex1)
            {

                throw;
            }
        }
    }
}
