using BusinessLogic.Models;
using BusinessLogic.Services;
using Integration;
using Integration.SqliteImplementations;
using Persistence.JSONImplementations;
using Integration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Choose storage type:");
            Console.WriteLine("1. JSON Files (Persistence Layer)");
            Console.WriteLine("2. SQLite Database (Integration Layer)");
            string choice = ConsoleHelpers.GetString("Your choice:");

            IPropertyService _propertyService;
            IAgentService _agentService;
            IClientService _clientService;
            IDealService _dealService;
            ISavedSearchService _savedSearchService;

            var _authService = new AuthService();
            MatchingService _matchingService;

            if (choice == "2")
            {
                Console.WriteLine("Using SQLite (Integration Layer)...");
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);
                IConfiguration config = builder.Build();

                var options = new DbContextOptionsBuilder<RealEstateDbContext>()
                    .UseSqlite(config.GetConnectionString("RealEstateDb"))
                    .Options;

                using (var dbContext = new RealEstateDbContext(options))
                {
                    dbContext.Database.Migrate();

                    _propertyService = new SqlitePropertyService(dbContext);
                    _agentService = new SqliteAgentService(dbContext);
                    _clientService = new SqliteClientService(dbContext);
                    _dealService = new SqliteDealService(dbContext);
                    _savedSearchService = new SqliteSavedSearchService(dbContext);
                    _matchingService = new MatchingService(_propertyService, _savedSearchService);

                    await RunApplicationLogic(_authService, _agentService, _propertyService, _clientService, _dealService, _savedSearchService, _matchingService);
                }
            }
            else
            {
                Console.WriteLine("Using JSON Files (Persistence Layer)...");

                _propertyService = new JSONPropertyService();
                _agentService = new JSONAgentService();
                _clientService = new JSONClientService();
                _dealService = new JSONDealService();
                _savedSearchService = new JSONSavedSearchService();
                _matchingService = new MatchingService(_propertyService, _savedSearchService);

                await RunApplicationLogic(_authService, _agentService, _propertyService, _clientService, _dealService, _savedSearchService, _matchingService);
            }
        }

        public static async Task RunApplicationLogic(
            AuthService _authService,
            IAgentService _agentService,
            IPropertyService _propertyService,
            IClientService _clientService,
            IDealService _dealService,
            ISavedSearchService _savedSearchService,
            MatchingService _matchingService)
        {
            LoginResponse userLogin = null;
            string username = null;

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