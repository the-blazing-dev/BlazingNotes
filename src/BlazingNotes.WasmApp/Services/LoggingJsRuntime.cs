using System.Diagnostics;
using BlazingDev.BlazingExtensions;
using Microsoft.JSInterop;

namespace BlazingNotes.WasmApp.Services;

public class LoggingJsRuntime(IJSRuntime inner, ILogger<LoggingJsRuntime> logger)
    : IJSRuntime
{
    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        var withArgsString = GetWithArgsString(args);

        logger.LogInformation($"Start of '{identifier}'{withArgsString}");
        var sw = Stopwatch.StartNew();
        var result = await inner.InvokeAsync<TValue>(identifier, args);
        sw.Stop();
        logger.LogInformation($"End of '{identifier}'{withArgsString}, took {sw.Elapsed}");
        return result;
    }

    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken,
        object?[]? args)
    {
        var withArgsString = GetWithArgsString(args);

        logger.LogInformation($"Start of '{identifier}'{withArgsString}");
        var sw = Stopwatch.StartNew();
        var result = await inner.InvokeAsync<TValue>(identifier, cancellationToken, args);
        sw.Stop();
        logger.LogInformation($"End of '{identifier}'{withArgsString}, took {sw.Elapsed}");
        return result;
    }

    private static string GetWithArgsString(object?[]? args)
    {
        var argsString = args is null ? "" :
            args.Length == 0 ? "" :
            args.Length == 1 ? args[0]?.ToString() :
            $"Length={args.Length}";

        var withArgsString = argsString.HasContent() ? $" with args '{argsString}'" : "";
        return withArgsString;
    }
}