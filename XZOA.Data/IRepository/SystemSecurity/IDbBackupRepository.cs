/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Data;
using XZOA.Domain.Entity.SystemSecurity;

namespace XZOA.Domain.IRepository.SystemSecurity
{
    public interface IDbBackupRepository : IRepositoryBase<DbBackupEntity>
    {
        void DeleteForm(string keyValue);
        void ExecuteDbBackup(DbBackupEntity dbBackupEntity);
    }
}
