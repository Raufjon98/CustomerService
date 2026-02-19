using Bogus;
using Customer.Contracts.User.Responses;
using CustomerService.Api.Domain;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;

namespace CustomerService.Api.Tests.Integration.Endpoints.Users;

public class GetUserTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly CustomerServiceApiFactory _factory;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public GetUserTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _userService = MagicOnionClient.Create<IUserService>(channel);
        _authService = MagicOnionClient.Create<IAuthService>(channel);
    }

    [Fact]
    public async Task GetUser_ReturnsUserResponse_WhenUserExists()
    {
        //Arrange
        var registerRequest = TestDataFactory.CreateRegisterRequest();
        var registerResponse = await _authService.RegisterAsync(registerRequest);
        registerResponse.Should().BeTrue();

        var loginRequest = new LoginRequest
        {
            Username = registerRequest.Username,
            Password = registerRequest.Password
        };

        var loginResponse = await _authService.LoginAsync(loginRequest);
        var userId = JwtReader.GetUserId(loginResponse.Token!);

        //Act
        var response = await _userService.GetUserAsync(userId.ToString());

        //Assert
        response.Should().NotBeNull();
        response.Should().BeOfType<UserResponse>();
        response.Id.Should().Be(userId.ToString());
        response.FirstName.Should().Be(registerRequest.Firstname);
        response.LastName.Should().Be(registerRequest.Lastname);
    }

    [Fact]
    public async Task GetUser_ThrowsException_WhenUserDoesNotExist()
    {
        //Arrange
        var userId = Guid.NewGuid();

        //Act
        Func<Task> act = async () => await _userService.GetUserAsync(userId.ToString());

        //Assert
        await act.Should().ThrowAsync<RpcException>($"Entity {nameof(User)} with key {userId} not found!");
    }
}