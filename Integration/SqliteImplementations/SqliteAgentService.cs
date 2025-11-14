using BusinessLogic.Models;
using BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integration.SqliteImplementations
{
    public class SqliteAgentService : IAgentService
    {
        private readonly RealEstateDbContext _context;

        public SqliteAgentService(RealEstateDbContext context)
        {
            _context = context;
        }

        public void AddAgent(Agent agent)
        {
            string upperFullName = agent.FullName.ToUpper();
            if (!_context.Agents.Any(a => a.FullName.ToUpper() == upperFullName))
            {
                _context.Agents.Add(agent);
                _context.SaveChanges();
            }
        }

        public List<Agent> GetAgents()
        {
            return _context.Agents.ToList();
        }
    }
}