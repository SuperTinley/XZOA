﻿/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using System;

namespace XZOA.Domain.Entity.SystemManage
{
    public class RoleEntity : IEntity<RoleEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_OrganizeId { get; set; }
        //分类:1-角色2-岗位
        public int? F_Category { get; set; }
        //编号
        public string F_EnCode { get; set; }
        public string F_FullName { get; set; }
        //类型
        public string F_Type { get; set; }
        public bool? F_AllowEdit { get; set; }
        public bool? F_AllowDelete { get; set; }
        public int? F_SortCode { get; set; }
        public bool? F_DeleteMark { get; set; }
        public bool? F_EnabledMark { get; set; }
        public string F_Description { get; set; }
        public DateTime? F_CreatorTime { get; set; }
        public string F_CreatorUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_DeleteUserId { get; set; }
    }
}