using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class Agent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; }
        public int Experience { get; set; }

        public Agent() { }

        public Agent(string fullName, int experience)
        {
            Id = Guid.NewGuid();
            FullName = fullName;
            Experience = experience;
        }
    }
}
