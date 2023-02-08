using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduler.Models
{
    public class Schedule
    {
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? WorkStyle { get; set; }
        public string? WorkingPlace { get; set; }
        public ScheduleType? ScheduleType { get; set; }
    }
}
