using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ToDoDomain.Sql.Context;

namespace ToDoWebApp
{
    public class Startup
    {

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            // add Database
            var dbTypeString = Configuration.GetValue<string>("DatabaseType");

            var dbType = Enum.Parse<SqlDatabaseType>(dbTypeString);

            switch (dbType)
            {
                case SqlDatabaseType.SqlServer: services.AddDbContext<ToDoDbContext>(option => option.UseSqlServer(Configuration.GetValue<string>("DbConnectionString"))); break;
                default:
                    {
                        var connection = new SqliteConnection("DataSource=:memory:");
                        connection.Open();

                        services.AddDbContext<ToDoDbContext>(option =>
                            option.UseSqlite(connection));
                        break;
                    }
            }

            // add swagger
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ToDoApi", new OpenApiInfo { Title = "To Do Simple Api - Web API", Version = "v1" });

                c.TagActionsBy(api => new List<string>() { api.RelativePath.Split("/")[2] /* base endpoint */ }); // configura endpoint .Split("/")[2]

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // configure filters if desidered
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;

            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            .AddNewtonsoftJson(options => // add json converter
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK";
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            services.AddSwaggerGenNewtonsoftSupport();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // cors settings
            app.UseCors(options =>
                options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/ToDoApi/swagger.json", "ToDoApi"); // serve swagger
            }
            );
        }

        private enum SqlDatabaseType
        {
            SqlLite,
            SqlServer,
        }
    }
}
