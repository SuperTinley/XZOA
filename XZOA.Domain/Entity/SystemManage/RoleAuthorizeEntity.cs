/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using System;

namespace XZOA.Domain.Entity.SystemManage
{
    public class RoleAuthorizeEntity : IEntity<RoleAuthorizeEntity>, ICreationAudited
    {
        public string F_Id { get; set; }
        //模块类型1-模块2-按钮3-列表
        public int? F_ItemType { get; set; }
        //模块主键
        public string F_ItemId { get; set; }
        //对象分类1-角色2-部门-3用户
        public int? F_ObjectType { get; set; }
        //对象主键
        public string F_ObjectId { get; set; }
        public int? F_SortCode { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
    }
}
