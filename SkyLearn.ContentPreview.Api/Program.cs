using Microsoft.EntityFrameworkCore;
using SkyLearn.ContentPreview.Api.DependencyInjections;
using SkyLearn.ContentPreview.Api.AutoMapper;
using SkyLearn.ContentPreview.Api;
using Application.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//register DbContext
var connectionString = builder.Configuration.GetConnectionString("LocalDB");
builder.Services.AddDbContext<ContentContextPreview>(options => options.UseSqlServer(connectionString));


DependencyInjections.Configure(builder.Services);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var AllowOrigins = "MyOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:3000", "https://localhost:7083")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging();
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseStaticFiles();
app.UseCors(AllowOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();
app.UseRouting();
app.Run();
