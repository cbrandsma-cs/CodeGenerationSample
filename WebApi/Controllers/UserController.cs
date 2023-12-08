using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services.UserServices;

namespace WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(
    IUserViewService viewService
    ) : ControllerBase
{
    [HttpGet("currentuser", Name = "UserFetchCurrentUser")]
    public async Task<ActionResult<UserViewModel?>> GetCurrentUser()
    {
        var user = await viewService.FetchCurrentUser();
        
        return user;
    }
}