namespace PruebaTaller.Models
{
    /// <summary>
    /// Modelo de búsqueda para la tienda en línea. 
    /// Representa los criterios de búsqueda y los resultados disponibles para la selección de productos.
    /// Este modelo se utiliza para filtrar y ordenar productos en la tienda según la marca, categoría, y otros criterios.
    /// </summary>
    public class StoreSearchModel
    {
        /// <summary>
        /// Texto de búsqueda introducido por el usuario.
        /// Permite filtrar productos basados en un término específico.
        /// </summary>
        public string? Search { get; set; }

        /// <summary>
        /// Marca seleccionada por el usuario.
        /// Permite filtrar productos según la marca elegida.
        /// </summary>
        public string? SelectedBrand { get; set; }

        /// <summary>
        /// Categoría seleccionada por el usuario.
        /// Permite filtrar productos según la categoría elegida.
        /// </summary>
        public string? SelectedCategory { get; set; }

        /// <summary>
        /// Tipo de ordenamiento seleccionado por el usuario.
        /// Permite ordenar productos por diferentes criterios, como precio, popularidad, etc.
        /// </summary>
        public string? Sort { get; set; }

        /// <summary>
        /// Lista de marcas disponibles para seleccionar.
        /// Esta lista se rellena dinámicamente con las marcas disponibles en la tienda.
        /// </summary>
        public List<string>? Brands { get; set; }

        /// <summary>
        /// Lista de categorías disponibles para seleccionar.
        /// Esta lista se rellena dinámicamente con las categorías disponibles en la tienda.
        /// </summary>
        public List<string>? Categories { get; set; }

        /// <summary>
        /// Lista de productos filtrados según los criterios de búsqueda.
        /// Esta lista se rellena con los productos resultantes de los filtros aplicados.
        /// </summary>
        public List<Product>? Products { get; set; }
    }
}
