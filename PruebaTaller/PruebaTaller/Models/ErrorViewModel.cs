namespace PruebaTaller.Models
{
    /// <summary>
    /// Modelo de vista que representa los datos de un error en la aplicaci�n.
    /// Contiene informaci�n relevante para identificar la solicitud en la que ocurri� el error.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la solicitud en la que ocurri� el error.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indica si se debe mostrar el identificador de la solicitud.
        /// Devuelve true si el identificador de la solicitud no es nulo ni est� vac�o.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
