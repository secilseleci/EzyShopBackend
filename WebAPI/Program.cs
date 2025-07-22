using WebAPI.Middlewares;
using WebAPI.ExtensionMethods;
using DataAccess;
using DataAccess.SeedDatabase;
using Microsoft.EntityFrameworkCore;   // eski adý buysa, namespace’i güncelle

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ---------- 1) DI BLOKU ----------
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ---------- 3) SEED + RUN ----------
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await SeedDatabase.SeedDatabaseAsync(scope.ServiceProvider);
}

app.Run();
