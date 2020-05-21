using System;
using XZOA.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace XZOA.Mapping.SystemManage
{
   public class LeaveMap : EntityTypeConfiguration<LeaveEntity>
    {
        public LeaveMap()
        {
            this.ToTable("Sys_Leave");
            this.HasKey(t => t.F_Id);
        }
    }
}
