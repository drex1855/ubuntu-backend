namespace WebcamStudio.Application.Common;

/// <summary>
/// Sobre de respuesta estandar para TODOS los endpoints de la API.
/// Corresponde al nodo "Generar respuesta para el frontend" (H) del diagrama:
/// sin importar por cual de los 5 modulos entro la solicitud, el frontend siempre
/// recibe la misma forma de respuesta, lo que simplifica muchisimo el consumo desde
/// el cliente (un solo interceptor/parser para todo el sistema, sin importar cuantos
/// modulos nuevos se agreguen despues).
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors?.ToList() ?? new List<string>() };
}
