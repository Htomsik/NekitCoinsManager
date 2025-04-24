namespace NekitCoinsManager.Shared.Constants;

/// <summary>
/// Константы для наименований HTTP-заголовков, используемых в API
/// </summary>
public static class HttpHeaderNames
{
    /// <summary>
    /// Заголовок для идентификатора устройства
    /// </summary>
    public const string HardwareId = "X-Hardware-Id";
    
    /// <summary>
    /// Заголовок для авторизации (токен)
    /// </summary>
    public const string Authorization = "Authorization";
    
    /// <summary>
    /// Префикс для Bearer-токена в заголовке Authorization
    /// </summary>
    public const string BearerPrefix = "Bearer ";
}
