using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SolrExamples.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SolrCollectionController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SolrCollectionController> _logger;
    private const string collection = "persons";

    public SolrCollectionController(HttpClient httpClient, IConfiguration configuration, ILogger<SolrCollectionController> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
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
                        'name': 'techproducts',
                        'numShards': 3,
                        'shards': 'shard-person-1,shard-person-2,shard-person-3'
                        'replicationFactor': 1
                      }
                    }";
        await PostAsync(uriString, jsonString);
    }

    /// <summary>
    /// DefineSchema
    /// </summary>
    /// <returns></returns>
    [HttpPost("DefineSchema")]
    public async Task DefineSchema()
    {
        var uriString = $"api/collections/{collection}/schema";
        var jsonString = @"{
                      'add-field': [
                        {'name': 'name', 'type': 'text_general', 'multiValued': false},
                        {'name': 'cat', 'type': 'string', 'multiValued': true},
                        {'name': 'manu', 'type': 'string'},
                        {'name': 'features', 'type': 'text_general', 'multiValued': true},
                        {'name': 'weight', 'type': 'pfloat'},
                        {'name': 'price', 'type': 'pfloat'},
                        {'name': 'popularity', 'type': 'pint'},
                        {'name': 'inStock', 'type': 'boolean', 'stored': true},
                        {'name': 'store', 'type': 'location'}
                      ]
                    }";
        await PostAsync(uriString, jsonString);
    }

    /// <summary>
    /// IndexMultipleData
    /// </summary>
    /// <returns></returns>
    [HttpPost("IndexMultipleData")]
    public async Task IndexMultipleData()
    {
        var uriString = $"api/collections/{collection}/update";
        var jsonString = @"[
                      {
                        'id' : '978-0641723445',
                        'cat' : ['book','hardcover'],
                        'name' : 'The Lightning Thief',
                        'author' : 'Rick Riordan',
                        'series_t' : 'Percy Jackson and the Olympians',
                        'sequence_i' : 1,
                        'genre_s' : 'fantasy',
                        'inStock' : true,
                        'price' : 12.50,
                        'pages_i' : 384
                      }
                    ,
                      {
                        'id' : '978-1423103349',
                        'cat' : ['book','paperback'],
                        'name' : 'The Sea of Monsters',
                        'author' : 'Rick Riordan',
                        'series_t' : 'Percy Jackson and the Olympians',
                        'sequence_i' : 2,
                        'genre_s' : 'fantasy',
                        'inStock' : true,
                        'price' : 6.49,
                        'pages_i' : 304
                      }
                    ]";
        await PostAsync(uriString, jsonString);
    }

    /// <summary>
    /// CommitChanges
    /// </summary>
    /// <returns></returns>
    [HttpPost("CommitChanges")]
    public async Task CommitChanges()
    {
        var uriString = $"api/collections/{collection}/config";
        var jsonString = @"{'set-property': { 'updateHandler.autoCommit.maxTime':15000 } }";
        await PostAsync(uriString, jsonString);
    }

    /// <summary>
    /// CreateShard
    /// </summary>
    /// <returns></returns>
    [HttpPost("CreateShard")]
    public async Task CreateShard()
    {
        var uriString = $"api/collections/{collection}/shards";
        var jsonString = @"
                      {
                        'create':{
                          'shard':'myShard1'
                        }
                      }";
        await PostAsync(uriString, jsonString);
    }


    /// <summary>
    /// QueryAll
    /// </summary>
    /// <returns></returns>
    [HttpPost("QueryAll")]
    public async Task<string> QueryAll()
    {
        var uriString = @$"solr/{collection}/select?indent=true&q=*:*";
        return await QueryAsync(uriString);
    }

    /// <summary>
    /// QueryValue
    /// </summary>
    /// <returns></returns>
    [HttpPost("QueryValue")]
    public async Task<string> QueryValue()
    {
        var uriString = @$"solr/{collection}/select?indent=true&q=hardcover";
        return await QueryAsync(uriString);
    }

    /// <summary>
    /// QueryFieldValue
    /// </summary>
    /// <returns></returns>
    [HttpPost("QueryFieldValue")]
    public async Task<string> QueryFieldValue()
    {
        var uriString = @$"solr/{collection}/select?indent=true&q=hardcover&fl=id";
        return await QueryAsync(uriString);
    }

    /// <summary>
    /// SendGet
    /// </summary>
    /// <param name="uriString"></param>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    private async Task<string> QueryAsync(string uriString)
    {
        try
        {
            var urlString = $"{_configuration["Solr"]}/{uriString}";
            return await _httpClient.GetStringAsync(urlString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    /// <summary>
    /// SendPost
    /// </summary>
    /// <param name="uriString"></param>
    /// <param name="jsonString"></param>
    /// <returns></returns>
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
            await _httpClient.SendAsync(httpRequestMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}
