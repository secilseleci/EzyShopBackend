using DataAccess;
using DataAccess.SeedDatabase;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebAPI.ExtensionMethods;
using WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ---------- 1) DI BLOKU ----------
builder.Services.AddCors(p =>
    p.AddPolicy("default", x => x
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("https://localhost:5173") // geçici frontend politikasý
        .AllowCredentials()
    ));

builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureDbContext(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureRepositoryRegistration();
builder.Services.ConfigureServiceRegistration();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerWithJwt();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);

// ---------- 2) APP BLOKU ----------
var app = builder.Build();
app.UseCors("default");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.ConfigureLocalization();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ---------- 3) SEED + RUN ----------
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await SeedDatabase.SeedDatabaseAsync(scope.ServiceProvider);
}

app.Run();
