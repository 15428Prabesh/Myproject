using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var AllowOrigins = "MyOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// Use CORS middleware before Ocelot middleware
app.UseCors(AllowOrigins);

app.MapGet("/", () => "Hello Ocelot");
await app.UseOcelot();
app.Run();
