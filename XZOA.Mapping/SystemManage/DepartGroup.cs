using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZFine.Domain.Entity.SystemManage;
using System.Data.Entity.ModelConfiguration;

namespace ZFine.Mapping.SystemManage
{
    class DepartGroup : EntityTypeConfiguration<DepartGroupEntity>
    {
        public DepartGroup()
        {
            this.ToTable("Sys_DepartGroup");
            this.HasKey(t => t.F_Id);
        }
    }
}
