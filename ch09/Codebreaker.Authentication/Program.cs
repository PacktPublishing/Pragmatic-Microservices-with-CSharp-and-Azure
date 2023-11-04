using System.Security.Claims;

using Codebreaker.Authentication.Data;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = "";
    options.UseSqlServer(connectionString);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/identity").MapIdentityApi<GamePlayerIdentityUser>();

app.MapGet("requires-auth", (ClaimsPrincipal user) =>
{
    return TypedResults.Ok($"Hello, {user.Identity?.Name}");
});

app.Run();
