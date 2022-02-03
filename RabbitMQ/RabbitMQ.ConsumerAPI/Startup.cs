using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.ConsumerAPI.ApplicationBuilders;
using RabbitMQ.ConsumerAPI.BackgroundServices;
using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.Emails.Services;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Core.RabbitMQ.Services;
using RabbitMQ.Domain.Emails.Handlers;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Emails.Interfaces.Repositories;
using RabbitMQ.Domain.Payments.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Handlers;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Domain.Reversals.Interfaces.Repositories;
using RabbitMQ.Infra.Data.Repositories;

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

            #region AppSettings

            var settings = new Settings();
            Configuration.GetSection("Settings").Bind(settings);
            _ = services.AddSingleton(settings);

            var rabbitMQSettings = new RabbitMQSettings();
            Configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
            _ = services.AddSingleton(rabbitMQSettings);

            var smtpSettings = new SmtpSettings();
            Configuration.GetSection("SmtpSettings").Bind(smtpSettings);
            _ = services.AddSingleton(smtpSettings);

            #endregion AppSettings

            #region Repositories

            _ = services.AddScoped<IElmahRepository, ElmahRepository>();
            _ = services.AddScoped<IQueueLogRepository, QueueLogRepository>();
            _ = services.AddScoped<IPaymentRepository, PaymentRepository>();
            _ = services.AddScoped<IReversalRepository, ReversalRepository>();
            _ = services.AddScoped<IEmailRepository, EmailRepository>();

            #endregion Repositories

            #region Handlers

            _ = services.AddScoped<BaseHandler, BaseHandler>();
            _ = services.AddScoped<IPaymentHandler, PaymentHandler>();
            _ = services.AddScoped<IReversalHandler, ReversalHandler>();
            _ = services.AddScoped<IEmailHandler, EmailHandler>();

            #endregion Handlers

            #region Services

            _ = services.AddScoped<IRabbitMQService, RabbitMQService>();
            _ = services.AddScoped<IEmailSenderService, EmailSenderService>();

            #endregion Services

            _ = services.AddHostedService<RabbitMQConsumerPayments>();
            _ = services.AddHostedService<RabbitMQConsumerReversals>();

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

            //_ = app.UseRabbitConsumerPayment();
            //_ = app.UseRabbitConsumerReversal();
            //_ = app.UseRabbitConsumerEmail();
        }
    }
}