using System.Reflection.Metadata;

namespace RealEstateAgency.Models
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        //public Property Property { get; set; }
        public Agent Agent { get; set; }
        //public Client Client { get; set; }
        public decimal FinalPrice { get; set; }
        public DealType Type { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal CommissionAmount
        {
            get { return FinalPrice * CommissionPercent / 100; }
        }
    }
}
