using BusinessLogic.Models;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface IAgentService
    {
        void AddAgent(Agent agent);
        List<Agent> GetAgents();
    }
}