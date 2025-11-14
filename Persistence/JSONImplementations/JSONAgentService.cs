using BusinessLogic.Models;
using BusinessLogic.Services;
using Persistence;

public class JSONAgentService : IAgentService
{
    private List<Agent> _agents = new List<Agent>();
    private JSONDataService _jsonDataService;
    private string _filePath = @"Data/agentsData.json";

    public JSONAgentService()
    {
        _jsonDataService = new JSONDataService();
        _agents = _jsonDataService.LoadFromFile<Agent>(_filePath);
    }

    public void AddAgent(Agent agent)
    {
        if (!_agents.Any(a => a.Id == agent.Id ||
            a.FullName.Equals(agent.FullName, StringComparison.OrdinalIgnoreCase)))
        {
            _agents.Add(agent);
            _jsonDataService.SaveToFile(_agents, _filePath);
        }
    }
    public List<Agent> GetAgents()
    {
        return _agents;
    }
}