using CleanArchMVC.Infra.IoC;
using HealthChecks.UI.Client;
using Logzio.DotNet.NLog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
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

            //configuracao CORS
            services.AddInfrastructureCORS();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            services.AddHealthChecks();

            services.AddControllers();

            //Logz
            var config = new LoggingConfiguration();
            var logzioTarget = new LogzioTarget
            {
                Name = "Logzio",
                Token = "cldMhKQFMbFeyPJRKTXPRDxdtMeDJhPi",
                LogzioType = "nlog",
                ListenerUrl = "https://listener.logz.io:8071",
                BufferSize = 100,
                BufferTimeout = TimeSpan.Parse("00:00:05"),
                RetriesMaxAttempts = 3,
                RetriesInterval = TimeSpan.Parse("00:00:02"),
                Debug = false,
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logzioTarget);
            LogManager.Configuration = config;



            //configuracao swagger
            services.AddInfrastructureSwagger();
            
            //configuracao HealthCheck
            services.AddInfrastructureHealthCheck(Configuration);
            
            services.AddHealthChecksUI(setupSettings: setup =>
            {                
                setup.SetEvaluationTimeInSeconds(5);
            }).AddInMemoryStorage();            
        }

        
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
