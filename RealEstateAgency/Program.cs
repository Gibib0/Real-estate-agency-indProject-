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

            agentService.AddAgent(new Agent("Ivanov Ivan", 3));
            agentService.AddAgent(new Agent("Romanenko Roman", 5));

            Console.WriteLine("Agents added");

            var allAgents = agentService.GetAgents();
            Console.WriteLine($"Agents: {allAgents.Count}");

            foreach (var a in allAgents)
            {
                Console.WriteLine($" - {a.FullName}, Exp: {a.Experience} years");
            }

            Console.WriteLine("Press any key");
            Console.ReadLine();
        }
    }
}