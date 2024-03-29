﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogger.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Tweetbook.Options;
using Tweetbook.Services;
using FluentValidation.AspNetCore;
using Blogger.Filters;

namespace Tweetbook.Installers
{
    public class MVCInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings); // bind config from appsettings to jwtSettings object
            services.AddSingleton(jwtSettings);
            services.AddScoped<IIdentityService, IdentityService>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // to validate our secret key from config file
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddAuthorization(opt => 
            {
                opt.AddPolicy("HashtagsManager", builder =>
                {
                    builder.RequireClaim("hashtags_manager", "true");
                });

                opt.AddPolicy("BloggerEmployee", builder =>
                {
                    builder.AddRequirements(new BloggerEmployeeRequirement("blogger.com"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, BloggerEmployeeHandler>();

            services.AddMvc(opt => 
                {
                    opt.Filters.Add<ValidationFilter>();
                })
                .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Startup>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }
    }
}
