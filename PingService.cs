namespace ServiceBeat;

public class PingService(IHttpClientFactory httpClientFactory)
{
    public async Task<string> PingAsync(string url)
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
}