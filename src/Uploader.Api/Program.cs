

using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using Uploader.Api;
using Uploader.Api.Middlewares;
using Uploader.Api.Services;
using Amazon.S3;
using Amazon.S3.Transfer;
using Google.Api;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Uploader.Api.AppSettings;
using Amazon.Extensions.NETCore.Setup;
using Dapr.Client;
using Dapr.Extensions.Configuration;

var daprClient = new DaprClientBuilder().Build();

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
                )
                .ConfigureAppConfiguration(config =>
                {
                    // Get the initial value from the configuration component.
                    config.AddDaprConfigurationStore("configstore", new List<string>() { "orderId1", "orderId2" }, daprClient, TimeSpan.FromSeconds(20));

                    // Watch the keys in the configuration component and update it in local configurations.
                    config.AddStreamingDaprConfigurationStore("configstore", new List<string>() { "orderId1", "orderId2" }, daprClient, TimeSpan.FromSeconds(20));
                });

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = Int32.MaxValue; // if don't set default value is: 30 MB
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MemoryBufferThreshold = Int32.MaxValue;
    options.MultipartBoundaryLengthLimit = Int32.MaxValue;
    options.MultipartBodyLengthLimit = Int32.MaxValue;
    options.MultipartHeadersLengthLimit = Int32.MaxValue;
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOptions<AuthSettings>().BindConfiguration("AuthSettings");
builder.Services.AddOptions<StorageSettings>().BindConfiguration("StorageSettings");

builder.Services.AddScoped<IStorageService, StorageService>();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddTransient<TransferUtility>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    Console.WriteLine(app.Configuration.GetValue<string>("orderId1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

//app.UseJwtParser();

app.MapControllers();

app.Run();
