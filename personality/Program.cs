// Copyright (c) 2022 Mario Lins <mario.lins@ins.jku.at>
//
// Licensed under the EUPL, Version 1.2. 
//  
// You may obtain a copy of the Licence at: 
// https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    opt => {
        opt.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme{
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Please enter your API key in the form of: ApiKey: YOUR-API-KEY",
            Name = "ApiKey",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        });
        opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement{
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "ApiKey",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "ApiKey"},
                },
                new string[] {}
            }
        });
    }
);
builder.Services.AddAuthentication(
    opt => {
        opt.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
        opt.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
    }   
        
    ).AddApiKeySupport(options => {});

//used for IoC
builder.Services.AddSingleton<IApiKeyService,ApiKeyService>();
builder.Services.AddSingleton<IAuthorizationHandler, TreeManagerRequirementAuthorizationHandler>();

builder.Services.AddAuthorization(
    opt => 
        opt.AddPolicy(Policies.TreeManagerPolicy, policy => policy.Requirements.Add(new TreeManagerRequirement()))    
    );

var app = builder.Build();

app.UseAuthorization();
app.UseAuthentication();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
