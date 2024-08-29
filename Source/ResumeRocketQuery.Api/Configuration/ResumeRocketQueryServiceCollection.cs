using System.Text;
using ResumeRocketQuery.Api.Builder;
using ResumeRocketQuery.Api.Filters;
using ResumeRocketQuery.Domain.Api;
using ResumeRocketQuery.Domain.Configuration;
using ResumeRocketQuery.Domain.DataLayer;
using ResumeRocketQuery.Domain.Repository;
using ResumeRocketQuery.Domain.Services;
using ResumeRocketQuery.Domain.Services.Helper;
using ResumeRocketQuery.Repository;
using ResumeRocketQuery.Services;
using ResumeRocketQuery.Services.Helper;
using ResumeRocketQuery.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using System.IO;
using System.Reflection;
using System;
using ResumeRocketQuery.Domain.External;
using ResumeRocketQuery.External;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http.Features;
using ResumeRocketQuery.Service;

namespace ResumeRocketQuery.Api.Configuration
{
    public class ResumeRocketQueryServiceCollection
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IResumeRocketQueryConfigurationSettings, ResumeRocketQueryConfigurationSettings>();
            services.AddSingleton<IResumeRocketQueryStorage, DapperResumeRocketQueryStorage>();

            services.AddSingleton<IResumeRocketQueryRepository, ResumeRocketQueryRepository>();
            services.AddTransient<IOpenAiClient, OpenAiClient>(); 
            services.AddTransient<IJobScraper, jobScraper>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IAuthenticationHelper, AuthenticationHelper>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IPortfolioService, PortfolioService>();
            services.AddTransient<IPdfService, PdfService>();
            services.AddTransient<IJobService, JobService>();

            services.AddSingleton<IServiceResponseBuilder, ServiceResponseBuilder>();
            services.AddSingleton<IResumeRocketQueryUserBuilder, ResumeRocketQueryUserBuilder>();

            ConfigureJwtAuthentication(services);
            ConfigureMiddlewareServices(services);
        }

        private void ConfigureMiddlewareServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MemoryBufferThreshold = int.MaxValue;
            });

            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
                options.Filters.Add(typeof(ValidationFilter));
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("ResumeRocketQuery", new OpenApiInfo
                {
                    Title = "ResumeRocketQuery",
                    Version = "1",
                    Description = "Restful Api for ResumeRocketQuery Service."
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        private void ConfigureJwtAuthentication(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var resumeRocketQueryConfiguration = serviceProvider.GetService<IResumeRocketQueryConfigurationSettings>();
            var key = Encoding.ASCII.GetBytes(resumeRocketQueryConfiguration.AuthenticationPrivateKey);
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

            serviceProvider.Dispose();
        }
    }
}
