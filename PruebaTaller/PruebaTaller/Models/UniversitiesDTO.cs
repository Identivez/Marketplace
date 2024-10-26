namespace PruebaTaller.Models
{
    /// <summary>
    /// DTO (Data Transfer Object) que representa la información de una universidad.
    /// Contiene detalles como dominios, estado o provincia, país, páginas web, nombre y código alfabético de dos letras.
    /// </summary>
    public class UniversitiesDTO
    {
        /// <summary>
        /// Obtiene o establece la lista de dominios asociados a la universidad.
        /// </summary>
        public List<string>? Domains { get; set; }

        /// <summary>
        /// Obtiene o establece el estado o provincia de la universidad, si aplica.
        /// </summary>
        public string? StateProvince { get; set; }

        /// <summary>
        /// Obtiene o establece el país donde se encuentra la universidad.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Obtiene o establece la lista de páginas web asociadas a la universidad.
        /// </summary>
        public List<string>? WebPages { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la universidad.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Obtiene o establece el código alfabético de dos letras del país según la norma ISO 3166-1.
        /// </summary>
        public string? AlphaTwoCode { get; set; }
    }
}
