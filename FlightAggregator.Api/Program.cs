using FlightAggregator.Services;
using FlightAggregator.Providers.ExternalProviders;
using FlightAggregator.Providers.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<IFlightAggregatorService, FlightAggregatorService>();

builder.Services.AddSingleton<IFlightProvider, FlightProvider1>();
builder.Services.AddSingleton<IFlightProvider, FlightProvider2>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
