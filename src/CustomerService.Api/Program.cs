using System.Reflection;
using CustomerService.Api.Data;
using CustomerService.Api.Interfaces;
using CustomerService.Api.Domain;
using CustomerService.Api.Features.Common.Behaviors;
using CustomerService.Api.Infrastructure.Data;
using CustomerService.Api.Infrastructure.Interceptors;
using CustomerService.Api.MagicOnion.Services;
using CustomerService.Api.Services;
using CustomerService.Contracts.Interfaces;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using PaymentService.Contracts.Extentions;
using RabbitMQ.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnections");

var rabbitConnectionString = builder.Configuration["MessageBroker:Host"];

if (!builder.Environment.IsEnvironment("IntegrationTest"))
{
    builder.Services.AddMassTransit(configuration =>
    {
        configuration.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(rabbitConnectionString);
            cfg.ExchangeType = ExchangeType.Fanout;
            cfg.ConfigureEndpoints(ctx);
        });
    });
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5051, listenOptions =>
    {
        listenOptions.UseHttps();
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<DbContextInitializer>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAuthorization();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddPaymentServiceContracts();
builder.Services.AddGrpc(options => { options.Interceptors.Add<ExceptionInterceptor>(); });
builder.Services.AddMagicOnion();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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