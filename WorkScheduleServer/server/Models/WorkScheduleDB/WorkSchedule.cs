using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkScheduleServer.Models.WorkScheduleDb
{
  [Table("WorkSchedule")]
  public partial class WorkSchedule
  {
    [Key]
    public int? Id
    {
      get;
      set;
    }
    public int? User
    {
      get;
      set;
    }

    [Column("Action")]
    public int? Action1
    {
      get;
      set;
    }
  }
}
