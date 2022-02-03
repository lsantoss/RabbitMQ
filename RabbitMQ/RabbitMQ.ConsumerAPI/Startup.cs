using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.ConsumerAPI.BackgroundServices;
using RabbitMQ.Infra.Crosscutting;

namespace RabbitMQ.ConsumerAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddControllers();

            _ = services.AddDependencies(Configuration);

            _ = services.AddHostedService<RabbitMQConsumerPayments>();
            //_ = services.AddHostedService<RabbitMQConsumerReversals>();

            _ = services.AddSwaggerGen(sw => {
                sw.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RabbitMQ.ConsumerAPI",
                    Version = "v1",
                    Description = "RabbitMQ Consumer API"
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI(sw => {
                    sw.SwaggerEndpoint("/swagger/v1/swagger.json", "RabbitMQ.ConsumerAPI v1");
                    sw.RoutePrefix = string.Empty;
                });
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            //_ = app.UseRabbitConsumerPayments();
            //_ = app.UseRabbitConsumerReversals();
        }
    }
}