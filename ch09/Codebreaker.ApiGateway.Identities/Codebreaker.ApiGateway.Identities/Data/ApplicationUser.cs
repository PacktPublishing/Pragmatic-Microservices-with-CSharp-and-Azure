using Microsoft.AspNetCore.Identity;

namespace Codebreaker.ApiGateway.Identities.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    public string? GamerName { get; set; }
}
