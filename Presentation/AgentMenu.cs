using BusinessLogic.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation
{
    public class AgentMenu
    {
        private readonly Agent _currentAgent;
        private readonly IAgentService _agentService;
        private readonly IPropertyService _propertyService;
        private readonly IClientService _clientService;
        private readonly IDealService _dealService;
        private readonly ISavedSearchService _savedSearchService;
        private readonly MatchingService _matchingService;

        public AgentMenu(Agent currentAgent, IAgentService agentService, IPropertyService propertyService, IClientService clientService, IDealService dealService, ISavedSearchService savedSearchService, MatchingService matchingService)
        {
            _currentAgent = currentAgent;
            _agentService = agentService;
            _propertyService = propertyService;
            _clientService = clientService;
            _dealService = dealService;
            _savedSearchService = savedSearchService;
            _matchingService = matchingService;
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine($"=== Agent ({_currentAgent.FullName}) menu ===");
                Console.WriteLine("1. Add property");
                Console.WriteLine("2. Add client");
                Console.WriteLine("3. Add deal");
                Console.WriteLine("4. Get all my deals");
                Console.WriteLine("5. Get all my clients");
                Console.WriteLine("6. Change property status");
                Console.WriteLine("7. Match properties for client");
                Console.WriteLine("0. Back");

                string choice = ConsoleHelpers.GetString("Your choice:").ToUpper();
                switch (choice)
                {
                    case "1": AddProperty(); break;
                    case "2": AddClient(); break;
                    case "3": AddDeal(); break;
                    case "4": ViewMyDeals(); break;
                    case "5": ViewMyClients(); break;
                    case "6": ChangePropertyStatus(); break;
                    case "7": MatchPropertiesToClient(); break;
                    case "0": isRunning = false; break;
                    default: Console.WriteLine("Wrong choice."); break;
                }
                if (isRunning) ConsoleHelpers.PressAnyKeyToContinue();
            }
        }

        private void AddProperty()
        {
            Console.WriteLine("--- Property adding ---");
            string type = ConsoleHelpers.GetString("Type ('Flat', 'House' etc.):");
            decimal square = ConsoleHelpers.GetDecimal("Area (m2):");
            string address = ConsoleHelpers.GetString("Address:");
            decimal price = ConsoleHelpers.GetDecimal("Price ($):");
            int rooms = ConsoleHelpers.GetInt("Rooms:");
            string district = ConsoleHelpers.GetString("District:");

            var landmarks = new List<LandmarkInfo>();
            while (true)
            {
                string name = ConsoleHelpers.GetString("Landmark (or Enter for end):");
                if (string.IsNullOrWhiteSpace(name)) break;
                int time = ConsoleHelpers.GetInt($"Time to '{name}' (min):");
                landmarks.Add(new LandmarkInfo(name, time));
            }

            var newProp = new Property(type, square, address, price, rooms, district, landmarks);
            _propertyService.AddProperty(newProp);
            _propertyService.ChangePropertyStatus(newProp.Id, Property.Status.OnSale, _currentAgent.FullName);

            Console.WriteLine("Property added successfully.");
        }

        private void AddClient()
        {
            Console.WriteLine("--- Client adding ---");
            string name = ConsoleHelpers.GetString("Full name:");
            string email = ConsoleHelpers.GetString("Email:");
            string phone = ConsoleHelpers.GetString("Tel:");

            _clientService.AddClient(new Client(name, email, phone));
            Console.WriteLine("Client added successfully.");
        }

        private void ViewMyDeals()
        {
            Console.WriteLine($"--- ({_currentAgent.FullName}) deals ---");
            var myDeals = _dealService.GetDeals().Where(d => d.AgentId == _currentAgent.Id).ToList();
            if (!myDeals.Any())
            {
                Console.WriteLine("There are not any deals.");
                return;
            }
            foreach (var d in myDeals)
            {
                string clientName = d.Client?.FullName ?? "N/A";
                string propertyAddress = d.Property?.Address ?? "N/A";

                Console.WriteLine($" - [{d.Date.ToShortDateString()}] {d.Type} - {propertyAddress}");
                Console.WriteLine($"   Client: {clientName}, Base Price: ${d.BasePrice:F2}, Commission: ${d.CommissionAmount:F2}");
            }
        }

        private void ViewMyClients()
        {
            Console.WriteLine($"--- ({_currentAgent.FullName}) clients ---");
            var myClientIds = _dealService.GetDeals()
                .Where(d => d.AgentId == _currentAgent.Id)
                .Select(d => d.ClientId)
                .Distinct()
                .ToList();

            if (!myClientIds.Any())
            {
                Console.WriteLine("There are not any clients.");
                return;
            }

            var myClients = _clientService.GetClients().Where(c => myClientIds.Contains(c.Id)).ToList();

            foreach (var c in myClients)
            {
                Console.WriteLine($" - {c.FullName}, Email: {c.Email}, Tel: {c.Phone}");
            }
        }

        private void ChangePropertyStatus()
        {
            Console.WriteLine("--- Property changing status ---");
            var properties = _propertyService.GetProperties();
            for (int i = 0; i < properties.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {properties[i].Address} (Status: {properties[i].CurrentStatus})");
            }
            int choice = ConsoleHelpers.GetInt("Choose a property:") - 1;
            if (choice < 0 || choice >= properties.Count)
            {
                Console.WriteLine("Wrong choice.");
                return;
            }
            var prop = properties[choice];

            Console.WriteLine("Choose new status:");
            Console.WriteLine("1. OnSale");
            Console.WriteLine("2. Rent");
            Console.WriteLine("3. Sold");
            string statusChoice = ConsoleHelpers.GetString("Choice:");

            Property.Status newStatus;
            switch (statusChoice)
            {
                case "1": newStatus = Property.Status.OnSale; break;
                case "2": newStatus = Property.Status.Rent; break;
                case "3": newStatus = Property.Status.Sold; break;
                default: Console.WriteLine("Wrong choice."); return;
            }

            bool success = _propertyService.ChangePropertyStatus(prop.Id, newStatus, _currentAgent.FullName);
            if (success)
                Console.WriteLine($"Status for {prop.Address} successfully changed to {newStatus}.");
            else
                Console.WriteLine("Status did not change. Maybe, it was before changing");
        }

        private void AddDeal()
        {
            Console.WriteLine("--- Deal adding ---");

            Console.WriteLine("\nChoose property:");
            var availableProperties = _propertyService.GetProperties()
                .Where(p => p.CurrentStatus == Property.Status.OnSale || p.CurrentStatus == Property.Status.Rent)
                .ToList();

            if (!availableProperties.Any())
            {
                Console.WriteLine("There are not any avaliable property.");
                return;
            }

            for (int i = 0; i < availableProperties.Count; i++)
            {
                var p = availableProperties[i];
                Console.WriteLine($"{i + 1}. {p.Address} ({p.PropertyType}) - ${p.Price} [{p.CurrentStatus}]");
            }

            int propChoice = ConsoleHelpers.GetInt("Property number:") - 1;
            if (propChoice < 0 || propChoice >= availableProperties.Count)
            {
                Console.WriteLine("Wrong choice.");
                return;
            }
            Property selectedProperty = availableProperties[propChoice];

            Console.WriteLine("\nChoose client:");
            var allClients = _clientService.GetClients();
            if (!allClients.Any())
            {
                Console.WriteLine("There are not any clients.");
                return;
            }

            for (int i = 0; i < allClients.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allClients[i].FullName} ({allClients[i].Email})");
            }

            int clientChoice = ConsoleHelpers.GetInt("Client number:") - 1;
            if (clientChoice < 0 || clientChoice >= allClients.Count)
            {
                Console.WriteLine("Wrong choice.");
                return;
            }
            Client selectedClient = allClients[clientChoice];

            Console.WriteLine($"\nChose: {selectedProperty.Address} and {selectedClient.FullName}");

            decimal basePrice = selectedProperty.Price;
            decimal commissionPercent = ConsoleHelpers.GetDecimal("Comission percent:");

            decimal commissionAmount = basePrice * (commissionPercent / 100);
            decimal finalPrice = basePrice + commissionAmount;

            Console.WriteLine($"Property Price: ${basePrice:F2}");
            Console.WriteLine($"Commission ({commissionPercent}%): ${commissionAmount:F2}");
            Console.WriteLine($"TOTAL (Final Price): ${finalPrice:F2}");

            DealType dealType;
            if (selectedProperty.CurrentStatus == Property.Status.OnSale)
            {
                dealType = DealType.Purchase;
                Console.WriteLine($"Deal type: Purchase");
            }
            else
            {
                dealType = DealType.Rent;
                Console.WriteLine($"Deal type: Rent");
            }

            _dealService.AddDeal(selectedProperty, _currentAgent, selectedClient, finalPrice, dealType, commissionPercent);

            if (dealType == DealType.Purchase)
            {
                _propertyService.ChangePropertyStatus(selectedProperty.Id, Property.Status.Sold, _currentAgent.FullName);
                Console.WriteLine($"{selectedProperty.Address} property status changed to 'Sold'.");
            }

            Console.WriteLine("\nDeal successfully added.");
        }
        private void MatchPropertiesToClient()
        {
            Console.WriteLine("--- Match properties for client ---");

            var allClients = _clientService.GetClients();
            if (!allClients.Any())
            {
                Console.WriteLine("There are not any clients.");
                return;
            }

            for (int i = 0; i < allClients.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {allClients[i].FullName}");
            }

            int clientChoice = ConsoleHelpers.GetInt("Client number:") - 1;
            if (clientChoice < 0 || clientChoice >= allClients.Count)
            {
                Console.WriteLine("Wrong choice.");
                return;
            }
            Client selectedClient = allClients[clientChoice];
            Console.WriteLine($"\nDoing match for: {selectedClient.FullName}");

            var matchedProperties = _matchingService.GetMatchingPropertiesForClient(selectedClient.Id);

            if (!matchedProperties.Any())
            {
                Console.WriteLine("There are not any properties for client.");
                return;
            }

            Console.WriteLine($"\n--- Found {matchedProperties.Count} properties ---");
            foreach (var p in matchedProperties)
            {
                var landmarksString = string.Join(", ", (p.Landmarks ?? Enumerable.Empty<LandmarkInfo>()).Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" - {p.PropertyType} on {p.Address} (${p.Price})");
                Console.WriteLine($"   Landmarks: [{landmarksString}]");
            }
        }
    }
}