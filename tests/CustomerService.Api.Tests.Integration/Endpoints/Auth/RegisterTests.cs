using Bogus;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using MagicOnion.Client;

namespace CustomerService.Api.Tests.Integration.Endpoints.Auth;

public class RegisterTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly IAuthService _authServiceClient;
    private readonly CustomerServiceApiFactory _factory;

    public RegisterTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _authServiceClient = MagicOnionClient.Create<IAuthService>(channel);
    }

    [Fact]
    public async Task RegisterUser_ReturnsTrue_WhenUserDataValid()
    {
        //Arrange
        var user = TestDataFactory.CreateRegisterRequest();

        //Act
        var response = await _authServiceClient.RegisterAsync(user);

        //Assert
        response.Should().BeTrue();
    } 
    
    [Fact]
    public async Task RegisterUser_ReturnsFalse_WhenUserDataInValid()
    {
        //Arrange
        var user = TestDataFactory.CreateRegisterRequest();
        user.Password = Guid.NewGuid().ToString();
        //Act
        var response = await _authServiceClient.RegisterAsync(user);

        //Assert
        response.Should().BeFalse();
    }
}