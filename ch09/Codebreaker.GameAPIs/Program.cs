using Codebreaker.GameAPIs;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(bearerOptions =>
    {
        builder.Configuration.Bind("AzureAdB2C", bearerOptions);
//        bearerOptions.TokenValidationParameters.NameClaimType = "name";
    }, identityOptions =>
    {
        builder.Configuration.Bind("AzureAdB2C", identityOptions);
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("playPolicy", config =>
    {
        config.RequireScope("Games.Play");      
    });
    options.AddPolicy("queryPolicy", config =>
    {
        config.RequireScope("Games.Query");
        config.RequireAuthenticatedUser();

        // config.RequireClaim("userType", [ "PowerUser" ]);
    });
});

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
            Name = "License API Usage",
            Url = new Uri("https://www.cninnovation.com/apiusage")
        }
    });
});

// Application Services

builder.AddApplicationServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

app.MapGameEndpoints();

await app.CreateOrUpdateDatabaseAsync();

app.Run();
