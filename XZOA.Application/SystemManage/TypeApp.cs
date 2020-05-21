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
    public class TypeApp
    {
        private ITypeRepository service = new TypeRepository();

        public List<TypeEntity> GetList()
        {
            return service.IQueryable().ToList();
        }
       
    }
}
