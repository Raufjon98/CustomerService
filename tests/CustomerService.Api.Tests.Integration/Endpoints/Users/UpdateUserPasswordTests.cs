using Bogus;
using CustomerService.Api.Domain;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;

namespace CustomerService.Api.Tests.Integration.Endpoints.Users;

public class UpdateUserPasswordTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly CustomerServiceApiFactory _factory;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UpdateUserPasswordTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _authService = MagicOnionClient.Create<IAuthService>(channel);
        _userService = MagicOnionClient.Create<IUserService>(channel);
    }

    [Fact]
    public async Task UpdateUserPassword_ReturnsTrue_WhenPasswordUpdated()
    {
        //Arrange 
        var user = TestDataFactory.CreateRegisterRequest();
        var registerResult = await _authService.RegisterAsync(user);
        registerResult.Should().BeTrue();

        var loginRequest = new LoginRequest
        {
            Username = user.Username,
            Password = user.Password
        };

        var loginResult = await _authService.LoginAsync(loginRequest);
        var userId = JwtReader.GetUserId(loginResult.Token!);
        var newPassword = user.Password + "Updated";
        //Act
        var response = await _userService.UpdateUserPassword(userId.ToString(), user.Password, newPassword);

        //Assert 
        response.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUserPassword_ReturnsFalse_WhenOldPasswordDoesNotMatches()
    {
        //Arrange
        var user = TestDataFactory.CreateRegisterRequest();
        var registerResult = await _authService.RegisterAsync(user);
        registerResult.Should().BeTrue();

        var loginRequest = new LoginRequest
        {
            Username = user.Username,
            Password = user.Password
        };
        var loginResult = await _authService.LoginAsync(loginRequest);
        var userId = JwtReader.GetUserId(loginResult.Token!);
        user.Password = Guid.NewGuid().ToString();
        //Act
        var response = await _userService.UpdateUserPassword(userId.ToString(), user.Password, user.Password);

        //Assert
        response.Should().BeFalse();
    }
}