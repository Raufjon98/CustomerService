using Bogus;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;

namespace CustomerService.Api.Tests.Integration.Endpoints.Auth;

public class LoginTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly IAuthService  _authService;
    private readonly CustomerServiceApiFactory _factory;
    
    public LoginTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _authService = MagicOnionClient.Create<IAuthService>(channel);
    }
    
    [Fact]
    public async Task  Login_ReturnsToken_WhenUserClaimsExists()
    {
        //Arrange
        var user =TestDataFactory.CreateRegisterRequest();
        var result =  await _authService.RegisterAsync(user);
        result.Should().BeTrue();
        
        var loginRequest = new LoginRequest
        {
            Username = user.Username,
            Password = user.Password
        };
            
        //Act
        var response = await _authService.LoginAsync(loginRequest);

        //Assert
        response.Should().NotBeNull();
        response.Token.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task Login_ThrowsException_WhenUserClaimsWrong()
    {
        //Arrange
        var user = TestDataFactory.CreateRegisterRequest();
        var result =  await _authService.RegisterAsync(user);
        result.Should().BeTrue();
        
        var loginRequest = new LoginRequest
        {
            Username = user.Username,
            Password = user.Lastname
        };
            
        //Act
        Func<Task> act= async() => await _authService.LoginAsync(loginRequest);

        //Assert
        await act.Should().ThrowAsync<RpcException>("Invalid username or password");
    }
}