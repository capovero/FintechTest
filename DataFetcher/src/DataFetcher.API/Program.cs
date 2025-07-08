using Hangfire;
using Hangfire.PostgreSql;
using DataFetcher.API.Services;
using DataFetcher.Application.Interfaces;
using DataFetcher.Application.Jobs;
using DataFetcher.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(cfg => cfg
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage()
);

builder.Services.AddHangfireServer();

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new DecimalStringConverter());
    });

builder.Services.AddHttpClient<IFuturesFetcherService, FuturesFetcherService>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<FetchPricesJob>();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});

RecurringJob.AddOrUpdate<FetchPricesJob>(
    "fetch-prices-every-minute",
    job => job.ExecuteAsync(),
    Cron.Minutely
);

app.Run();