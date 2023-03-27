namespace SolrExamples.Api.Models;

public class Person
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public PersonType Type { get; set; }
    public IList<Address> Addresses { get; set; }

    public Person()
    {
        Addresses = new List<Address>();
    }

    public void AddAddress(Address address)
    {
        Addresses.Add(address);
    }
}
