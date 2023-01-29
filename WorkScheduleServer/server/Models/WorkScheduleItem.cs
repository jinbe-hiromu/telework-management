using System;

namespace WorkScheduleServer.Models
{
    public class WorkScheduleItem
    {
        public DateTime? Date
        {
            get;
            set;
        }
        public DateTime? StartTime
        {
            get;
            set;
        }
        public DateTime? EndTime
        {
            get;
            set;
        }
        public string WorkStyle
        {
            get;
            set;
        }
        public string WorkingPlace
        {
            get;
            set;
        }
    }
}
