using CleanArchMVC.Infra.IoC;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;

namespace CleanArchMVC.API
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
            services.AddInfrastructureAPI(Configuration);

            //ativar autenticacao e validar o token
            services.AddInfrastructureJWT(Configuration);

            //cors
            //services.AddCors();
            //services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //{
            //    builder.WithOrigins("http://example.com",
            //                        "http://exemplo.com")
            //           .AllowAnyMethod()
            //           .AllowAnyHeader();
            //}));
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder.WithOrigins("https://localhost:2000"));
                options.AddPolicy("MyPolicy", builder => 
                    builder.WithOrigins("https://localhost:2001"));
              }  );

            services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = "localhost:6379";
                }
            );

            services.AddHealthChecks();

            services.AddControllers();

            //configuracao swagger
            services.AddInfrastructureSwagger();

            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: Configuration.GetConnectionString("DefaultConnection"),
                    healthQuery: "SELECT 1;",
                    name: "sqlserver",
                    failureStatus: HealthStatus.Degraded,
                    tags: new string[] { "db", "data", "sqlserver" })
                .AddRedis(
                    "localhost:6379",
                    name: "redis", 
                    tags: new string[] {"db","cache", "redis" } );
            //services.AddHealthChecksUI()
            //    .AddInMemoryStorage();
            services.AddHealthChecksUI(setupSettings: setup =>
            {
                
                setup.SetEvaluationTimeInSeconds(5);
            }).AddInMemoryStorage();
                

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchMVC.API v1"));
            }
            //health Check
            app.UseHealthChecks("/status",
                new HealthCheckOptions()
                {
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonConvert.SerializeObject(
                            new
                            {
                                currentTime = DateTime.Now.ToString("g"),
                                statusApplication = report.Status.ToString(),
                                healthChecks = report.Entries.Select(e=> new
                                {
                                    check = e.Key,
                                    status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                                })
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }

                });
            

            app.UseHealthChecks("/healthchecks-data-ui", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/monitor";
            });

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();

            //cors
            //app.UseCors(x=>x
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .SetIsOriginAllowed(origin => true)
            //    .AllowCredentials()
            //);
            app.UseCors("MyPolicy");


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
