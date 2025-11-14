using BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface IClientService
    {
        void AddClient(Client client);
        List<Client> GetClients();
        List<Client> GetClientByType(Client.ClientType type);
        bool UpdateClient(Client updatedClient);
        bool DeleteClient(Guid id);
    }
}