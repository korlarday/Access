using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allprimetech.DAL;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Models;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Allprimetech.Interfaces.Roles;

namespace SAP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options => options
                        .UseMySql(Configuration.GetConnectionString("MySqlConnection")));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);


            #region Interfaces bindings
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<ICustomersRepository, CustomerRepository>();
            services.AddScoped<ICylinderRepository, CylinderRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISystemAuditRepository, SystemAuditRepository>();
            services.AddScoped<IStatsRepository, StatsRepository>();
            services.AddScoped<IProductionRepository, ProductionRepository>();
            services.AddScoped<IDiscRepository, DiscRepository>();
            services.AddScoped<IGroupsInfoRepository, GroupsInfoRepository>();
            #endregion

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddCors();

            //Oauth Authentication
            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:Secret"].ToString());

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for the hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/api/connect")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddMailKit(config => config.UseMailKit(Configuration.GetSection("Email").Get<MailKitOptions>()));


            #region policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AccountManagement", policy => policy.RequireRole(RolesConst.AccountManagement));
                options.AddPolicy("AccountCreate", policy => policy.RequireRole(RolesConst.AccountManagement, RolesConst.AccountCreate));
                options.AddPolicy("AccountRead", policy => policy.RequireRole(RolesConst.AccountManagement, RolesConst.AccountRead));
                options.AddPolicy("AccountUpdate", policy => policy.RequireRole(RolesConst.AccountManagement, RolesConst.AccountUpdate));
                options.AddPolicy("AccountDelete", policy => policy.RequireRole(RolesConst.AccountManagement, RolesConst.AccountDelete));

                options.AddPolicy("CustomerManagement", policy => policy.RequireRole(RolesConst.CustomerManagement));
                options.AddPolicy("CustomerCreate", policy => policy.RequireRole(RolesConst.CustomerManagement, RolesConst.CustomerCreate));
                options.AddPolicy("CustomerRead", policy => policy.RequireRole(RolesConst.CustomerManagement, RolesConst.CustomerRead));
                options.AddPolicy("CustomerUpdate", policy => policy.RequireRole(RolesConst.CustomerManagement, RolesConst.CustomerUpdate));
                options.AddPolicy("CustomerDelete", policy => policy.RequireRole(RolesConst.CustomerManagement, RolesConst.CustomerDelete));

                options.AddPolicy("OrderManagement", policy => policy.RequireRole(RolesConst.OrderManagement));
                options.AddPolicy("OrderCreate", policy => policy.RequireRole(RolesConst.OrderManagement, RolesConst.OrderCreate));
                options.AddPolicy("OrderRead", policy => policy.RequireRole(RolesConst.OrderManagement, RolesConst.OrderRead));
                options.AddPolicy("OrderUpdate", policy => policy.RequireRole(RolesConst.OrderManagement, RolesConst.OrderUpdate));
                options.AddPolicy("OrderDelete", policy => policy.RequireRole(RolesConst.OrderManagement, RolesConst.OrderDelete));

                options.AddPolicy("PartnerManagement", policy => policy.RequireRole(RolesConst.PartnerManagement));
                options.AddPolicy("PartnerCreate", policy => policy.RequireRole(RolesConst.PartnerManagement, RolesConst.PartnerCreate));
                options.AddPolicy("PartnerRead", policy => policy.RequireRole(RolesConst.PartnerManagement, RolesConst.PartnerRead));
                options.AddPolicy("PartnerUpdate", policy => policy.RequireRole(RolesConst.PartnerManagement, RolesConst.PartnerUpdate));
                options.AddPolicy("PartnerDelete", policy => policy.RequireRole(RolesConst.PartnerManagement, RolesConst.PartnerDelete));

                options.AddPolicy("CylinderManagement", policy => policy.RequireRole(RolesConst.CylinderManagement));
                options.AddPolicy("CylinderCreate", policy => policy.RequireRole(RolesConst.CylinderManagement, RolesConst.CylinderCreate));
                options.AddPolicy("CylinderRead", policy => policy.RequireRole(RolesConst.CylinderManagement, RolesConst.CylinderRead));
                options.AddPolicy("CylinderUpdate", policy => policy.RequireRole(RolesConst.CylinderManagement, RolesConst.CylinderUpdate));
                options.AddPolicy("CylinderDelete", policy => policy.RequireRole(RolesConst.CylinderManagement, RolesConst.CylinderDelete));

                options.AddPolicy("GroupsManagement", policy => policy.RequireRole(RolesConst.GroupsManagement));
                options.AddPolicy("GroupsCreate", policy => policy.RequireRole(RolesConst.GroupsManagement, RolesConst.GroupsCreate));
                options.AddPolicy("GroupsRead", policy => policy.RequireRole(RolesConst.GroupsManagement, RolesConst.GroupsRead));
                options.AddPolicy("GroupsUpdate", policy => policy.RequireRole(RolesConst.GroupsManagement, RolesConst.GroupsUpdate));
                options.AddPolicy("GroupsDelete", policy => policy.RequireRole(RolesConst.GroupsManagement, RolesConst.GroupsDelete));

                options.AddPolicy("SystemAuditManagement", policy => policy.RequireRole(RolesConst.SystemAuditManagement));
                options.AddPolicy("SystemAuditCreate", policy => policy.RequireRole(RolesConst.SystemAuditManagement, RolesConst.SystemAuditCreate));
                options.AddPolicy("SystemAuditRead", policy => policy.RequireRole(RolesConst.SystemAuditManagement, RolesConst.SystemAuditRead));
                options.AddPolicy("SystemAuditUpdate", policy => policy.RequireRole(RolesConst.SystemAuditManagement, RolesConst.SystemAuditUpdate));
                options.AddPolicy("SystemAuditDelete", policy => policy.RequireRole(RolesConst.SystemAuditManagement, RolesConst.SystemAuditDelete));

                options.AddPolicy("DiscManagement", policy => policy.RequireRole(RolesConst.DiscManagement));
                options.AddPolicy("DiscCreate", policy => policy.RequireRole(RolesConst.DiscManagement, RolesConst.DiscCreate));
                options.AddPolicy("DiscRead", policy => policy.RequireRole(RolesConst.DiscManagement, RolesConst.DiscRead));
                options.AddPolicy("DiscUpdate", policy => policy.RequireRole(RolesConst.DiscManagement, RolesConst.DiscUpdate));
                options.AddPolicy("DiscDelete", policy => policy.RequireRole(RolesConst.DiscManagement, RolesConst.DiscDelete));

                options.AddPolicy("GroupsInfoManagement", policy => policy.RequireRole(RolesConst.GroupsInfoManagement));
                options.AddPolicy("GroupsInfoCreate", policy => policy.RequireRole(RolesConst.GroupsInfoManagement, RolesConst.GroupsInfoCreate));
                options.AddPolicy("GroupsInfoRead", policy => policy.RequireRole(RolesConst.GroupsInfoManagement, RolesConst.GroupsInfoRead));
                options.AddPolicy("GroupsInfoUpdate", policy => policy.RequireRole(RolesConst.GroupsInfoManagement, RolesConst.GroupsInfoUpdate));
                options.AddPolicy("GroupsInfoDelete", policy => policy.RequireRole(RolesConst.GroupsInfoManagement, RolesConst.GroupsInfoDelete));

                options.AddPolicy("ProductionManagement", policy => policy.RequireRole(RolesConst.ProductionManagement));
                options.AddPolicy("ProductionCreate", policy => policy.RequireRole(RolesConst.ProductionManagement, RolesConst.ProductionCreate));
                options.AddPolicy("ProductionRead", policy => policy.RequireRole(RolesConst.ProductionManagement, RolesConst.ProductionRead));
                options.AddPolicy("ProductionUpdate", policy => policy.RequireRole(RolesConst.ProductionManagement, RolesConst.ProductionUpdate));
                options.AddPolicy("ProductionDelete", policy => policy.RequireRole(RolesConst.ProductionManagement, RolesConst.ProductionDelete));
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
