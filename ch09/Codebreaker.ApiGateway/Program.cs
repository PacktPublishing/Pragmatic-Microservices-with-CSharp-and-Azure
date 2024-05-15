using Codebreaker.Proxy;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));
    //.EnableTokenAcquisitionToCallDownstreamApi()
    //.AddDownstreamApi("", "")
    //.AddInMemoryTokenCache();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(bearerOptions =>
//    {
//        builder.Configuration.Bind("AzureAdB2C", bearerOptions);
//        //        bearerOptions.TokenValidationParameters.NameClaimType = "name";
//    }, identityOptions =>
//    {
//        builder.Configuration.Bind("AzureAdB2C", identityOptions);
//    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("playPolicy", config =>
    {
        config.RequireScope("Games.Play");
    })
   .AddPolicy("queryPolicy", config =>
   {
        config.RequireScope("Games.Query");
        config.RequireAuthenticatedUser();
   })
   .AddPolicy("botPolicy", config =>
   {
       config.RequireScope("Bot.Play");
    //   config.RequireClaim("role", [ "Bot" ]);
       config.RequireAuthenticatedUser();
   });

builder.AddServiceDefaults();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();


// builder.Services.AddRazorPages(); // authentication UI in Views

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapGet("/", () => "YARP Proxy");

app.Run();

// https://github.com/microsoft/woodgrove-groceries
// https://github.com/davidfowl/AspireYarp

