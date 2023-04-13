using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SolrExamples.Api.Models;
using SolrExamples.Api.Services;
using System.Text.Json;

namespace SolrExamples.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonQueryController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PersonsController> _logger;

    public PersonQueryController(HttpClient httpClient, IConfiguration configuration, ILogger<PersonsController> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("Persons")]
    public IEnumerable<Person> Persons()
    {

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
