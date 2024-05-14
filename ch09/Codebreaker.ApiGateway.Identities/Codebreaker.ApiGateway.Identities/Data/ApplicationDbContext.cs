using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Codebreaker.ApiGateway.Identities.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var applicationUser = builder.Entity<ApplicationUser>();
        applicationUser
            .Property(u => u.GamerName)
            .HasColumnType("varchar(56)")
            .HasMaxLength(56);

        applicationUser
            .Property(u => u.Id)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        applicationUser
            .Property(u => u.NormalizedEmail)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        applicationUser
            .Property(u => u.NormalizedUserName)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        var identityRole = builder.Entity<IdentityRole>();
        identityRole
            .Property(r => r.Id)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        identityRole
            .Property(r => r.NormalizedName)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        var identityUserClaim = builder.Entity<IdentityUserClaim<string>>();
        identityUserClaim
            .Property(uc => uc.UserId)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        var identityRoleClaim = builder.Entity<IdentityRoleClaim<string>>();
        identityRoleClaim
            .Property(rc => rc.RoleId)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256); 
        var identityUserLogin = builder.Entity<IdentityUserLogin<string>>();
        identityUserLogin
            .Property(l => l.ProviderKey)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        identityUserLogin
            .Property(l => l.LoginProvider)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        identityUserLogin
            .Property(l => l.UserId)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        var identityUserToken = builder.Entity<IdentityUserToken<string>>();
        identityUserToken
            .Property(t => t.LoginProvider)
            .HasColumnType("varchar(256)")
            .HasMaxLength(56);
        identityUserToken
            .Property(t => t.UserId)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        identityUserToken
            .Property(t => t.Name)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        var identityUserRole = builder.Entity<IdentityUserRole<string>>();
        identityUserRole
            .Property(r => r.UserId)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
        identityUserRole
            .Property(r => r.RoleId)
            .HasColumnType("varchar(256)")
            .HasMaxLength(256);
    }
}
