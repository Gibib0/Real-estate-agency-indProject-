using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class StatusChange
    {
        public Property.Status OldStatus { get; set; }
        public Property.Status NewStatus { get; set; }
        public DateTime Time { get; set; }
        public string? ChangeBy { get; set; }
    }
}
