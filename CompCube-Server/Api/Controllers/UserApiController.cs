using CompCube_Models.Models.ClientData;
using CompCube_Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace CompCube_Server.Api.Controllers;

[ApiController]
public class UserApiController(UserData userData) : ControllerBase
{
    [HttpGet("/api/user/id/{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<UserStatistics> GetUserById(string id)
    {
        var user = userData.GetUserStatisticsByPlatformId(id);
        
        if (user == null) 
            return NotFound();

        return user;
    }
}