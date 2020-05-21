/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using XZOA.Code;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.IRepository.SystemManage;
using XZOA.Domain.ViewModel;
using XZOA.Repository.SystemManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace XZOA.Application.SystemManage
{
    public class RoleAuthorizeApp
    {
        private IRoleAuthorizeRepository service = new RoleAuthorizeRepository();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public List<RoleAuthorizeEntity> GetList(string ObjectId)
        {
            return service.IQueryable(t => t.F_ObjectId == ObjectId).ToList();
        }
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<ModuleEntity> GetMenuList(string roleId)
        {
            var data = new List<ModuleEntity>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleApp.GetList();
            }
            else
            {
                var moduledata = moduleApp.GetList();
                var authorizedata = service.IQueryable(t => t.F_ObjectId == roleId && t.F_ItemType == 1).ToList();
                foreach (var item in authorizedata)
                {
                    ModuleEntity moduleEntity = moduledata.Find(t => t.F_Id == item.F_ItemId);
                    if (moduleEntity != null)
                    {
                        data.Add(moduleEntity);
                    }
                }
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }
        /// <summary>
        /// 获取按钮列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public List<ModuleButtonEntity> GetButtonList(string roleId)
        {
            var data = new List<ModuleButtonEntity>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleButtonApp.GetList();
            }
            else
            {
                var buttondata = moduleButtonApp.GetList();
                var authorizedata = service.IQueryable(t => t.F_ObjectId == roleId && t.F_ItemType == 2).ToList();
                foreach (var item in authorizedata)
                {
                    ModuleButtonEntity moduleButtonEntity = buttondata.Find(t => t.F_Id == item.F_ItemId);
                    if (moduleButtonEntity != null)
                    {
                        data.Add(moduleButtonEntity);
                    }
                }
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }
        public bool ActionValidate(string roleId, string moduleId, string action)
        {
            
            try
            {
                var authorizeurldata = new List<AuthorizeActionModel>();
                var cachedata = CacheFactory.Cache().GetCache<List<AuthorizeActionModel>>("authorizeurldata_" + roleId);
                if (cachedata == null)
                {
                    var moduledata = moduleApp.GetList();
                    var buttondata = moduleButtonApp.GetList();
                    var authorizedata = service.IQueryable(t => t.F_ObjectId == roleId).ToList();
                    foreach (var item in authorizedata)
                    {
                        if (item.F_ItemType == 1)//菜单
                        {
                            ModuleEntity moduleEntity = moduledata.Find(t => t.F_Id == item.F_ItemId);
                            if(moduleEntity!=null)
                            {
                                authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleEntity.F_Id, F_UrlAddress = moduleEntity.F_UrlAddress });
                            }
                            
                        }
                        else if (item.F_ItemType == 2)//按钮
                        {
                            ModuleButtonEntity moduleButtonEntity = buttondata.Find(t => t.F_Id == item.F_ItemId);
                            if(moduleButtonEntity!=null)
                            {
                                authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleButtonEntity.F_ModuleId, F_UrlAddress = moduleButtonEntity.F_UrlAddress });
                            }
                            
                        }
                    }
                    CacheFactory.Cache().WriteCache(authorizeurldata, "authorizeurldata_" + roleId, DateTime.Now.AddMinutes(5));
                }
                else
                {
                    authorizeurldata = cachedata;
                }
                authorizeurldata = authorizeurldata.FindAll(t => t.F_Id.Equals(moduleId));
                foreach (var item in authorizeurldata)
                {
                    if (!string.IsNullOrEmpty(item.F_UrlAddress))
                    {
                        string[] url = item.F_UrlAddress.Split('?');
                        if (item.F_Id == moduleId && url[0] == action)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
