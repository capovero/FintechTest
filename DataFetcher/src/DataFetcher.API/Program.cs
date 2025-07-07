using DataFetcher.API.Services;
using DataFetcher.Application.Interfaces;
using DataFetcher.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using DataFetcher.Application.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DecimalStringConverter());
    });

builder.Services.AddHttpClient<IFuturesFetcherService, FuturesFetcherService>();

builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

builder.Services.AddScoped<FetchPricesJob>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();