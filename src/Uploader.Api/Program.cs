

using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Uploader.Api;
using Uploader.Api.Middlewares;


Log.Logger = new LoggerConfiguration().MinimumLevel
               .Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
               .MinimumLevel.Override("System", LogEventLevel.Warning)
               .MinimumLevel.Override(
                   "Microsoft.AspNetCore.Authentication",
                   LogEventLevel.Information
               )
               .Enrich.FromLogContext()
               .WriteTo.Console(
                   outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                   theme: AnsiConsoleTheme.Code
               )
               .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseMetricsWebTracking()
                .UseMetrics(
                    option =>
                    {
                        option.EndpointOptions = endpointOption =>
                        {
                            endpointOption.MetricsTextEndpointOutputFormatter =
                                new MetricsPrometheusTextOutputFormatter();
                            endpointOption.MetricsEndpointOutputFormatter =
                                new MetricsPrometheusProtobufOutputFormatter();
                            endpointOption.EnvironmentInfoEndpointEnabled = false;
                        };
                    }
                );

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseJwtParser();

app.MapControllers();

app.Run();
