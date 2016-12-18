using Newtonsoft.Json;
using System;

namespace ProjetoApp.Data
{
    /// <summary>
    /// Represents a characteristic item on a building.
    /// </summary>
    public class Characteristic : DbObject
    {

        /// <summary>
        /// Gets or sets the id of the order the line item is associated with.
        /// </summary>
        public Guid BuildingId { get; set; }

        /// <summary>
        /// Gets or sets the building the characteristic item is associated with.
        /// </summary>
        [JsonIgnore]
        public Building Building { get; set; }

        /// <summary>
        /// Gets or sets the characteristic's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the characteristic's type.
        /// </summary>
        public CharacteristicType Type { get; set; } = CharacteristicType.Soccer;

    }

    /// <summary>
    /// Represents the term for an order.
    /// </summary>
    public enum CharacteristicType
    {
        Volley,
        Barbacue,
        Soccer,
        Elevators
    }

}