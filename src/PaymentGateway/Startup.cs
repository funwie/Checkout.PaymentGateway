using CardPaymentAcquirerBank.Sdk;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PaymentGateway.Application.AcquiringService;
using PaymentGateway.Application.MerchantService;
using PaymentGateway.Domain;
using PaymentGateway.Persistence;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using PaymentGateway.Application.Payments.Commands;
using PaymentGateway.Application.Payments.Commands.Validation;

namespace PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddHealthChecks();
            services.AddSwaggerGen(config =>
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    config.IncludeXmlComments(xmlPath);
                });

            services.Configure<DatabaseSettings>(Configuration.GetSection("databaseSettings"));
            services.Configure<CardAcquirerBankSettings>(Configuration.GetSection("cardAcquiringBank"));

            services.AddMediatR(Assembly.Load("PaymentGateway"), Assembly.Load("PaymentGateway.Application"));

            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentAcquiringService, PaymentAcquiringService>();
            services.AddScoped<IAcquirerFactory, AcquirerFactory>();
            services.AddScoped<IValidator<RequestPaymentCommand>, RequestPaymentCommandValidator>();

            services.AddCardAcquirerBankClient();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthcheck");
                endpoints.MapControllers();
            });
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Gateway API");
                    c.RoutePrefix = string.Empty;
                });
        }
    }
}
