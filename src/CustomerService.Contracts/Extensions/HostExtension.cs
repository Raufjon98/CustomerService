using CustomerService.Contracts.Interfaces;
using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Contracts.Extensions;

public static class HostExtension
{
    public static IServiceCollection AddCustomerServiceContracts(this IServiceCollection services)
    {
        var customerApiUrl = "https://localhost:5051";
       services.AddSingleton<IAuthService>(_ =>
            MagicOnionClient.Create<IAuthService>(GrpcChannel.ForAddress(customerApiUrl)));
        services.AddSingleton<IUserService>(_ =>
            MagicOnionClient.Create<IUserService>(GrpcChannel.ForAddress(customerApiUrl)));
        
        return services;
    }
}