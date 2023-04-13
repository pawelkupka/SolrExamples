using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SolrExamples.Api.Models;
using SolrExamples.Api.Services;
using System.Text.Json;

namespace SolrExamples.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonsController : Controller
{
    private readonly PersonGenerator _personGenerator;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PersonsController> _logger;

    public PersonsController(PersonGenerator personGenerator, HttpClient httpClient, IConfiguration configuration, ILogger<PersonsController> logger)
    {
        _personGenerator = personGenerator;
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("Persons")]
    public IEnumerable<Person> Persons()
    {
        for (var i = 0; i < 50000; i++)
        {
            yield return _personGenerator.GeneratePerson();
        }
    }

    /// <summary>
    /// CreateCollection
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateCollection")]
    public async Task CreateCollection()
    {
        var uriString = $"api/collections";
        var jsonString = @"{
                      'create': {
                        'name': 'persons',
                        'numShards': 3,
                        'shards': 'shard-person-1,shard-person-2,shard-person-3'
                        'replicationFactor': 2
                      }
                    }";
        await PostAsync(uriString, jsonString);
    }

    [HttpPost("CreatePersonsSchema")]
    public async Task CreatePersonsSchema()
    {
        var uriString = $"api/collections/persons/schema";
        var jsonString = @"{
                      'add-field': [
                        {'name': 'person_id', 'type': 'string'},
                        {'name': 'first_name', 'type': 'string'},
                        {'name': 'last_name', 'type': 'string'},
                        {'name': 'email', 'type': 'string'},
                        {'name': 'phone', 'type': 'string'}
                      ]
                    }";
        await PostAsync(uriString, jsonString);
    }

    [HttpPost("IndexData")]
    public async Task IndexData()
    {
        var uriString = $"api/collections/persons/update";
        var persons = Persons();
        var jsonString = JsonSerializer.Serialize(persons);
        await PostAsync(uriString, jsonString);
    }

    [HttpPost("CommitChanges")]
    public async Task CommitChanges()
    {
        var uriString = $"api/collections/persons/config";
        var jsonString = @"{'set-property': { 'updateHandler.autoCommit.maxTime':15000 } }";
        await PostAsync(uriString, jsonString);
    }

    private async Task PostAsync(string uriString, string jsonString)
    {
        try
        {
            var urlString = $"{_configuration["Solr"]}/{uriString}";
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(urlString),
                Headers = { { HttpRequestHeader.ContentType.ToString(), "application/json" } },
                Content = new StringContent(jsonString)
            };
            var response = await _httpClient.SendAsync(httpRequestMessage);
            var responseMessage = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception(responseMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}
