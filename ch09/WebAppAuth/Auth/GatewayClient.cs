namespace WebAppAuth.Auth;

public class GatewayClient(HttpClient client)
{
    public async Task<string> TestAsync()
    {
        var response = await client.GetAsync("/testauth");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
