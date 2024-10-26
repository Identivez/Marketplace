namespace PruebaTaller.Models
{
    /// <summary>
    /// Modelo de vista que representa los datos de un error en la aplicación.
    /// Contiene información relevante para identificar la solicitud en la que ocurrió el error.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la solicitud en la que ocurrió el error.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indica si se debe mostrar el identificador de la solicitud.
        /// Devuelve true si el identificador de la solicitud no es nulo ni está vacío.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
