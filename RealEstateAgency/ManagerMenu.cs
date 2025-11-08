using RealEstateAgency.Models;
using RealEstateAgency.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static RealEstateAgency.Models.Client;

namespace RealEstateAgency
{
    public class ManagerMenu
    {
        private readonly AgentService _agentService;
        private readonly PropertyService _propertyService;
        private readonly ClientService _clientService;
        private readonly DealService _dealService;

        public ManagerMenu(AgentService agentService, PropertyService propertyService, ClientService clientService, DealService dealService)
        {
            _agentService = agentService;
            _propertyService = propertyService;
            _clientService = clientService;
            _dealService = dealService;
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== MANAGER MENU ===");
                Console.WriteLine("1. Add new agent");
                Console.WriteLine("2. Get all agents");
                Console.WriteLine("3. Get all properties");
                Console.WriteLine("4. Get all clients");
                Console.WriteLine("5. Get all deals");
                Console.WriteLine("6. Get stats");
                Console.WriteLine("7. Get clients by role");
                Console.WriteLine("8. Get agency income");
                Console.WriteLine("0. Back");

                string choice = ConsoleHelpers.GetString("Your choice:").ToUpper();
                switch (choice)
                {
                    case "1":
                        AddAgent();
                        break;
                    case "2":
                        ViewAllAgents();
                        break;
                    case "3":
                        ViewAllProperties();
                        break;
                    case "4":
                        ViewAllClients();
                        break;
                    case "5":
                        ViewAllDeals();
                        break;
                    case "6":
                        ViewStatistics();
                        break;
                    case "7":
                        ViewClientsByRole();
                        break;
                    case "8":
                        ViewAgencyIncome();
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Wrong choice.");
                        break;
                }
                if (isRunning) ConsoleHelpers.PressAnyKeyToContinue();
            }
        }

        private void AddAgent()
        {
            Console.WriteLine("--- Agent adding ---");
            string name = ConsoleHelpers.GetString("Full name:");
            int exp = ConsoleHelpers.GetInt("Expirience:");
            _agentService.AddAgent(new Agent(name, exp));
            Console.WriteLine("Agent successfully added.");
        }

        private void ViewAllAgents()
        {
            Console.WriteLine("--- All agents list ---");
            var agents = _agentService.GetAgents();
            if (!agents.Any())
            {
                Console.WriteLine("There are not any agents.");
                return;
            }
            foreach (var a in agents)
            {
                Console.WriteLine($" - {a.FullName}, Expirience: {a.Experience} years");
            }
        }

        private void ViewAllProperties()
        {
            Console.WriteLine("--- All properties list ---");
            var properties = _propertyService.GetProperties();
            if (!properties.Any())
            {
                Console.WriteLine("There are not any properties.");
                return;
            }
            foreach (var p in properties)
            {
                var landmarksString = string.Join(", ", (p.Landmarks ?? Enumerable.Empty<LandmarkInfo>()).Select(lm => $"{lm.Name} ({lm.TravelTimeMinutes} min)"));
                Console.WriteLine($" - {p.PropertyType} on {p.Address} ({p.Square} m2) for ${p.Price}. Status: {p.CurrentStatus}");
                Console.WriteLine($"   Landmarks: [{landmarksString}]");
            }
        }

        private void ViewAllClients()
        {
            Console.WriteLine("--- All clients list ---");
            var clients = _clientService.GetClients();
            if (!clients.Any())
            {
                Console.WriteLine("There are not any clients.");
                return;
            }
            foreach (var c in clients)
            {
                Console.WriteLine($" - {c.FullName}, Email: {c.Email}, Tel: {c.Phone}, Type: {c.CurrentType}");
            }
        }

        private void ViewAllDeals()
        {
            Console.WriteLine("--- All deals list ---");
            var deals = _dealService.GetDeals();
            if (!deals.Any())
            {
                Console.WriteLine("There are not any deals.");
                return;
            }
            foreach (var d in deals)
            {
                Console.WriteLine($" - [{d.Date.ToShortDateString()}] {d.Type} - {d.Property.Address}");
                Console.WriteLine($"   Agent: {d.Agent.FullName}, Client: {d.Client.FullName}, Price: ${d.FinalPrice}");
                Console.WriteLine($"   Base Price: ${d.BasePrice:F2}");
                Console.WriteLine($"   Commission: ${d.CommissionAmount} ({d.CommissionPercent}%)");
                Console.WriteLine($"   Total Final Price: ${d.FinalPrice:F2}");
            }
        }

        private void ViewStatistics()
        {
            Console.WriteLine("--- Main stats ---");
            var allDeals = _dealService.GetDeals();
            if (!allDeals.Any())
            {
                Console.WriteLine("There are not any deals for stats.");
                return;
            }

            Console.WriteLine($"\n--- Agents efficiency ---");
            List<AgentEfficiencyStats> agentStats = DealStatsService.GetAgentEfficiency(allDeals);
            foreach (var a in agentStats)
            {
                Console.WriteLine($" - {a.AgentName}: {a.DealCount} deals, Total commission: ${a.TotalCommission}");
            }

            Console.WriteLine($"\n--- The most active clients ---");
            List<ClientActivityStats> clientStats = DealStatsService.GetMostActiveClients(allDeals);
            foreach (var c in clientStats)
            {
                Console.WriteLine($" - {c.ClientName}: {c.DealCount} deals");
            }

            Console.WriteLine($"\n--- General numbers ---");
            decimal totalSales = DealStatsService.GetTotalSalesAmount(allDeals);
            Console.WriteLine($"Total sales amount: ${totalSales}");

            decimal avgPricePerSqm = DealStatsService.GetAveragePricePerSquareMeter(allDeals);
            Console.WriteLine($"Average price per m2: ${avgPricePerSqm:F2}");

            int dealsThisMonth = DealStatsService.GetDealCountForPeriod(allDeals, DateTime.Now.AddMonths(-1), DateTime.Now);
            Console.WriteLine($"Deals for last month: {dealsThisMonth}");
        }

        private void ViewClientsByRole()
        {
            Console.WriteLine("---Clients by role ---");
            Console.WriteLine("Select role: ");
            Console.WriteLine("1. Buyer");
            Console.WriteLine("2. Seller");
            Console.WriteLine("3. Owner");
            string choice = Console.ReadLine();

            Client.ClientType? selectedRole = null;

            switch (choice)
            {
                case "1":
                    selectedRole = Client.ClientType.Buyer;
                    break;
                case "2":
                    selectedRole = Client.ClientType.Seller;
                    break;
                case "3":
                    selectedRole = Client.ClientType.Owner;
                    break;
                default:
                    selectedRole = null;
                    break;
            }

            if (selectedRole == null)
            {
                Console.WriteLine("Wrong choice.");
                return;
            }

            var clients = _clientService.GetClientByType(selectedRole.Value);
            if (!clients.Any())
            {
                Console.WriteLine($"No clients with role {selectedRole}.");
            }

            Console.WriteLine($"---Clients with role {selectedRole} ---");
            foreach (var c in clients)
            {
                Console.WriteLine($" - {c.FullName}, Email: {c.Email}, Tel: {c.Phone}");
            }
        }
        private void ViewAgencyIncome()
        {
            Console.WriteLine("--- Agency total income ---");
            decimal totalIncome = _dealService.GetTotalComissionIncome();
            Console.WriteLine($"Total commission earned by agency: ${totalIncome:F2}");
        }
    }
}