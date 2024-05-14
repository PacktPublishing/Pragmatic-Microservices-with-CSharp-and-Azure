using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Codebreaker.Authentication.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : 
    IdentityDbContext<GamePlayerIdentityUser, IdentityRole, string>(options)
{

}
