using funclog;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging.Console;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder
            .AddApplicationInsights()
            .AddApplicationInsightsLogger();
    })
    .ConfigureLogging(builder =>
    {
        builder.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Information);
        builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Error);
        
        builder // <-- optional - set a console logger
            // .AddSystemdConsole() <-- alternative console logger
            .AddConsole(console => console.FormatterName = "plain") // <-- custom logger - just for fun
            .AddConsoleFormatter<PlainConsoleFormatter, PlainConsoleFormatterOptions>() // <-- custom logger - just for fun
            ;
        
    })
    .Build();

host.Run();
