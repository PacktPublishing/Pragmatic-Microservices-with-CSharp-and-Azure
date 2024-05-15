using Microsoft.AspNetCore.Identity;

namespace Codebreaker.Authentication.Data;

public class GamePlayerIdentityUser : IdentityUser
{
    [PersonalData]
    public string? GamerName { get; set; }
}
