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

            propertyService.AddProperty(new Property("House", 344, "Privet Drive 3/62", 350000, 6, "West", new List<LandmarkInfo>
            {
                new LandmarkInfo("Park", 5),
                new LandmarkInfo("Subway", 10),
                new LandmarkInfo("Supermarket", 3)
            }));
            propertyService.AddProperty(new Property("Flat", 40, "Broker Street 25/12", 20000, 1, "East", new List<LandmarkInfo>
            {
                new LandmarkInfo("School", 5),
                new LandmarkInfo("Subway", 10),
                new LandmarkInfo("Supermarket", 3)
            }));

            var allProperty = propertyService.GetProperties();
            Console.WriteLine($"Properties: {allProperty.Count}");
            foreach (var a in allProperty)
            {
                var landmarksString = string.Join(", ", a.Landmarks.Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" Type: {a.PropertyType}, Square: {a.Square}, Address: {a.Address}, Rooms: {a.Rooms}, Price: {a.Price}, District: {a.District}, Landmarks: [{landmarksString}]");
            }
        
            var NewProperty = allProperty[0];
            NewProperty.Address = "Beverly Hills 90210";
            propertyService.UpdateProperty(NewProperty);
            Console.WriteLine("\n");
            foreach (var a in allProperty)
            {
                var landmarksString = string.Join(", ", a.Landmarks.Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" Type: {a.PropertyType}, Square: {a.Square}, Address: {a.Address}, Rooms: {a.Rooms}, Price: {a.Price}, District: {a.District}, Landmarks: [{landmarksString}]");
            }
            propertyService.DeleteProperty(allProperty[1].Id);
            Console.WriteLine($"Properties: {allProperty.Count}");
            foreach (var a in allProperty)
            {
                var landmarksString = string.Join(", ", a.Landmarks.Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" Type: {a.PropertyType}, Square: {a.Square}, Address: {a.Address}, Rooms: {a.Rooms}, Price: {a.Price}, District: {a.District}, Landmarks: [{landmarksString}]");
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
                Console.WriteLine($" Amount: {a.CommissionAmount}, Final Price: {a.FinalPrice + a.CommissionAmount}");
            }



            Console.WriteLine("\nFilters");
            var filter = new PropertyFilter
            {
                PropertyType = "House",
                MinPrice = 350000,
                MaxPrice = 350000,
                MinArea = 344,
                MaxArea = 344,
                MinRooms = 1,
                MaxRooms = 7,
            };

            var filtered = propertyService.GetPropertiesByFilters(filter);
            Console.WriteLine($"Filtered properties: {filtered.Count()}");
            foreach (var p in filtered)
            {
                var landmarksString = string.Join(", ", p.Landmarks.Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" - {p.PropertyType}, {p.Square}m^2, {p.Address}, {p.Rooms} rooms, {p.Price}$, {p.District} district, landmarks: [{landmarksString}]");
            }

            var savedSearchService = new SavedSearchService();
            if (allClients != null && allClients.Count > 0)
            {
                var clientForSave = allClients[0];
                string description = $"Saved filter: {filter.PropertyType}, price {filter.MinPrice}-{filter.MaxPrice}, area {filter.MinArea}-{filter.MaxArea}, rooms {filter.MinRooms}-{filter.MaxRooms}";

                savedSearchService.AddSavedSearch(clientForSave.Id, filter, description);
                Console.WriteLine($"\nFilter saved for client: {clientForSave.FullName}");
            }
            else
            {
                Console.WriteLine("\nNo clients available - ckipping saved search creation.");
            }

            if (allClients != null && allClients.Count > 0)
            {
                var clientForLoad = allClients[0];
                var previousSearches = savedSearchService.GetSavedSearches(clientForLoad.Id);

                Console.WriteLine($"\nSaved searches for {clientForLoad.FullName}: {previousSearches}");
                foreach (var s in previousSearches)
                {
                    var f = s.Filter;
                    Console.WriteLine($" - [{s.DateSaved}] {s.Description} => Type:{f.PropertyType}, Price:{f.MinPrice}-{f.MaxPrice}, Area:{f.MinArea}-{f.MaxArea}, Rooms:{f.MinRooms}-{f.MaxRooms}");
                }
            }


                Console.WriteLine("\nStatuses");
            var prop = propertyService.GetProperties().First();
            Console.WriteLine($"Initial status of {prop.Address}: {prop.CurrentStatus}");

            propertyService.ChangePropertyStatus(prop.Id, Property.Status.Rent, "Romanenko Roman");
            propertyService.ChangePropertyStatus(prop.Id, Property.Status.Sold, "Romanenko Roman");

            Console.WriteLine($"New status of {prop.Address}: {prop.CurrentStatus}");

            var history = propertyService.GetStatusHistory(prop.Id);
            Console.WriteLine($"Status history for {prop.Address}:");
            foreach (var record in history)
            {
                Console.WriteLine($" - {record.Time}: {record.OldStatus} -> {record.NewStatus}, by {record.ChangeBy}");
            }
            
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            int dealsThisMonth = DealStatsService.GetDealCountForPeriod(allDeals, startOfMonth, endOfMonth);
            Console.WriteLine($"In this month was {dealsThisMonth} deals");

            decimal totalSales = DealStatsService.GetTotalSalesAmount(allDeals);
            Console.WriteLine($"Total sales: {totalSales}");

            decimal avgPricePerSqm = DealStatsService.GetAveragePricePerSquareMeter(allDeals);
            Console.WriteLine($"Average price per square meter: {avgPricePerSqm}");

            List<AgentEfficiencyStats> agentStats = DealStatsService.GetAgentEfficiency(allDeals);
            foreach (var a in agentStats)
            {
                Console.WriteLine($" - {a.AgentName}, {a.DealCount}, {a.TotalCommission}");
            }
            List<ClientActivityStats> clientStats = DealStatsService.GetMostActiveClients(allDeals);
            foreach (var a in clientStats)
            {
                Console.WriteLine($" - {a.ClientName}, {a.DealCount}");
            }

            Console.WriteLine("\nPress any key");
            Console.ReadLine();
        }
    }
}