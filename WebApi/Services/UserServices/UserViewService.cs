using Sample.Common;

namespace WebApi.Services.UserServices;

public interface IUserViewService
{
    Task<UserViewModel?> FetchCurrentUser();
}

[AddToScope]
public class UserViewService : IUserViewService
{
    public async Task<UserViewModel?> FetchCurrentUser()
    {
        return new UserViewModel
        {
            Id = new Guid("3BDA17A3-9301-42F1-AB79-E5190936F285"),
            DisplayName = "Chris Brandsma",
            Email = "cbrandsma@cspace.com"
        };
    }
}