using FlightAggregator.Services;
using FlightAggregator.Providers.ExternalProviders;
using FlightAggregator.Providers.Interfaces;
using FlightAggregator.Models.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FlightConfiguration>(
    builder.Configuration.GetSection("TestFlights"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddTransient<IFlightAggregatorService, FlightAggregatorService>();

builder.Services.AddTransient<IFlightProvider, FlightProvider1>();
builder.Services.AddTransient<IFlightProvider, FlightProvider2>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();