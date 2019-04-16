using DataAccess;
using DataAccess.Concrete;
using DataAccess.EFModels;
using DataAccess.Interface;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Linq2dbTest
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton(Configuration);

            #region DI setup for db settings

            services.Configure<ConnectionStringSettings>(options => Configuration.GetSection("DBInfo").Bind(options));

            #endregion

            #region DI setup for EF context

            var connectionString = GetConnectionStringSettings().ConnectionString;

            services
                    .AddDbContext<researchContext>(options =>
                    options.UseNpgsql(connectionString));

            #endregion

            #region General DI setup for repositories

            services.AddScoped<IResearchRepository, Linq2dbResearchRepository>();
            services.AddScoped<DapperResearchRepository>();

            services.AddScoped<Func<bool, IResearchRepository>>(serviceProvider => isAuthorized =>
            {
                switch (isAuthorized)
                {
                    case true:
                        return serviceProvider.GetService<DapperResearchRepository>();
                    case false:
                        return serviceProvider.GetService<IResearchRepository>();
                    default:
                        throw new InvalidOperationException();
                }
            });

            #endregion

            #region Miniprofiler setup

            //see Linq2db instructions on https://linq2db.github.io/#miniprofiler
            services.AddMiniProfiler(options =>
               options.RouteBasePath = "/profiler"
            ).AddEntityFramework();

            #endregion

            #region Swagger setup

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-2.1
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info()
                {
                    Title = "Test API specification - querying",
                    Description = "Exposed methods to query data in the test API",
                    Version = "v1"
                });
                c.SwaggerDoc("v2", new Info()
                {
                    Title = "Test API specification - editing",
                    Description = "Exposed methods to edit data in the test API",
                    Version = "v1"
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            #region Linq2db data provider setup

            var connectionStringSettings = this.GetConnectionStringSettings();

            DataConnection.DefaultSettings = new MySettings(connectionStringSettings);

            //Enable generated SQL logging

            //https://stackoverflow.com/questions/43722141/linq2db-nlog-or-logging
            //https://linq2db.github.io/api/LinqToDB.Data.DataConnection.html#LinqToDB_Data_DataConnection_TurnTraceSwitchOn_System_Diagnostics_TraceLevel_
            DataConnection.TurnTraceSwitchOn(TraceLevel.Info); //can be parameterless
            DataConnection.WriteTraceLine = (message, displayName) =>
            {
                Console.WriteLine($"{message} {displayName}");
            };
            LinqToDB.Common.Configuration.Linq.AllowMultipleQuery = true;

            #endregion

            // Get Dapper to ignore/remove underscores in field names when mapping
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            // profiling, url to see last profile check: http://localhost:xxxxx/profiler/results
            app.UseMiniProfiler();

            app.UseHttpsRedirection();
            app.UseMvc();

            #region Swagger middleware

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "swagger");
                //c.InjectStylesheet("/swagger/custom.css");
            });

            #endregion
        }

        private ConnectionStringSettings GetConnectionStringSettings()
        {
            return new ConnectionStringSettings
            {
                Name = Configuration.GetSection("DBInfo:Name").Value.ToString(),
                ConnectionString = Configuration.GetSection("DBInfo:ConnectionString").Value.ToString(),
                ProviderName = Configuration.GetSection("DBInfo:ProviderName").Value.ToString()
            };
        }
    }
}
