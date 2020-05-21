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
    public class MaterialPriceApp
    {
        private IMaterialPriceRepository service = new MaterialPriceRepository();

        public MaterialPriceEntity GetForm(int keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public MaterialPriceEntity GetForm(string keyValue)
        {
            return service.IQueryable(t=>t.MATERIAL.Equals(keyValue)).FirstOrDefault();
        }

    }
}
