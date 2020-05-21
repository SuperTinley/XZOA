/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Data;
using XZOA.Domain.Entity.SystemManage;
using System.Collections.Generic;

namespace XZOA.Domain.IRepository.SystemManage
{
    public interface IItemsDetailRepository : IRepositoryBase<ItemsDetailEntity>
    {
        List<ItemsDetailEntity> GetItemList(string enCode);
    }
}
