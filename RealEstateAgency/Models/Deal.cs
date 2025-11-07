using System.Reflection.Metadata;

namespace RealEstateAgency.Models
{
    public class Deal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public required Property Property { get; set; }
        public required Agent Agent { get; set; }
        public required Client Client { get; set; }
        public decimal FinalPrice { get; set; }
        public DealType Type { get; set; }
        public decimal CommissionPercent { get; set; }
        public decimal BasePrice
        {
            get
            {
                decimal multiplier = 1 + (CommissionPercent / 100);
                if (multiplier == 0) return 0;
                return Math.Round(FinalPrice / multiplier, 2);
            }
        }
        public decimal CommissionAmount
        {
            get { return FinalPrice - BasePrice; }
        }
    }
}
