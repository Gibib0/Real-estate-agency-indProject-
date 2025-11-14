using BusinessLogic.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integration.SqliteImplementations
{
    public class SqliteClientService : IClientService
    {
        private readonly RealEstateDbContext _context;

        public SqliteClientService(RealEstateDbContext context)
        {
            _context = context;
        }

        public void AddClient(Client client)
        {
            string upperFullName = client.FullName.ToUpper();
            string upperEmail = client.Email.ToUpper();
            if (!_context.Clients.Any(c => c.FullName.ToUpper() == upperFullName || c.Email.ToUpper() == upperEmail))
            {
                _context.Clients.Add(client);
                _context.SaveChanges();
            }
        }

        public List<Client> GetClients()
        {
            return _context.Clients.ToList();
        }

        public List<Client> GetClientByType(Client.ClientType type)
        {
            return _context.Clients.Where(c => c.CurrentType == type).ToList();
        }

        public bool UpdateClient(Client updatedClient)
        {
            _context.Clients.Update(updatedClient);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteClient(Guid id)
        {
            var client = _context.Clients.Find(id);
            if (client == null) return false;

            _context.Clients.Remove(client);
            _context.SaveChanges();
            return true;
        }
    }
}