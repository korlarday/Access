// <auto-generated />
using System;
using Allprimetech.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Allprimetech.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210217193425_AddingReclaimedKeyToCylinder")]
    partial class AddingReclaimedKeyToCylinder
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Allprimetech.Interfaces.Models.ApplicationRole", b =>
                {
                    b.Property<int>("ApplicationRoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("_RoleName")
                        .HasColumnType("double");

                    b.HasKey("ApplicationRoleID");

                    b.ToTable("ApplicationRoles");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<int?>("PartnerID")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("RegisteredById")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<DateTime>("_DateRegistered")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("_FirstName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("_LastLogin")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("_LastName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.HasIndex("RegisteredById");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.ApplicationUserRole", b =>
                {
                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("ApplicationRoleID")
                        .HasColumnType("int");

                    b.HasKey("ApplicationUserId", "ApplicationRoleID");

                    b.HasIndex("ApplicationRoleID");

                    b.ToTable("ApplicationUserRoles");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Customer", b =>
                {
                    b.Property<int>("CustomerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int?>("PartnerID")
                        .HasColumnType("int");

                    b.Property<string>("_ContactPerson")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("_CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("_CustomerNumber")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("_InstallationCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("_Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("_SystemCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("_UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("CustomerID");

                    b.HasIndex("CreatedById");

                    b.HasIndex("PartnerID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Cylinder", b =>
                {
                    b.Property<int>("CylinderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<int>("_ArticleNumber")
                        .HasColumnType("int");

                    b.Property<int>("_Assembled")
                        .HasColumnType("int");

                    b.Property<int>("_Blocked")
                        .HasColumnType("int");

                    b.Property<int>("_Color")
                        .HasColumnType("int");

                    b.Property<DateTime>("_CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_Cylinder")
                        .HasColumnType("int");

                    b.Property<string>("_DoorName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_LengthInside")
                        .HasColumnType("int");

                    b.Property<int>("_LengthOutside")
                        .HasColumnType("int");

                    b.Property<int>("_Options")
                        .HasColumnType("int");

                    b.Property<int>("_PositionId")
                        .HasColumnType("int");

                    b.Property<int>("_QRCodeIssued")
                        .HasColumnType("int");

                    b.Property<int>("_Quantity")
                        .HasColumnType("int");

                    b.Property<int>("_Reclaimed")
                        .HasColumnType("int");

                    b.Property<DateTime>("_UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_Validated")
                        .HasColumnType("int");

                    b.HasKey("CylinderID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("OrderID");

                    b.ToTable("Cylinders");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.CylinderGroup", b =>
                {
                    b.Property<int>("CylinderID")
                        .HasColumnType("int");

                    b.Property<int>("GroupID")
                        .HasColumnType("int");

                    b.HasKey("CylinderID", "GroupID");

                    b.HasIndex("GroupID");

                    b.ToTable("CylinderGroups");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Disc", b =>
                {
                    b.Property<int>("DiscID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CylinderID")
                        .HasColumnType("int");

                    b.Property<int>("_Genre")
                        .HasColumnType("int");

                    b.Property<string>("_Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_Number")
                        .HasColumnType("int");

                    b.Property<int>("_Slot")
                        .HasColumnType("int");

                    b.Property<int>("_Type")
                        .HasColumnType("int");

                    b.HasKey("DiscID");

                    b.HasIndex("CylinderID");

                    b.ToTable("Discs");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Group", b =>
                {
                    b.Property<int>("GroupID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<int>("_Blocked")
                        .HasColumnType("int");

                    b.Property<DateTime>("_CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_KeyNumber")
                        .HasColumnType("int");

                    b.Property<string>("_Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_PositionId")
                        .HasColumnType("int");

                    b.Property<int>("_Produced")
                        .HasColumnType("int");

                    b.Property<int>("_Quantity")
                        .HasColumnType("int");

                    b.Property<int>("_Reclaimed")
                        .HasColumnType("int");

                    b.Property<DateTime>("_UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_Validated")
                        .HasColumnType("int");

                    b.HasKey("GroupID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("OrderID");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CreatedById")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("CustomerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("_CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_CylinderQuantity")
                        .HasColumnType("int");

                    b.Property<string>("_Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_KeyQuantity")
                        .HasColumnType("int");

                    b.Property<int>("_OrderNumber")
                        .HasColumnType("int");

                    b.Property<string>("_ProjectName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("_UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("OrderID");

                    b.HasIndex("CreatedById");

                    b.HasIndex("CustomerID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.OrderDetail", b =>
                {
                    b.Property<int>("OrderDetailID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ByPersonId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("_Date")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_NewQty")
                        .HasColumnType("int");

                    b.Property<string>("_Notes")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_OldQty")
                        .HasColumnType("int");

                    b.Property<int>("_ProductID")
                        .HasColumnType("int");

                    b.Property<int>("_ProductType")
                        .HasColumnType("int");

                    b.HasKey("OrderDetailID");

                    b.HasIndex("ByPersonId");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Partner", b =>
                {
                    b.Property<int>("PartnerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("_CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("_Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_PartnerNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("_UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("PartnerID");

                    b.ToTable("Partners");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Production", b =>
                {
                    b.Property<int>("ProductionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ByUserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<int>("OrderID")
                        .HasColumnType("int");

                    b.Property<DateTime>("_CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("_ProductID")
                        .HasColumnType("int");

                    b.Property<int>("_ProductType")
                        .HasColumnType("int");

                    b.Property<int>("_Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("_UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ProductionID");

                    b.HasIndex("ByUserId");

                    b.ToTable("Productions");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.SystemAudit", b =>
                {
                    b.Property<int>("SystemAuditID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("OperatorId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("_Date")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("_Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("_Operation")
                        .HasColumnType("int");

                    b.Property<int>("_Source")
                        .HasColumnType("int");

                    b.HasKey("SystemAuditID");

                    b.HasIndex("OperatorId");

                    b.ToTable("SystemAudits");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.VerificationCode", b =>
                {
                    b.Property<int>("VerificationCodeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("Expired")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("RetryCount")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("VerificationCodeID");

                    b.ToTable("VerificationCodes");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("varchar(256) CHARACTER SET utf8mb4")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Value")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.ApplicationUser", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "RegisteredBy")
                        .WithMany()
                        .HasForeignKey("RegisteredById");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.ApplicationUserRole", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationRole", "ApplicationRole")
                        .WithMany("_Users")
                        .HasForeignKey("ApplicationRoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "ApplicationUser")
                        .WithMany("_Roles")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Customer", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Allprimetech.Interfaces.Models.Partner", "Partner")
                        .WithMany("_Customers")
                        .HasForeignKey("PartnerID");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Cylinder", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allprimetech.Interfaces.Models.Order", "Order")
                        .WithMany("_Cylinders")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.CylinderGroup", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.Cylinder", "Cylinder")
                        .WithMany()
                        .HasForeignKey("CylinderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allprimetech.Interfaces.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Disc", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.Cylinder", "Cylinder")
                        .WithMany()
                        .HasForeignKey("CylinderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Group", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allprimetech.Interfaces.Models.Order", "Order")
                        .WithMany("_Groups")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Order", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Allprimetech.Interfaces.Models.Customer", "Customer")
                        .WithMany("_Orders")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.OrderDetail", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "ByPerson")
                        .WithMany()
                        .HasForeignKey("ByPersonId");

                    b.HasOne("Allprimetech.Interfaces.Models.Order", null)
                        .WithMany("_OrderDetails")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.Production", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "ByUser")
                        .WithMany()
                        .HasForeignKey("ByUserId");
                });

            modelBuilder.Entity("Allprimetech.Interfaces.Models.SystemAudit", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", "Operator")
                        .WithMany()
                        .HasForeignKey("OperatorId");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Allprimetech.Interfaces.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
