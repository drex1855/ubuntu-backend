using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace WebcamStudio.Infrastructure.Persistence;

/// <summary>
/// Registra en el log (nivel Warning) cualquier consulta a la base de datos que tarde
/// mas del umbral definido. No reemplaza un APM real, pero da visibilidad basica de
/// consultas lentas/costosas sin agregar una dependencia externa.
/// </summary>
public class SlowQueryLoggingInterceptor : DbCommandInterceptor
{
    private static readonly TimeSpan SlowQueryThreshold = TimeSpan.FromSeconds(1);
    private readonly ILogger<SlowQueryLoggingInterceptor> _logger;

    public SlowQueryLoggingInterceptor(ILogger<SlowQueryLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override ValueTask<System.Data.Common.DbDataReader> ReaderExecutedAsync(
        System.Data.Common.DbCommand command, CommandExecutedEventData eventData,
        System.Data.Common.DbDataReader result, CancellationToken cancellationToken = default)
    {
        WarnIfSlow(command, eventData);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        System.Data.Common.DbCommand command, CommandExecutedEventData eventData,
        int result, CancellationToken cancellationToken = default)
    {
        WarnIfSlow(command, eventData);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void WarnIfSlow(System.Data.Common.DbCommand command, CommandExecutedEventData eventData)
    {
        if (eventData.Duration >= SlowQueryThreshold)
        {
            _logger.LogWarning(
                "Consulta lenta a la base de datos ({DurationMs} ms): {CommandText}",
                eventData.Duration.TotalMilliseconds, command.CommandText);
        }
    }
}
