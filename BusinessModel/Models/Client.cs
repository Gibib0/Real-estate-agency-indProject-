using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLogic.Models.Property;

namespace BusinessLogic.Models
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public enum ClientType
        {
            Buyer,
            Seller,
            Owner
        }
        public ClientType CurrentType { get; set; }
        public Client() { }
        public Client(string fullName, string email, string phone)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            Email = email;
            Phone = phone;
            CurrentType = ClientType.Buyer;
        }
    }
}
