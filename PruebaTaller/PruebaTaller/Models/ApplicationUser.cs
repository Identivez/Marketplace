﻿using Microsoft.AspNetCore.Identity;

namespace PruebaTaller.Models
{
    /// <summary>
    /// Clase que representa un usuario de la aplicación, extendiendo la clase IdentityUser de ASP.NET Core Identity.
    /// Contiene propiedades adicionales como nombre, apellido, dirección y fecha de creación del usuario.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Obtiene o establece el primer nombre del usuario.
        /// </summary>
        public string FirstName { get; set; } = "";

        /// <summary>
        /// Obtiene o establece el apellido del usuario.
        /// </summary>
        public string LastName { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la dirección del usuario.
        /// </summary>
        public string Address { get; set; } = "";

        /// <summary>
        /// Obtiene o establece la fecha y hora de creación del usuario.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}