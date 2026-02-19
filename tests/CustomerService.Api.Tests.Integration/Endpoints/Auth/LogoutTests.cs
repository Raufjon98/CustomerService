using Bogus;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using MagicOnion.Client;

namespace CustomerService.Api.Tests.Integration.Endpoints.Auth;

public class LogoutTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly CustomerServiceApiFactory _factory;
    private readonly IAuthService _authService;
  
    public LogoutTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _authService = MagicOnionClient.Create<IAuthService>(channel); ;
    }

    [Fact]
    public async Task Logout_ReturnsTrue()
    {
        //Act
        var response = await _authService.LogoutAsync();

        //Assert
        response.Should().BeTrue();
    }
}