using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FlightAggregator.Api.Services;
using FlightAggregator.Api.Providers;

var builder = WebApplication.CreateBuilder(args);

// Регистрируем контроллеры
builder.Services.AddControllers();

// Добавляем Swagger/OpenAPI для документации
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Используем распределённый кэш (на базе in‑memory для демонстрации; можно подключить Redis и др.)
builder.Services.AddDistributedMemoryCache();

// Регистрируем сервис агрегатора
builder.Services.AddSingleton<IFlightAggregatorService, FlightAggregatorService>();

// Регистрируем фиктивные провайдеры (источники внешних данных)
builder.Services.AddSingleton<IFlightProvider, FlightProvider1>();
builder.Services.AddSingleton<IFlightProvider, FlightProvider2>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Роутинг по контроллерам
app.MapControllers();

app.Run();
