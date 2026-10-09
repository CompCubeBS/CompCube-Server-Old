using System.Text;
using CompCube_Server.Api.BeatSaver;
using CompCube_Server.Data;
using CompCube_Server.Data.Schema;
using Microsoft.AspNetCore.Mvc;

namespace CompCube_Server.Api.Controllers;

[ApiController]
public class AuthenticationApiController(AuthData authData, BeatKhanaService beatKhanaService, ILogger<AuthenticationApiController> logger) : ControllerBase
{
    private static readonly Random Random = new Random();
    
    [Route("/oauth/login")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Login(string returnTo, string mode)
    {
        try
        {
            var bytes = new byte[32];

            Random.NextBytes(bytes);

            var state = Convert.ToBase64String(bytes);

            returnTo = returnTo.StartsWith('/') &&
                       !returnTo.StartsWith("//")
                ? returnTo
                : "/";
            
            var responseMode = mode == "json" ? AuthState.ResponseModeType.Json : AuthState.ResponseModeType.Redirect;
            
            authData.AddOAuthState(state, returnTo, responseMode);
            
            return Redirect(beatKhanaService.AuthorizationUrl(state));
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Failed to start BeatKhana Login");
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }

    [HttpGet]
    [Route("/oauth/callback")]
    public IActionResult Callback()
    {
        
    }
}