using Microsoft.AspNetCore.Mvc;
using SolrExamples.Api.Models;
using SolrExamples.Api.Services;

namespace SolrExamples.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonsController : Controller
{
    private readonly PersonGenerator _personGenerator;

    public PersonsController(PersonGenerator personGenerator)
    {
        _personGenerator = personGenerator;
    }

    [HttpGet("All")]
    public IEnumerable<Person> All()
    {
        for (var i = 0; i < 10000; i++)
        {
            yield return _personGenerator.GeneratePerson();
        }
    }
}
