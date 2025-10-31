using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency.Models
{
    public class Property
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PropertyType { get; set; }
        public decimal Square { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public int Rooms { get; set; }
        public enum Status
        {
            None,
            OnSale,
            Rent,
            Sold
        }
        public Status CurrentStatus { get; set; }
        public Property() { }

        public List<StatusChange> StatusHistory { get; set; } = new List<StatusChange>();

        public Property(string propertytype, decimal square, string address, decimal price, int rooms)
        {
            Id = Guid.NewGuid();
            PropertyType = propertytype;
            Square = square;
            Address = address;
            Price = price;
            Rooms = rooms;
            CurrentStatus = Status.OnSale;
        }
    }
}
