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
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (message, displayName) =>
            {
                Console.WriteLine($"{message} {displayName}");
            };

            #endregion

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
