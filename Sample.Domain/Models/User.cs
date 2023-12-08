using System;

namespace Sample.Common.Models;

public class User
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
}