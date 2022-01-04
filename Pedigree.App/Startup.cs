using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Pedigree.Core.Data;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Extensions;
using Pedigree.Core.Mapping;
using Pedigree.Core.Service;
using Pedigree.Core.Service.Impl;
using Pedigree.Core.Service.Interface;
using Pedigree.Infrastructure.Database;
using Pedigree.Infrastructure.Database.Repository;
using System;
using System.Text;

namespace Pedigree.App
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
            services.AddControllersWithViews();
            // Settings
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


            // Database
            services.AddDbContext<ApplicationDbContext>(ops => ops.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.MigrationsAssembly("Pedigree.App")));
            var connectionString = new ConnectionString(Configuration.GetConnectionString("DefaultConnection"));
            services.AddSingleton(connectionString);
            // Mappers
            services.AddAutoMapper(typeof(MappingProfile));

            // Repositories
            services.AddScoped<IHorseRepository, HorseRepository>();
            services.AddScoped<IRelationshipRepository, RelationshipRepository>();
            //services.AddScoped<IGenericRepository<Relationship>, GenericRepository<Relationship>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRaceRepository, RaceRepository>();
            services.AddScoped<IWeightRepository, WeightRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IInbreedRepository, InbreedRepository>();
            services.AddScoped<IMtDNARepository, MtDNARepository>();
            services.AddScoped<IStallionRatingRepository, StallionRatingRepository>();
            services.AddScoped<IMLRepository, MLRepository>();
            services.AddScoped<IAuctionRepository, AuctionRepository>();

            // Services
            services.AddScoped<IHorseService, HorseService>();
            services.AddScoped<IRelationshipService, RelationshipService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IRaceService, RaceService>();
            services.AddScoped<IWeightService, WeightService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IInbreedService, InbreedService>();
            services.AddScoped<IMtDNAService, MtDNAService>();
            services.AddScoped<IStallionRatingService, StallionRatingService>();
            services.AddScoped<IMLService, MLService>();
            services.AddScoped<IAuctionService, AuctionService>();


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // Cron jobs
            services.AddCronJob<HorseCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/10 * * * 0,1,2,4,5,6";     // Process 300 horses every 10 minutes
            });
            services.AddCronJob<MtDNACronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 0 * * 1";        // Every Monday 00:00
            });
            services.AddCronJob<BPRCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 0 * * 3";        // Every Wednesday 00:00
            });
            services.AddCronJob<StallionRatingCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 3 * * 3";        // Every Wednesday 03:00 after BPRs were calculated
            });
            services.AddCronJob<ProbOrigCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/30 * * * *";     // Process 100 horses ProbOrigs and 500 horses Pedigree every 30 minutes
            });
            services.AddCronJob<AncestryCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 4 * * 3";     // 04:00 every Wednesday
            });
            services.AddCronJob<GRainCronJob1>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0,10,20,30,40,50 * * * 0,1,2,4,5,6";     // Process 15 horses every 10 minutes
            });
            services.AddCronJob<GRainCronJob2>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"2,12,22,32,42,52 * * * 0,1,2,4,5,6";     // Process 15 horses every 10 minutes
            });
            services.AddCronJob<GRainCronJob3>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"5,15,25,35,45,55 * * * 0,1,2,4,5,6";     // Process 15 horses every 10 minutes
            });
            services.AddCronJob<GRainCronJob4>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"7,17,27,37,47,57 * * * 0,1,2,4,5,6";     // Process 15 horses every 10 minutes
            });
            services.AddCronJob<UniqueAncestorsCountCronJob>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"*/10 * * * 0,1,2,4,5,6";     // Every 10 minutes
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
