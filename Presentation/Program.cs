using BusinessLogic.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation
{
    class Program
    {
        private static readonly AgentService _agentService = new AgentService();
        private static readonly DealService _dealService = new DealService();
        private static readonly PropertyService _propertyService = new PropertyService();
        private static readonly ClientService _clientService = new ClientService();
        private static readonly SavedSearchService _savedSearchService = new SavedSearchService();
        private static readonly MatchingService _matchingService = new MatchingService(_propertyService, _savedSearchService);

        static void Main(string[] args)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== WELCOME TO OUR REAL ESTATE AGENCY! ===");
                Console.WriteLine("Who are you?");
                Console.WriteLine("1. Manager");
                Console.WriteLine("2. Agent");
                Console.WriteLine("3. Client");
                Console.WriteLine("0. Exit");
                Console.Write("Choose the option: ");
                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "1":
                        var managerMenu = new ManagerMenu(_agentService, _propertyService, _clientService, _dealService);
                        managerMenu.Run();
                        break;
                    case "2":
                        Agent currentAgent = SelectAgent();
                        if (currentAgent != null)
                        {
                            var agentMenu = new AgentMenu(currentAgent, _agentService, _propertyService, _clientService, _dealService, _savedSearchService, _matchingService);
                            agentMenu.Run();
                        }
                        break;
                    case "3":
                        Client currentClient = SelectClient();
                        if (currentClient != null)
                        {
                            var clientMenu = new ClientMenu(currentClient, _propertyService, _dealService, _savedSearchService);
                            clientMenu.Run();
                        }
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Wrong choice. Try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static Agent SelectAgent()
        {
            var agents = _agentService.GetAgents();
            if (!agents.Any())
            {
                Console.WriteLine("There are not any agents. Tell to your manager.");
                ConsoleHelpers.PressAnyKeyToContinue();
                return null;
            }

            Console.WriteLine("Choose your user:");
            for (int i = 0; i < agents.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {agents[i].FullName}");
            }

            int choice = ConsoleHelpers.GetInt("Agent number:") - 1;
            if (choice >= 0 && choice < agents.Count)
            {
                return agents[choice];
            }
            Console.WriteLine("Wrong choice.");
            ConsoleHelpers.PressAnyKeyToContinue();
            return null;
        }

        private static Client SelectClient()
        {
            var clients = _clientService.GetClients();
            if (!clients.Any())
            {
                Console.WriteLine("There are not any clients. Tell to agent.");
                ConsoleHelpers.PressAnyKeyToContinue();
                return null;
            }

            Console.WriteLine("Choose your user:");
            for (int i = 0; i < clients.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {clients[i].FullName} ({clients[i].Email})");
            }

            int choice = ConsoleHelpers.GetInt("Client number:") - 1;
            if (choice >= 0 && choice < clients.Count)
            {
                return clients[choice];
            }
            Console.WriteLine("Wrong choice.");
            ConsoleHelpers.PressAnyKeyToContinue();
            return null;
        }
    }
}