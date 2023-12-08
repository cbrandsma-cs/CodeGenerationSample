using Sample.Generator;

namespace WebApi.Services.UserServices;


public class UserViewModel
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
}