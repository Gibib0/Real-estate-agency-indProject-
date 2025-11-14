using BusinessLogic.Models;
using BusinessLogic.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integration.SqliteImplementations
{
    public class SqliteDealService : IDealService
    {
        private readonly RealEstateDbContext _context;

        public SqliteDealService(RealEstateDbContext context)
        {
            _context = context;
        }

        public void AddDeal(Property property, Agent agent, Client client, decimal finalPrice, DealType dealtype, decimal commissionPercent)
        {
            var deal = new Deal()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                PropertyId = property.Id,
                AgentId = agent.Id,
                ClientId = client.Id,
                FinalPrice = finalPrice,
                Type = dealtype,
                CommissionPercent = commissionPercent,
                Property = null,
                Agent = null,
                Client = null
            };

            _context.Deals.Add(deal);
            _context.SaveChanges();
        }

        public List<Deal> GetDeals()
        {
            return _context.Deals
                .Include(d => d.Property)
                .Include(d => d.Agent)
                .Include(d => d.Client)
                .ToList();
        }

        public decimal GetTotalComissionIncome()
        {
            return _context.Deals.ToList().Sum(d => d.CommissionAmount);
        }

        public decimal GetAgentComissionIncome(Guid agentId)
        {
            return _context.Deals
                .Where(d => d.AgentId == agentId)
                .ToList()
                .Sum(d => d.CommissionAmount);
        }
    }
}