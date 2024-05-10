using BlazorWASMAuth.Client;

// using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMsalAuthentication(options =>
{
   options.ProviderOptions.LoginMode = "redirect";
    options.ProviderOptions.DefaultAccessTokenScopes.Add("TODO: complete path Game.Play");
});

builder.Services.AddHttpClient<BotClient>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);   
})
.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();   

//builder.Services.Configure<JwtBearerOptions>(
//    JwtBearerDefaults.AuthenticationScheme, options =>
//    {
//        options.TokenValidationParameters.NameClaimType = "name";
//    });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

await builder.Build().RunAsync();


public class BotClient(HttpClient client)
{

}