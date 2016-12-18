using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjetoApp.Data
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User : DbObject
    {
        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the user's first and last name.
        /// </summary>
        public string Name => $"{FirstName} {LastName}";

        /// <summary>
        /// Gets or sets the user's image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the user's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Returns the user's name,.
        /// </summary>
        public override string ToString() => Name;
    }
}
