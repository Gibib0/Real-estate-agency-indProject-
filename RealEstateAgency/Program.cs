// просто проверка

using RealEstateAgency.Models;
using RealEstateAgency.Services;
using System;

namespace RealEstateAgency
{
    class Program
    {
        static void Main(string[] args)
        {
            var agentService = new AgentService();
            var dealService = new DealService();
            var propertyService = new PropertyService();
            var clientService = new ClientService();

            propertyService.AddProperty(new Property("House", 344, "Privet Drive 3/62", 350000));
            propertyService.AddProperty(new Property("Flat", 40, "Broker Street 25/12", 20000));
            var allProperty = propertyService.GetProperties();
            Console.WriteLine($"Properties: {allProperty.Count}");
            foreach (var a in allProperty)
            {
                Console.WriteLine($" Type: {a.PropertyType}, Square: {a.Square}, Address: {a.Address}, Price: {a.Price}");
            }
            var NewProperty = allProperty[0];
            NewProperty.Address = "Beverly Hills 90210";
            propertyService.UpdateProperty(NewProperty);
            Console.WriteLine("\n");
            foreach (var a in allProperty)
            {
                Console.WriteLine($" Type: {a.PropertyType}, Square: {a.Square}, Address: {a.Address}, Price: {a.Price}");
            }
            propertyService.DeleteProperty(allProperty[1].Id);
            Console.WriteLine($"Properties: {allProperty.Count}");
            foreach (var a in allProperty)
            {
                Console.WriteLine($" Type: {a.PropertyType}, Square: {a.Square}, Address: {a.Address}, Price: {a.Price}");
            }

            clientService.AddClient(new Client("Vasya Pupkin", "vasyapupkin@example.com", "+38(096)1234567"));
            clientService.AddClient(new Client("John Smith", "johnsmith@example.com", "+38(093)4815162"));
            var allClients = clientService.GetClients();
            Console.WriteLine($"Clients: {allClients.Count}");
            foreach (var a in allClients)
            {
                Console.WriteLine($" Name: {a.FullName}, E-Mail: {a.Email}, Phone: {a.Phone}");
            }
            var NewClient = allClients[0];
            NewClient.Phone = "+38(050)2233344";
            clientService.UpdateClient(NewClient);
            Console.WriteLine("\n");
            foreach (var a in allClients)
            {
                Console.WriteLine($" Name: {a.FullName}, E-Mail: {a.Email}, Phone: {a.Phone}");
            }
            clientService.DeleteClient(allClients[1].Id);
            Console.WriteLine($"Clients: {allClients.Count}");
            foreach (var a in allClients)
            {
                Console.WriteLine($" Name: {a.FullName}, E-Mail: {a.Email}, Phone: {a.Phone}");
            }

            agentService.AddAgent(new Agent("Ivanov Ivan", 3));
            agentService.AddAgent(new Agent("Romanenko Roman", 5));

            Console.WriteLine("Agents added");

            var allAgents = agentService.GetAgents();
            Console.WriteLine($"Agents: {allAgents.Count}");

            foreach (var a in allAgents)
            {
                Console.WriteLine($" - {a.FullName}, Exp: {a.Experience} years");
            }


            dealService.AddDeal(allProperty[0], allAgents[0], allClients[0], 350000, 0, 3);
            var allDeals = dealService.GetDeals();
            Console.WriteLine($"Deals: {allDeals.Count}");
            foreach (var a in allDeals)
            {
                Console.WriteLine($" Property: {a.Property.PropertyType}, Agent: {a.Agent.FullName}, Client: {a.Client.FullName}, Price: {a.FinalPrice}, Type: {a.Type}, Percent: {a.CommissionPercent}");
            }
            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
    }
}