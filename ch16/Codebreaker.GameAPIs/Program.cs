using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationTelemetry();

// Swagger/EndpointDocumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3", new OpenApiInfo
    {
        Version = "v3",
        Title = "Codebreaker Games API",
        Description = "An ASP.NET Core minimal APIs to play Codebreaker games",
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

builder.Services.AddCors(policy => policy.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseCors();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v3/swagger.json", "v3");
});

_ = app.CreateOrUpdateDatabaseAsync();

app.MapGameEndpoints();

// temporary turn off grpc services
// app.MapGrpcService<GrpcGameEndpoints>();

//string? mode = builder.Configuration["StartupMode"];
//if (mode == "OnPremises")
//{
//    var kafkaproducer = app.Services.GetRequiredService<IProducer<string, string>>();
//    await kafkaproducer.ProduceAsync("gamesummary", new Message<string, string> { Key = "init", Value = "init" });
//}


app.Run();
