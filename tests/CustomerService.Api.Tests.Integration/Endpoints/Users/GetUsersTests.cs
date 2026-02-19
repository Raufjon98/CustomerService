using System.Formats.Asn1;
using Bogus;
using CustomerService.Api.Infrastructure.Data;
using CustomerService.Api.Tests.Integration.Models;
using CustomerService.Contracts.Authorization.Requests;
using CustomerService.Contracts.Interfaces;
using FluentAssertions;
using MagicOnion.Client;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Api.Tests.Integration.Endpoints.Users;

public class GetUsersTests : IClassFixture<CustomerServiceApiFactory>
{
    private readonly CustomerServiceApiFactory _factory;
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public GetUsersTests(CustomerServiceApiFactory factory)
    {
        _factory = factory;
        var channel = _factory.CreateGrpcChannel();
        _authService = MagicOnionClient.Create<IAuthService>(channel);
        _userService = MagicOnionClient.Create<IUserService>(channel);
    }

    [Fact]
    public async Task ShouldReturnAllUsers()
    {
        //Arrange
        var user = TestDataFactory.CreateRegisterRequest();
        var registerResult = await _authService.RegisterAsync(user);
        registerResult.Should().BeTrue();
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        //Act
        var response = await _userService.GetUsersAsync();

        //Assert
        response.Should().Contain(u => u.FirstName == user.Firstname && u.LastName == user.Lastname);
    }
}