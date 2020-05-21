/*******************************************************************************
 * Copyright © 2016
 * 
 * Description: 雄智供应链平台  
 *
*********************************************************************************/
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using XZOA.Domain.Entity.SystemManage;
using XZOA.Domain.Entity.SystemSecurity;

namespace XZOA.Data
{
    public class XZOADbContext : DbContext
    {
        public XZOADbContext()
            : base("XZOADbContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;//通过 DbContext 和相关类的方法自动调用 DetectChanges() 方法
            this.Configuration.ValidateOnSaveEnabled = false;//在调用 SaveChanges() 时，是否应自动验证所跟踪的实体
            this.Configuration.LazyLoadingEnabled = false;//是否启用针对公开为导航属性的关系的延迟加载
            this.Configuration.ProxyCreationEnabled = false; //框架在创建实体类型的实例时是否会创建动态生成的代理类的实例
        }
        public DbSet<UserEntity> UserEntity { get; set; }

        public DbSet<UserLogOnEntity> UserLogOnEntity { get; set; }

        public DbSet<LeaveEntity> LeaveEntity { get; set; }

        public DbSet<AreaEntity> AreaEntity { get; set; }

        public DbSet<ItemsDetailEntity> ItemsDetailEntity { get; set; }

        public DbSet<ItemsEntity> ItemsEntity { get; set; }

        public DbSet<ModuleButtonEntity> ModuleButtonEntity { get; set; }

        public DbSet<ModuleEntity> ModuleEntity { get; set; }

        public DbSet<OrganizeEntity> OrganizeEntity { get; set; }

        public DbSet<RoleAuthorizeEntity> RoleAuthorizeEntity { get; set; }

        public DbSet<RoleEntity> RoleEntity { get; set; }

        public DbSet<LogEntity> LogEntity { get; set; }

        public DbSet<DbBackupEntity> DbBackupEntity { get; set; }

        public DbSet<FilterIPEntity> FilterIPEntity { get; set; }

        public DbSet<PriceEntity> PriceEntity { get; set; }

        public DbSet<AppointManEntity> AppointManEntity { get; set; }

        public DbSet<BillEntity> BillEntity { get; set; }

        public DbSet<ErrorLogEntity> ErrorLogEntity { get; set; }

        public DbSet<TemplateEntity> TemplateEntity { get; set; }

        public DbSet<TempChargeEntity> TempChargeEntity { get; set; }

        public DbSet<MaterialPriceEntity> MaterialPriceEntity { get; set; }
        
        public DbSet<BookMeetingEntity> BookMeetingEntity { get; set; }
        public DbSet<MeetRoomEntity> MeetRoomEntity { get; set; }

        public DbSet<ProposalEntity> ProposalEntity { get; set; }

        public DbSet<BookEntity> BookEntity { get; set; }
        public DbSet<TypeEntity> TypeEntity { get; set; }
        public DbSet<ProjectEntity> ProjectEntity { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //string assembleFileName = Assembly.GetExecutingAssembly().CodeBase.Replace("XZOA.Data.dll", "XZOA.Mapping.dll").Replace("file:///", "");
            //Assembly asm = Assembly.LoadFile(assembleFileName);
            //var typesToRegister = asm.GetTypes()
            //.Where(type => !String.IsNullOrEmpty(type.Namespace))
            //.Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>));
            //foreach (var type in typesToRegister)
            //{
            //    dynamic configurationInstance = Activator.CreateInstance(type);
            //    modelBuilder.Configurations.Add(configurationInstance);
            //}

            modelBuilder.Entity<RoleEntity>().ToTable("Sys_Role", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<UserEntity>().ToTable("Sys_User", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<UserLogOnEntity>().ToTable("Sys_UserLogOn", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<LeaveEntity>().ToTable("Sys_Leave", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<RoleAuthorizeEntity>().ToTable("Sys_RoleAuthorize", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<ModuleButtonEntity>().ToTable("Sys_ModuleButton", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<ModuleEntity>().ToTable("Sys_Module", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<OrganizeEntity>().ToTable("Sys_Organize", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<ItemsDetailEntity>().ToTable("Sys_ItemsDetail", "dbo").HasKey(u=>u.F_Id);
            modelBuilder.Entity<ItemsEntity>().ToTable("Sys_Items", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<AreaEntity>().ToTable("Sys_Area", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<LogEntity>().ToTable("Sys_Log", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<DbBackupEntity>().ToTable("Sys_DbBackup", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<FilterIPEntity>().ToTable("Sys_FilterIP", "dbo").HasKey(u => u.F_Id);
            modelBuilder.Entity<ApplyBillEntity>().ToTable("Sys_ApplyBill", "dbo").HasKey(u => u.ID).Property(user => user.purPrice).HasPrecision(30, 20);
            modelBuilder.Entity<ApplyBillEntity>().Property(user => user.prePrice).HasPrecision(30, 20);
            modelBuilder.Entity<BillEntity>().ToTable("Sys_Bill", "dbo").HasKey(u => u.id);
            modelBuilder.Entity<PriceEntity>().ToTable("Sys_Price", "dbo").HasKey(u=>u.priNO).Property(p => p.price).HasPrecision(30, 20);
            modelBuilder.Entity<AppointManEntity>().ToTable("Sys_AppointMan", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<ErrorLogEntity>().ToTable("Sys_ErrorLog", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<TemplateEntity>().ToTable("Sys_Template", "dbo").HasKey(u => u.TEM_ID);
            modelBuilder.Entity<TempChargeEntity>().ToTable("Sys_TempCharge", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<MaterialPriceEntity>().ToTable("Sys_MaterialPrice", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<BookMeetingEntity>().ToTable("Sys_BookMeeting", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<MeetRoomEntity>().ToTable("Sys_MeetRoom", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<ProposalEntity>().ToTable("Sys_Proposal", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<BookEntity>().ToTable("Sys_Book", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<BookTypeEntity>().ToTable("Sys_BookType", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<TypeEntity>().ToTable("Sys_Type", "dbo").HasKey(u => u.ID);
            modelBuilder.Entity<ProjectEntity>().ToTable("Sys_Project", "dbo").HasKey(u => u.F_Id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
