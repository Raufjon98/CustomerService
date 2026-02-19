using Bogus;
using CustomerService.Contracts.Authorization.Requests;

namespace CustomerService.Api.Tests.Integration.Models;

public static class TestDataFactory
{
    private static readonly Faker<RegisterRequest> _faker = new Faker<RegisterRequest>()
        .RuleFor(x => x.Username, f => f.Person.UserName)
        .RuleFor(x => x.Firstname, f => f.Person.FirstName)
        .RuleFor(x => x.Lastname, f => f.Person.LastName)
        .RuleFor(x => x.Email, f => f.Internet.Email())
        .RuleFor(x => x.Password, (f, u) => $"Customer{u.Firstname}@1234!")
        .RuleFor(x => x.DateOfBirth,
            f => DateOnly.FromDateTime(f.Date.Between(DateTime.Today.AddYears(-100), DateTime.Today.AddYears(-18))));

    public static RegisterRequest CreateRegisterRequest()
    {
        return _faker.Generate();
    }
}