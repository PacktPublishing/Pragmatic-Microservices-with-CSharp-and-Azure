
using Codebreaker.Data.Sqlite;

using Microsoft.Extensions.Configuration;

ConfigurationManager configurationManager = new();
configurationManager.AddJsonFile("appsettings.json");
string sqliteConnection = configurationManager.GetConnectionString("GamesSqliteConnection") ?? throw new InvalidOperationException("Could not read GamesSqliteConnection");

using GamesSqliteContext context = new(sqliteConnection);
bool created = context.Database.EnsureCreated();
string text = created ? "created" : "already exists";
Console.WriteLine($"database {text}");

