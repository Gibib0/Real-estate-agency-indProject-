using System;

namespace BusinessLogic.Models
{
    public class AgentEfficiencyStats
    {
        public string AgentName { get; set; }
        public int DealCount { get; set; }
        public decimal TotalCommission { get; set; }
    }

    public class ClientActivityStats
    {
        public string ClientName { get; set; }
        public int DealCount { get; set; }
    }
}