using System.Reflection;
using CustomerService.Api.Data;
using CustomerService.Api.Interfaces;
using CustomerService.Api.Domain;
using CustomerService.Api.Infrastructure.Data;
using CustomerService.Api.Infrastructure.Interceptors;
using CustomerService.Api.MagicOnion.Services;
using CustomerService.Api.Services;
using CustomerService.Contracts.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PaymentService.Contracts.Extentions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnections");
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5051, listenOptions =>
    {
        listenOptions.UseHttps();              
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});
// builder.Services.AddOpenApi(options =>
// {
//     options.AddDocumentTransformer((document, context, cancellationToken) =>
//     {
//         document.Components ??= new();
//         document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
//         
//         document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
//         {
//             Type = SecuritySchemeType.Http,
//             Scheme = "bearer",
//             BearerFormat = "JWT",
//             Description = "Enter your JWT token in the format: Bearer {your token}"
//         });
//
//         document.SecurityRequirements = new List<OpenApiSecurityRequirement>
//         {
//             new()
//             {
//                 [new OpenApiSecurityScheme
//                 {
//                     Reference = new OpenApiReference
//                     {
//                         Type = ReferenceType.SecurityScheme,
//                         Id = "Bearer"
//                     }
//                 }] = Array.Empty<string>()
//             }
//         };
//
//         return Task.CompletedTask;
//     });
// });
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<ApplicationDbContext>();
//
// builder.Services.AddAuthentication(options =>
// {
//     options.DefaultAuthenticateScheme =
//         options.DefaultChallengeScheme =
//             options.DefaultForbidScheme =
//                 options.DefaultScheme =
//                     options.DefaultSignInScheme =
//                         options.DefaultSignOutScheme =
//                             JwtBearerDefaults.AuthenticationScheme;
// }).AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters()
//     {
//         ValidateIssuer = true,
//         ValidIssuer = builder.Configuration["JWT:Issuer"],
//         ValidateAudience = true,
//         ValidAudience = builder.Configuration["JWT:Audience"],
//         ValidateIssuerSigningKey = true,
//         IssuerSigningKey = new SymmetricSecurityKey(
//             System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SignInKey"]!)),
//     };
// });
builder.Services.AddScoped<DbContextInitializer>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAuthorization();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddPaymentServiceContracts();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});
builder.Services.AddMagicOnion();

var app = builder.Build();

await app.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapMagicOnionService();
app.Run();