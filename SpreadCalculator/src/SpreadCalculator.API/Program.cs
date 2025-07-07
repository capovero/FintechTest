using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SpreadCalculator.API.Services;
using SpreadCalculator.Domain.Interfaces;
using SpreadCalculator.Infrastructure;
using SpreadCalculator.Infrastructure.Configurations;
using SpreadCalculator.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1) Настраиваем DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Репозиторий для контроллера
builder.Services.AddScoped<ISpreadRepository, SpreadRepository>();

// 3) Фоновый RabbitMQ‑сервис
builder.Services.AddHostedService<RabbitMqBackgroundService>();

// 4) Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Spread Calculator API", Version = "v1" });
});

// 5) Controllers
builder.Services.AddControllers();

var app = builder.Build();

// 6) Применяем миграции автоматически
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 7) Swagger UI и маршрутизация
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Spread Calculator API v1"));

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();