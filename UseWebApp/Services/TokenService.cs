using Newtonsoft.Json;
using System.Text;
using UseWebApp.IServices;
using UseWebApp.Models.Response;

namespace UseWebApp.Services;

public class TokenService : ITokenService
{
    private readonly HttpClient _tokenHttpClient;
    private readonly HttpClient _secretKeyHttpClient;
    private readonly IConfiguration _configuration;
    public TokenService(IHttpClientFactory httpClient, IConfiguration configuration)
    {
        _tokenHttpClient = httpClient.CreateClient("Token");
        _secretKeyHttpClient = httpClient.CreateClient("SecretKey");
        _configuration = configuration;
    }

    public async Task<string?> GetToken()
    {
        string? token = "";
        var bodyRequest = _configuration.GetSection("Token").GetChildren()
            .Where(settings => settings.Key == "clientId" || settings.Key == "secretName")
            .ToDictionary(settings => settings.Key, settings => settings.Value);           
        //          examplebodyRequest
        //      clientId   | zxcvasdqwefasdasd=
        //      secretName | MySecretName

        var request = new HttpRequestMessage(HttpMethod.Post, _tokenHttpClient.BaseAddress); // Post Method
        request.Headers.Add("Accept", "application/json"); // HeaderRequest
        var json = JsonConvert.SerializeObject(bodyRequest, Formatting.Indented);
        request.Content = new StringContent(json,Encoding.UTF8, "application/json"); // BodyRequest
        var response = await _tokenHttpClient.SendAsync(request);
        if(response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<ResponseToken>(responseBody);
            token = jsonResponse?.Data?.Token;
        }
        return token;
    }

    public async Task<string?> GetKey()
    {
        string key = "";
        var clientId = _configuration["Token:clientId"] ?? "";
        var systemname = _configuration["Token:secretName"] ?? "";

        var request =new HttpRequestMessage(HttpMethod.Get, _secretKeyHttpClient.BaseAddress);  // GetMethod
        request.Headers.Add("ClientId", clientId);
        request.Headers.Add("SecretName", systemname);   // HeaderRequest
        request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var response = await _secretKeyHttpClient.SendAsync(request);
        if(response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<SecretKeyResponse>(responseBody);
            key = jsonResponse?.Data ?? "";
        }
        return key;
    }
}
