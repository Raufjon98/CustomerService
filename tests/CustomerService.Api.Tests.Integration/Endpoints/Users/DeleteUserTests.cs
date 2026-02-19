using Bogus;
using Customer.Contracts.User.Requests;
using CustomerService.Api.Domain;
using CustomerService.Api.Features.Common.Exceptions;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using NSubstitute;
using PaymentService.Contracts.Interfaces;

namespace CustomerService.Api.Tests.Integration.Endpoints.Users;

public class DeleteUserTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly CustomerServiceApiFactory _factory;

    public DeleteUserTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _authService = MagicOnionClient.Create<IAuthService>(channel);
        _userService = MagicOnionClient.Create<IUserService>(channel);
    }

    [Fact]
    public async Task DeleteUser_ReturnsTrue_WhenUserDeleted()
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
        _factory.AccountServiceMock.DeleteAccountAsync(userId).Returns(new UnaryResult<bool>(true));

        //Act
        var response = await _userService.DeleteUserAsync(userId.ToString());

        //Assert
        response.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUser_ReturnsException_WhenUserNotFound()
    {
        //Arrange
        var userId = Guid.NewGuid();
        _factory.AccountServiceMock.DeleteAccountAsync(userId).Returns(new UnaryResult<bool>(false));

        //Act
        Func<Task> act = async () => await _userService.DeleteUserAsync(userId.ToString());

        //Assert
        await act.Should().ThrowAsync<RpcException>($"Entity {nameof(User)} with key {userId} not found!");
    }
}