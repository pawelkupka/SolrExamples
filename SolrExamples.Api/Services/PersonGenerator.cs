using Bogus;
using SolrExamples.Api.Models;

namespace SolrExamples.Api.Services;

public class PersonGenerator
{
    readonly Faker<Models.Person> _personFaker;
    readonly Faker<Models.Address> _addressFaker;

    public PersonGenerator()
    {
        _personFaker = new Faker<Models.Person>()
            .RuleFor(x => x.Id, f => Guid.NewGuid())
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(x => x.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(x => x.Type, f => f.PickRandom<PersonType>());

        _addressFaker = new Faker<Models.Address>()
            .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.State, f => f.Address.State())
            .RuleFor(x => x.ZipCode, f => f.Address.ZipCode());
    }

    public Models.Person GeneratePerson()
    {
        var person = _personFaker.Generate();
        var address1 = _addressFaker.Generate();
        var address2 = _addressFaker.Generate();
        person.AddAddress(address1);
        person.AddAddress(address2);
        return person;
    }
}
