using Bogus;
using Customer.Contracts.User.Requests;
using CustomerService.Api.Domain;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using Grpc.Core;
using MagicOnion.Client;

namespace CustomerService.Api.Tests.Integration.Endpoints.Users;

public class UpdateUserTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly CustomerServiceApiFactory _factory;

    private readonly Faker<UpdateUserRequest> _updateGenerator =
        new Faker<UpdateUserRequest>()
            .RuleFor(x => x.Username, f => f.Person.UserName)
            .RuleFor(x => x.Email, f => f.Internet.Email())
            .RuleFor(x => x.FirstName, f => f.Person.FirstName)
            .RuleFor(x => x.Phone, f => f.Person.Phone)
            .RuleFor(x => x.LastName, f => f.Person.LastName);

    public UpdateUserTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _userService = MagicOnionClient.Create<IUserService>(channel);
        _authService = MagicOnionClient.Create<IAuthService>(channel);
    }

    [Fact]
    public async Task UpdateUser_ReturnsUserResponse_WhenUserUpdated()
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
        var updateUserRequest = _updateGenerator.Generate();
        var userId = JwtReader.GetUserId(loginResult.Token!);
        //Act
        var response = await _userService.UpdateUserAsync(updateUserRequest, userId.ToString());

        //Assert
        response.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateUser_ReturnsError_WhenUserNotUpdated()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var updateUserRequest = _updateGenerator.Generate();

        //Act
        Func<Task> act = async () => await _userService.UpdateUserAsync(updateUserRequest, userId.ToString());

        //Assert
        await act.Should().ThrowAsync<RpcException>($"Entity {nameof(User)} with key {userId} not found!");
    }
}