using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkScheduleServer.Models.WorkScheduleDb
{
  [Table("ActionItem")]
  public partial class ActionItem
  {
    [Key]
    public int Id
    {
      get;
      set;
    }
    public DateTime From
    {
      get;
      set;
    }
    public DateTime To
    {
      get;
      set;
    }
    public string EventName
    {
      get;
      set;
    }
    public string Notes
    {
      get;
      set;
    }
    public string BackgroudColor
    {
      get;
      set;
    }
  }
}
