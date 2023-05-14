namespace TestEngineering.Interfaces;

public interface IHTTP
{
    public Task<T> PostAsync<T>(string url, T data);
    public Task<List<T>> GetAsync<T>(string url, Dictionary<string, string>? parameters = null);
    public Task<HttpResponseMessage> PutAsync<T>(string url, T data);
}