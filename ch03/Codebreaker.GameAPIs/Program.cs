

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo
    {
        Version = "v3",
        Title = "Codebreaker Games API",
        Description = "An ASP.NET Core minimal API to play Codebreaker games",
        TermsOfService = new Uri("https://www.cninnovation.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Christian Nagel",
            Url = new Uri("https://csharp.christiannagel.com")
        },
        License = new OpenApiLicense
        {
            Name="License API Usage",
            Url= new Uri("https://www.cninnovation.com/apiusage")
        }
    });
});

// Application Services

builder.AddApplicationServices();

builder.Services.AddScoped<IGamesService, GamesService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
    });
}

// Update or create the SQL Server database 
string? dataStore = builder.Configuration.GetValue<string>("DataStore");
if (dataStore == "SqlServer")
{
    using var scope = app.Services.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
    if (service is DataContextProxy<GamesSqlServerContext> proxy)
    {
        await proxy.UpdateDatabaseAsync(app.Logger);
    }
}

app.MapGameEndpoints();

app.Run();
