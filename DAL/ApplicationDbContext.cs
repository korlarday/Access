using Allprimetech.Interfaces.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    //public class AppContext : DbContext 
    //{
    //    public AppContext() : base("MysqlConnection")
    //    {
            
    //    }
    //    public static async Task<bool> ExecuteNonQueries(string Queries)
    //    {
    //        try
    //        {
    //            int Exc = 0;
    //            using (AppContext t = new AppContext())
    //            {
    //                Exc = await t.Database.ExecuteSqlCommandAsync(Queries);
    //            }

    //            return (Exc > 0) ? true : false;
    //        }
    //        catch (Exception ex)
    //        {
    //            return false;
    //        }

    //    }


    //}

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
          
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        #region DbSet Models
        public Microsoft.EntityFrameworkCore.DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Customer> Customers { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Partner> Partners { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Production> Productions { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<OrderDetail> OrderDetails { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Order> Orders { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<SystemAudit> SystemAudits { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Group> Groups { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Cylinder> Cylinders { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<VerificationCode> VerificationCodes { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<CylinderGroup> CylinderGroups { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Disc> Discs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<KeyGroupCylinderAnalysis> KeyGroupCylinderAnalyses { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<GroupSummary> GroupSummaries { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<GroupFinal> GroupFinals { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<KeyGroupCylinderDetail> KeyGroupCylinderDetails { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<CylinderGroupsRelation> CylinderGroupsRelations { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<GroupsInfo> GroupsInfos { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<CylinderGroupVerification> CylinderGroupVerifications { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Configuration> Configurations { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Country> Countries { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<GroupInfoVerification> GroupInfoVerifications { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ApplicationUserCustomer> ApplicationUserCustomers { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<OrderValidation> OrderValidations { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<OrderAvailable> OrderAvailables { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region model config
            modelBuilder.Entity<ApplicationUserRole>().HasKey(x =>
                new { x.ApplicationUserId, x.ApplicationRoleID });

            //modelBuilder.Entity<CylinderGroup>().HasKey(x =>
            //    new { x.CylinderID, x.GroupID });

            //modelBuilder.Entity<CylinderGroupVerification>().HasKey(x =>
            //    new { x.CylinderID, x.GroupID });


            //modelBuilder.Entity<SystemAudit>().Property(e => e._Source)
            //.HasConversion(
            //    v => v.ToString(),
            //    v => (Source)Enum.Parse(typeof(Source), v));

            //modelBuilder.Entity<SystemAudit>().Property(e => e._Operation)
            //.HasConversion(
            //    v => v.ToString(),
            //    v => (Operation)Enum.Parse(typeof(Operation), v));
            #endregion


            base.OnModelCreating(modelBuilder);
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            #region CreateDbContext
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(
                                                        Directory.GetCurrentDirectory())
                                                        .AddJsonFile(@Directory.GetCurrentDirectory() + "/../SAP/appsettings.json")
                                                        .Build();
                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                var connectionString = configuration.GetConnectionString("MysqlConnection");
                builder.UseMySql(connectionString);
                return new ApplicationDbContext(builder.Options);
            }

            #endregion
        }

    }
}
