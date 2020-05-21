using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XZOA.Domain.Entity.SystemManage;

namespace XZOA.Data.DBContext
{
   public class TOOLDbContext:DbContext
    {
        public TOOLDbContext()
           : base("TOOLDbContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;//通过 DbContext 和相关类的方法自动调用 DetectChanges() 方法
            this.Configuration.ValidateOnSaveEnabled = false;//在调用 SaveChanges() 时，是否应自动验证所跟踪的实体
            this.Configuration.LazyLoadingEnabled = false;//是否启用针对公开为导航属性的关系的延迟加载
            this.Configuration.ProxyCreationEnabled = false; //框架在创建实体类型的实例时是否会创建动态生成的代理类的实例
        }

        public DbSet<PRDTEntity> PRDTEntity { get; set; }

        public DbSet<PRDT1Entity> PRDT1Entity { get; set; }

        public DbSet<TF_PSSEntity> TF_PSSEntity { get; set; }

        public DbSet<MF_PSSEntity> MF_PSSEntity { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PRDTEntity>().ToTable("PRDT", "dbo").HasKey(u => u.PRD_NO);
            modelBuilder.Entity<PRDT1Entity>().ToTable("PRDT1", "dbo").HasKey(u => new { u.WH , u.PRD_NO ,u.PRD_MARK } );
            modelBuilder.Entity<TF_PSSEntity>().ToTable("TF_PSS", "dbo").HasKey(u => new { u.PS_ID,u.PS_NO,u.ITM } ).Property(p => p.UP).HasPrecision(30, 20);
            modelBuilder.Entity<MF_PSSEntity>().ToTable("MF_PSS", "dbo").HasKey(u => new { u.PS_ID, u.PS_NO } ) ;
            base.OnModelCreating(modelBuilder);
        }


    }
}
