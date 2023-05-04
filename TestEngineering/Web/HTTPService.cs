using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Web;

namespace TestEngineering.Web;

public class HTTPService : IHTTPService
{
    public string BaseUri { get; private set; }

    private readonly Lazy<HttpClient> _httpClient;
    private readonly IConfiguration _configuration;

    public HTTPService(string baseUrl)
    {
        _httpClient = new Lazy<HttpClient>(CreateHTTPClient(baseUrl));
    }
    public HTTPService(IConfiguration configuration)
    {
        _configuration = configuration;
        var baseUrl = _configuration[HttpSettingsKeys.URI];
        _httpClient = new Lazy<HttpClient>(CreateHTTPClient(baseUrl));
    }

    private HttpClient CreateHTTPClient(string baseUrl)
    {
        var httpClient = new HttpClient();
        BaseUri = baseUrl;
        httpClient.BaseAddress = new Uri(BaseUri);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }


    public async Task<T> PostAsync<T>(string url, T data)
    {
        var client = _httpClient.Value;
        var response = await client.PostAsJsonAsync(url, data).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>().ConfigureAwait(false);
    }

    public async Task<List<T>> GetAsync<T>(string url, Dictionary<string, string>? parameters = null)
    {
        var client = _httpClient.Value;
        if (parameters != null)
        {
            url = BuildQuery(url, parameters);
        }

        var response = await client.GetAsync(url).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var data = await response.Content.ReadFromJsonAsync<List<T>>().ConfigureAwait(false);
        return data;
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
    {
        var client = _httpClient.Value;
        var response = await client.PutAsJsonAsync(url, data).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return response;
    }


    private string BuildQuery(string url, Dictionary<string, string> parameters)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var parameter in parameters)
        {
            query[parameter.Key] = parameter.Value;
        }
        return $"{url}?{query}";
    }
}

