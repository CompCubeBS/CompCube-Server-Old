using System.Web;
using CompCube_Server.Config;

namespace CompCube_Server.Api.BeatSaver;

public class BeatKhanaService(ConfigHelper config)
{
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
}