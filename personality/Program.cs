// Copyright (c) 2022 Mario Lins <mario.lins@ins.jku.at>
//
// Licensed under the EUPL, Version 1.2. 
//  
// You may obtain a copy of the Licence at: 
// https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12

using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your token: Bearer <Token>",
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }, new string[] {}}
    });
});

if(Environment.GetEnvironmentVariable("security_key") == null) {
    throw new ArgumentNullException("security_key");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("issuer"),
            ValidAudience = Environment.GetEnvironmentVariable("audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("security_key")))
        };
    });
builder.Services.AddAuthorization(options => {
    options.AddPolicy("onlyTreeManager", policy => policy.RequireClaim(claimType: ClaimTypes.Role, "treeManager"));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
