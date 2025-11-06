using RealEstateAgency.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace RealEstateAgency.Services
{
    public static class DealStatsService
    {
        private static IEnumerable<Deal> FilterByDate(List<Deal> allDeals, DateTime? startDate, DateTime? endDate)
        {
            var query = allDeals.AsEnumerable();

            if (startDate.HasValue)
            {
                query = query.Where(d => d.Date.Date >= startDate.Value.Date);
            }
            if (endDate.HasValue)
            {
                var inclusiveEndDate = endDate.Value.Date.AddDays(1);
                query = query.Where(d => d.Date < inclusiveEndDate);
            }
            return query;
        }
        public static int GetDealCountForPeriod(List<Deal> allDeals, DateTime startDate, DateTime endDate)
        {
            return FilterByDate(allDeals, startDate, endDate).Count();
        }
        public static decimal GetTotalSalesAmount(List<Deal> allDeals, DateTime? startDate = null, DateTime? endDate = null)
        {
            var deals = FilterByDate(allDeals, startDate, endDate);

            return deals
                .Where(d => d.Type == DealType.Purchase)
                .Sum(d => d.FinalPrice);
        }
        public static decimal GetAveragePricePerSquareMeter(List<Deal> allDeals, DateTime? startDate = null, DateTime? endDate = null)
        {
            var purchaseDeals = FilterByDate(allDeals, startDate, endDate)
                .Where(d => d.Type == DealType.Purchase && d.Property != null && d.Property.Square > 0)
                .ToList();

            if (!purchaseDeals.Any())
            {
                return 0;
            }

            decimal totalPrice = purchaseDeals.Sum(d => d.FinalPrice);
            decimal totalSquare = purchaseDeals.Sum(d => d.Property.Square);

            return (totalSquare == 0) ? 0 : (totalPrice / totalSquare);
        }


        public static List<AgentEfficiencyStats> GetAgentEfficiency(List<Deal> allDeals, DateTime? startDate = null, DateTime? endDate = null)
        {
            var deals = FilterByDate(allDeals, startDate, endDate);

            return deals
                .GroupBy(d => d.Agent)
                .Select(group => new AgentEfficiencyStats
                {
                    AgentName = group.Key.FullName,
                    DealCount = group.Count(),
                    TotalCommission = group.Sum(d => d.CommissionAmount)
                })
                .OrderByDescending(stats => stats.TotalCommission)
                .ToList();
        }

        public static List<ClientActivityStats> GetMostActiveClients(List<Deal> allDeals, DateTime? startDate = null, DateTime? endDate = null)
        {
            var deals = FilterByDate(allDeals, startDate, endDate);

            return deals
                .GroupBy(d => d.Client)
                .Select(group => new ClientActivityStats
                {
                    ClientName = group.Key.FullName,
                    DealCount = group.Count()
                })
                .OrderByDescending(stats => stats.DealCount)
                .ToList();
        }
    }
}