using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjetoApp.Data
{
    /// <summary>
    /// Represents a customer order.
    /// </summary>
    public class Negotiation : DbObject
    {
        /// <summary>
        /// Creates a new order.
        /// </summary>
        public Negotiation()
        { }

        /// <summary>
        /// Creates a new order for the given customer.
        /// </summary>
        public Negotiation(Customer customer) : this()
        {
            Customer = customer;
            CustomerName = $"{customer.FirstName} {customer.LastName}";
            Address = customer.Address;
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
        /// Gets or sets the invoice number.
        /// </summary>
        public int InvoiceNumber { get; set; } = 0;

        /// <summary>
        /// Gets or sets the order shipping address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the items in the order.
        /// </summary>
        // public virtual List<LineItem> LineItems { get; set; } = new List<LineItem>();

        /// <summary>
        /// Gets or sets when the order was placed.
        /// </summary>
        public DateTime DatePlaced { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets when the order was filled.
        /// </summary>
        public DateTime? DateFilled { get; set; } = null;

        /// <summary>
        /// Gets or sets the order's status.
        /// </summary>
        public NegotiationStatus Status { get; set; } = NegotiationStatus.Open;

        /// <summary>
        /// Gets or sets the order's payment status.
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        /// <summary>
        /// Gets or sets the order's term.
        /// </summary>
        public Term Term { get; set; }

        
        /// <summary>
        /// Returns the invoice number.
        /// </summary>
        public override string ToString() => $"{InvoiceNumber}";
    }

    /// <summary>
    /// Represents the term for an order.
    /// </summary>
    public enum Term
    {
        Net1, 
        Net5,
        Net15, 
        Net30
    }

    /// <summary>
    /// Represents the payment status for an order.
    /// </summary>
    public enum PaymentStatus
    {
        Unpaid,
        Paid 
    }

    /// <summary>
    /// Represents the status of an order.
    /// </summary>
    public enum NegotiationStatus
    {
        Open,
        Filled, 
        Cancelled
    }
}
