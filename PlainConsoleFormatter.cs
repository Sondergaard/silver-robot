using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace funclog;

public class PlainConsoleFormatterOptions : ConsoleFormatterOptions
{
}

public class PlainConsoleFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable _optionsReloadToken;
    private PlainConsoleFormatterOptions _formatterOptions;

    public static readonly string FormatterName = "plain";
    
    public PlainConsoleFormatter(IOptionsMonitor<PlainConsoleFormatterOptions> options) : base(FormatterName)
        => (_optionsReloadToken, _formatterOptions) =
            (options.OnChange(ReloadLoggerOptions), options.CurrentValue);
    
    private void ReloadLoggerOptions(PlainConsoleFormatterOptions options) =>
        _formatterOptions = options;

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (logEntry.Exception == null && message == null)
        {
            return;
        }
        
        textWriter.Write(GetForegroundColor(logEntry));
        textWriter.WriteLine(message);
        
    }

    private static string GetForegroundColor<TState>(LogEntry<TState> entry)
        => entry.LogLevel switch
        {
            LogLevel.Information => "\x1B[1m\x1B[32m",
            LogLevel.Warning => "\x1B[1m\x1B[33m",
            LogLevel.Error => "\x1B[1m\x1B[35m",
            LogLevel.Critical => "\x1B[1m\x1B[31m",
            _ => "\x1B[1m\x1B[36m"
        };
    
    public void Dispose() => _optionsReloadToken?.Dispose();
}