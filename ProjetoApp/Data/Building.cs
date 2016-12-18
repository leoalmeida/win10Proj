using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoApp.Data
{
    /// <summary>
    /// Represents a building.
    /// </summary>
    public class Building : DbObject
    {
        /// <summary>
        /// Creates a new order.
        /// </summary>
        public Building()
        { }

        /// <summary>
        /// Creates a new order for the given customer.
        /// </summary>
        public Building(Customer customer) : this()
        {
            Customer = customer;
            CustomerName = $"{customer.FirstName} {customer.LastName}";
        }

        /// <summary>
        /// Gets or sets the id of the customer placing the order.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer placing the order.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer placing the order.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the building's street type.
        /// </summary>
        public string AddressType { get; set; }

        /// <summary>
        /// Gets or sets the building's address street.
        /// </summary>
        public string AddressStreet { get; set; }

        /// <summary>
        /// Gets or sets the building's address street number.
        /// </summary>
        public string AddressStreetNumber { get; set; }

        /// <summary>
        /// Gets or sets the building's address complement.
        /// </summary>
        public string AddressComplement { get; set; }

        /// <summary>
        /// Gets or sets the building's address street neigborhood.
        /// </summary>
        public string AddressNeigborhood { get; set; }

        /// <summary>
        /// Gets or sets the building's address street city.
        /// </summary>
        public string AddressCity { get; set; }

        /// <summary>
        /// Gets or sets the building's address street state.
        /// </summary>
        public string AddressState { get; set; }

        /// <summary>
        /// Gets or sets the building's address street country.
        /// </summary>
        public string AddressCountry { get; set; }

        /// <summary>
        /// Gets or sets the order's status.
        /// </summary>
        public BuildingType Type { get; set; } = BuildingType.Sell;
        
        /// <summary>
        /// Gets or sets the characteristics of the building.
        /// </summary>
        public virtual List<Characteristic> Characteristics { get; set; } = new List<Characteristic>();


        /// <summary>
        /// Gets or sets the items in the Building.
        /// </summary>
        public virtual List<Building> Buildings { get; set; } = new List<Building>();

        /// <summary>
        /// Gets or sets the size the building.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the size the building.
        /// </summary>
        public int Rooms { get; set; }

        /// <summary>
        /// Gets or sets the size the building.
        /// </summary>
        public int Baths { get; set; }

        /// <summary>
        /// Gets or sets the size the building.
        /// </summary>
        public int ParkingLots { get; set; }

        /// <summary>
        /// Gets or sets the size the building.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the Floor of the building.
        /// </summary>
        public int Floor { get; set; }

        /// <summary>
        /// Gets or sets the building's standard cost.
        /// </summary>
        public decimal MarketValue { get; set; }

        /// <summary>
        /// Gets or sets the building's list price.
        /// </summary>
        public decimal ListedValue { get; set; }

        /// <summary>
        /// Gets or sets the order's status.
        /// </summary>
        public BuildingStatus Status { get; set; } = BuildingStatus.Void;

        /// <summary>
        /// Gets or sets when the order was placed.
        /// </summary>
        public DateTime DatePlaced { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets when the order was filled.
        /// </summary>
        public DateTime? DateFilled { get; set; } = null;

        /// <summary>
        /// Returns the name of the building and the list price.
        /// </summary>
        public override string ToString() => $"{AddressType} {AddressStreet} {AddressStreetNumber} \n{ListedValue}";
    }

    /// <summary>
    /// Represents the term for an order.
    /// </summary>
    public enum BuildingType
    {
        Sell,
        Rent,
        Seasonal
    }

    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    public enum BuildingStatus
    {
        Void,
        Occupied,
        Closed
    }
}