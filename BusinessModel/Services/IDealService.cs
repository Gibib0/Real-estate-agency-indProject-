using BusinessLogic.Models;
using System;
using System.Collections.Generic;

namespace BusinessLogic.Services
{
    public interface IDealService
    {
        void AddDeal(Property property, Agent agent, Client client, decimal finalPrice, DealType dealtype, decimal commissionPercent);
        List<Deal> GetDeals();
        decimal GetTotalComissionIncome();
        decimal GetAgentComissionIncome(Guid agentId);
    }
}