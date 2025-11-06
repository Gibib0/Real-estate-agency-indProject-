using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RealEstateAgency.Models;

namespace RealEstateAgency.Models
{
    public class SaveSearch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; }
        public DateTime DateSaved { get; set; } = DateTime.Now;
        public string Description { get; set; }
        public PropertyFilter Filter { get; set; }

        public SaveSearch() { }

        public SaveSearch(Guid clientId, PropertyFilter filter)
        {
            ClientId = clientId;
            Filter = filter;
            DateSaved = DateTime.Now;
        }
    }
}
