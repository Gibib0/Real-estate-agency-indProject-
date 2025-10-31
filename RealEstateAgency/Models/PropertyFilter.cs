using System;

namespace RealEstateAgency.Models
{
    public class PropertyFilter
    {
        public string PropertyType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinArea { get; set; }
        public decimal? MaxArea { get; set; }
        public int? MinRooms { get; set; }
        public int? MaxRooms { get; set; }
    }
}
