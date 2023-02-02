using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduler.Models
{
    public class InputDetailsContact
    {
        public DateTime Date { get; init; }
        public TimeSpan StartTime { get; init; }
        public TimeSpan EndTime { get; init; }
        public string WorkStyle { get; init; }
        public string WorkingPlace { get; init; }
    }
}
