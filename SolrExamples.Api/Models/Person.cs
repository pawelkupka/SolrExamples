using System.Text.Json.Serialization;

namespace SolrExamples.Api.Models;

public class Person
{
    [JsonPropertyName("person_id")]
    public Guid Id { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonIgnore]
    public PersonType Type { get; set; }

    [JsonIgnore]
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
