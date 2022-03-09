using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchMVC.Infra.IoC
{
    public static class DependencyInjectionCORS
    {
        public static IServiceCollection AddInfrastructureCORS(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder => builder.WithOrigins("https://localhost:2000"));
                options.AddPolicy("MyPolicy", builder =>
                    builder.WithOrigins("https://localhost:2001"));
            });

            return services;
        }
    }
}
