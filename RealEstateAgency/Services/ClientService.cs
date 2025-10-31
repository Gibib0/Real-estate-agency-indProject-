using RealEstateAgency.Models;
using System.Collections.Generic;

namespace RealEstateAgency.Services
{
    public class ClientService
    {
        private List<Client> _clients;
        private JSONDataService _jsonDataService;
        private string _filePath = @"Data/clientsData.json";

        public ClientService()
        {
            _jsonDataService = new JSONDataService();
            _clients = _jsonDataService.LoadFromFile<Client>(_filePath);
        }
        public void AddClient(Client client)
        {
            if (!_clients.Any(c => c.FullName.Equals(client.FullName, StringComparison.OrdinalIgnoreCase)))
            {
                _clients.Add(client);
                _jsonDataService.SaveToFile(_clients, _filePath);
            }
        }

        public List<Client> GetClients()
        {
            return _clients;
        }
        public bool UpdateClient(Client updatedClient)
        {
            int index = _clients.FindIndex(p => p.Id == updatedClient.Id);

            if (index != -1)
            {
                _clients[index] = updatedClient;
                _jsonDataService.SaveToFile(_clients, _filePath);
                return true;
            }

            return false;
        }
        public bool DeleteClient(Guid id)
        {
            var clientToRemove = _clients.FirstOrDefault(p => p.Id == id);

            if (clientToRemove != null)
            {
                _clients.Remove(clientToRemove);
                _jsonDataService.SaveToFile(_clients, _filePath);
                return true;
            }

            return false;
        }
    }
}
