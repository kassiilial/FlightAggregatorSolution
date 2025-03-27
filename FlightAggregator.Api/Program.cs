using FlightAggregator.Api.Specification;
using FlightAggregator.Models.Configurations;
using FlightAggregator.Providers.ExternalProviders;
using FlightAggregator.Providers.Interfaces;
using FlightAggregator.Services;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<FlightConfiguration>(
    builder.Configuration.GetSection("TestFlights"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<FlightRequestExample>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<IFlightAggregatorService, FlightAggregatorService>();
builder.Services.AddTransient<IFlightProvider, FlightProvider1>();
builder.Services.AddTransient<IFlightProvider, FlightProvider2>();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();