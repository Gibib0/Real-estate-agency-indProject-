using RealEstateAgency.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace RealEstateAgency.Services
{
    public class DealService
    {
        private List<Deal> _deals;
        private JSONDataService _jsonDataService;
        private string _filePath = @"Data/dealsData.json";

        public DealService()
        {
            _jsonDataService = new JSONDataService();
            _deals = _jsonDataService.LoadFromFile<Deal>(_filePath);
        }

        public void AddDeal(Property property, Agent agent, Client client, decimal finalPrice, DealType dealtype, decimal commissionPercent)
        {

            bool exists = _deals.Any(d =>
            d.Agent.Id == agent.Id &&
            d.Client.Id == client.Id &&
            d.Type == dealtype &&
            d.Date.Date == DateTime.Now.Date);

            if (exists)
            {
                return;
            }

            var deal = new Deal()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                Property = property,
                Agent = agent,
                Client = client,
                FinalPrice = finalPrice,
                Type = dealtype,
                CommissionPercent = commissionPercent
            };

            _deals.Add(deal);
            _jsonDataService.SaveToFile(_deals, _filePath);
        }

        public List<Deal> GetDeals()
        {
            return _deals;
        }

        public decimal GetTotalComissionIncome()
        {
            return _deals.Sum(d => d.CommissionAmount);
        }

        public decimal GetAgentComissionIncome(Guid agentId)
        {
            return _deals
                .Where(d => d.Agent.Id == agentId)
                .Sum(d => d.CommissionAmount);
        }
    }
}
