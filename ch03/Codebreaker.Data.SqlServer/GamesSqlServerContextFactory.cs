using Microsoft.EntityFrameworkCore.Design;

namespace Codebreaker.Data.SqlServer;

public class GamesSqlServerContextFactory : IDesignTimeDbContextFactory<GamesSqlServerContext>
{
    public GamesSqlServerContext CreateDbContext(string[] args)
    {
        if (args.Length != 1)
        {
            throw new InvalidOperationException("Usage: dotnet ef migrations add <Name> -- <connectionString>");
        }
        string connectionString = args[0];
        var optionsBuilder = new DbContextOptionsBuilder<GamesSqlServerContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new GamesSqlServerContext(optionsBuilder.Options);
    }
}
