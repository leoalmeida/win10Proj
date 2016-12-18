using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ProjetoApp.Data
{
    /// <summary>
    /// Represents a customer.
    /// </summary>
    public class Customer : DbObject
    {
        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the customer's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the customer's first and last name.
        /// </summary>
        public string Name => $"{FirstName} {LastName}"; 

        /// <summary>
        /// Gets or sets the customer's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the customer's phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the customer's address. 
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the customer's address. 
        /// </summary>
        public string RG { get; set; }

        /// <summary>
        /// Gets or sets the customer's address. 
        /// </summary>
        public string CPF { get; set; }

        /// <summary>
        /// Gets or sets the customer's relation. 
        /// </summary>
        public string Relation { get; set; }

        /// <summary>
        /// Gets or sets when the order was placed.
        /// </summary>
        public DateTime DatePlaced { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets when the order was filled.
        /// </summary>
        public DateTime? DateFilled { get; set; } = null;

        /// <summary>
        /// A list of the customer's buildings. 
        /// </summary>
        [JsonIgnore]
        public virtual List<Building> Buildings { get; set; }

        /// <summary>
        /// Returns the customer's name,.
        /// </summary>
        public override string ToString() => Name;
    }
}
