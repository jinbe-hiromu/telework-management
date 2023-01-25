using Syncfusion.Maui.Scheduler;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkScheduler.ViewModels
{
    public class SchedulerViewModel
    {
        public ObservableCollection<SchedulerAppointment> SchedulerEvents { get; set; }
        public SchedulerViewModel()
        {
            SchedulerEvents = new ObservableCollection<SchedulerAppointment>
            {
                new SchedulerAppointment {StartTime=new DateTime(2023, 1, 23, 18,0,0),EndTime=new DateTime(2023, 1, 23, 20, 0,0),Subject="テスト"}
            };
        }
    }
}
