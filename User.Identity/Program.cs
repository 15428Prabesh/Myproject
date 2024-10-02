using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using User.Identity.Authorization;
using User.Identity.Data;
using User.Identity.Herlpers;
using User.Identity.Services;
using Microsoft.AspNetCore.Cors;
using User.Identity.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserIdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserIdentityContext") ?? throw new InvalidOperationException("Connection string 'UserIdentityContext' not found.")));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// configure strongly typed settings object
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
// configure DI for application services
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();

//app.UseCors(AllowOrigins);
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors("AllowSpecificOrigin");
// custom jwt auth middleware
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<JwtMiddleware>();
app.MapControllers();

app.Run();
