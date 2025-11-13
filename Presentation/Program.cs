using BusinessLogic.Models;
using BusinessLogic.Services;
using Integration;
using Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        private static readonly AuthService _authService = new AuthService();

        static async Task Main(string[] args)
        {
            LoginResponse userLogin = null;
            string username = null;
            while (userLogin == null)
            {
                while (userLogin == null)
                {
                    Console.Clear();
                    Console.WriteLine("=== WELCOME TO OUR REAL ESTATE AGENCY! ===");
                    username = ConsoleHelpers.GetString("Login:");
                    string password = ConsoleHelpers.GetString("Password:");
                    userLogin = await _authService.Login(username, password);
                    if (userLogin == null)
                    {
                        Console.WriteLine("Wrong login or password. Try again.");
                        ConsoleHelpers.PressAnyKeyToContinue();
                    }
                }
                Console.Clear();
                Console.WriteLine($"Success! Your role: {userLogin.Role}");
                ConsoleHelpers.PressAnyKeyToContinue();
                switch (userLogin.Role.ToLower())
                {
                    case "admin":
                        var managerMenu = new ManagerMenu(_agentService, _propertyService, _clientService, _dealService);
                        managerMenu.Run();
                        break;
                    case "agent":
                        Agent currentAgent = _agentService.GetAgents().FirstOrDefault(a => a.FullName.Equals(username, StringComparison.OrdinalIgnoreCase));
                        if (currentAgent == null)
                        {
                            currentAgent = new Agent(username, 0);
                            _agentService.AddAgent(currentAgent);
                            Console.WriteLine($"Agent {username} did not find, new agent created.");
                        }

                        var agentMenu = new AgentMenu(currentAgent, _agentService, _propertyService, _clientService, _dealService, _savedSearchService, _matchingService);
                        agentMenu.Run();
                        break;
                    case "client":
                        Client currentClient = _clientService.GetClients().FirstOrDefault(c => c.FullName.Equals(username, StringComparison.OrdinalIgnoreCase));
                        if (currentClient == null)
                        {
                            currentClient = new Client(username, "api-user@email.com", "000");
                            _clientService.AddClient(currentClient);
                            Console.WriteLine($"Client {username} did not find, new client created.");
                        }
                        var clientMenu = new ClientMenu(currentClient, _propertyService, _dealService, _savedSearchService, _matchingService);
                        clientMenu.Run();
                        break;
                    default:
                        Console.WriteLine($"Role '{userLogin.Role}' did not find.");
                        break;
                }
            }
        }
    }
}