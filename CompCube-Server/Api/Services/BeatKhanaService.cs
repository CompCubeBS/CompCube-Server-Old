using System.Buffers.Text;
using System.Text;
using System.Web;
using CompCube_Server.Config;
using CompCube_Server.Models.CompCube_Models.Models.Auth.BeatKhana;
using Newtonsoft.Json;

namespace CompCube_Server.Api.BeatSaver;

public class BeatKhanaService(ConfigHelper config)
{
    private readonly HttpClient _httpClient = new HttpClient();
    
    public string AuthorizationUrl(string state)
    {
        var uri = new UriBuilder(config.BeatKhanaAuthorizationUrl);
        
        var query = HttpUtility.ParseQueryString(uri.Query);
        query["response_type"] = "code";
        query["client_id"] = config.BeatKhanaClientId;
        query["redirect_uri"] = config.BeatKhanaCallbackUrl;
        query["scope"] = config.BeatKhanaScope;
        query["state"] = state;
        
        uri.Query = query.ToString();
        return uri.ToString();
    }

    public async Task<TokenResponse> ExchangeCode(string code)
    {
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{config.BeatKhanaClientId}:{config.BeatKhanaClientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, $"{config.BeatKhanaApiUrl}/oauth/token");
        request.Headers.Add("Authorization", $"Basic {basic}");
        request.Headers.Add("content-type", "application/x-www-form-urlencoded");

        var content = new Dictionary<string, string>()
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", config.BeatKhanaCallbackUrl },
        };
        
        request.Content = new FormUrlEncodedContent(content);
        
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync()) ?? throw new Exception("Could not get token");
    }
    
    private 
}