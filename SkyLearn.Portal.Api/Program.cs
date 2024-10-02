using Microsoft.EntityFrameworkCore;
using SkyLearn.Portal.Api.AutoMapper;
using SkyLearn.Portal.Api.DependencyInjection;
using Core.Helper.APiCall;
using Core.Constants;
using SkyLearn.Portal.Api;
using SkyLearn.Portal.Api.Middleware;
using Microsoft.Net.Http.Headers;
using Application.Helpers;
using Core.Helper.Interfaces;
using Core.Helper.Service;
using Microsoft.OpenApi.Models;
using Application;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ApiClient>();
builder.Services.AddSingleton<BaseUrl>();
builder.Services.AddSingleton<FTP>();
builder.Services.AddSingleton<EmailSetting>();
//builder.Services.Configure<BaseUrl>(builder.Configuration.GetSection("BaseUrl"));
builder.Services.Configure<BaseUrl>(options => builder.Configuration.GetSection("BaseUrl").Bind(options));
builder.Services.Configure<FTP>(options => builder.Configuration.GetSection("FTP").Bind(options));
builder.Services.Configure<EmailSetting>(options => builder.Configuration.GetSection("EmailSetting").Bind(options));
//builder.Configuration.GetSection("BaseUrl").Get<BaseUrl>();
//register DbContext
var connectionString = builder.Configuration.GetConnectionString("LocalDB");
builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<IDapperHelper>(c => new DapperHelper(connectionString));
//adding newtonsoftjson for swagger support
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddHttpContextAccessor();
DependencyInjectionConfig.Configure(builder.Services);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging();
//call service classes here
var AllowOrigins = "MyOrigins";


builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

//end

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkyLearn", Version = "v1" });
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "Using the Authorization header with Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securitySchema);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });
    //var xmlFile = "SkyLearn.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});
var app = builder.Build();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{

app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("MyOrigins");
app.UseAuthorization();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMyCustomMiddleware();
app.MapControllers();

app.Run();

