using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

/// <summary>
/// Clase estática que proporciona métodos de extensión para manejar objetos JSON en la sesión de ASP.NET Core.
/// Permite almacenar y recuperar objetos complejos en la sesión mediante serialización y deserialización JSON.
/// </summary>
public static class SessionHelper
{
    /// <summary>
    /// Extensión que guarda un objeto como una cadena JSON en la sesión.
    /// </summary>
    /// <param name="session">La sesión de ASP.NET Core donde se almacenará el objeto.</param>
    /// <param name="key">La clave con la que se identificará el objeto en la sesión.</param>
    /// <param name="value">El objeto que se guardará en la sesión.</param>
    public static void SetObjectAsJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    /// <summary>
    /// Extensión que recupera un objeto de la sesión y lo deserializa desde una cadena JSON.
    /// </summary>
    /// <typeparam name="T">El tipo del objeto que se espera recuperar.</typeparam>
    /// <param name="session">La sesión de ASP.NET Core desde la que se recuperará el objeto.</param>
    /// <param name="key">La clave con la que se identificó el objeto en la sesión.</param>
    /// <returns>El objeto deserializado, o el valor predeterminado del tipo si no se encuentra.</returns>
    public static T GetObjectFromJson<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
    }
}
