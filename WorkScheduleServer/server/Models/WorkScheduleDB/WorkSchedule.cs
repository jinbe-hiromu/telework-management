using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkScheduleServer.Models.WorkScheduleDb
{
  [Table("WorkSchedule")]
  public partial class WorkSchedule
  {
    [Key]
    public int Id
    {
      get;
      set;
    }
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
    public string User
    {
      get;
      set;
    }
  }
}
