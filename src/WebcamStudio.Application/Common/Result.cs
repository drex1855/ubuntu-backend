namespace WebcamStudio.Application.Common;

/// <summary>
/// Resultado de una operacion de negocio que puede fallar por una razon esperada
/// (no un bug): "stock insuficiente", "cuenta desactivada", "periodo invalido", etc.
/// Los servicios de Application devuelven Result/Result&lt;T&gt; en vez de lanzar excepciones
/// para esos casos, porque son parte normal del flujo (las decisiones del diagrama,
/// como "Modelo registrada?" o "Stock bajo?", son ramas esperadas, no errores del sistema).
/// Las excepciones se reservan para fallos inesperados, que atrapa el middleware global.
/// </summary>
public class Result
{
    public bool Succeeded { get; }
    public string? Error { get; }

    protected Result(bool succeeded, string? error)
    {
        Succeeded = succeeded;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool succeeded, T? value, string? error) : base(succeeded, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static new Result<T> Failure(string error) => new(false, default, error);
}
