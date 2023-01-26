using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkScheduleServer.Models.WorkScheduleDb
{
  [Table("Acount")]
  public partial class Acount
  {
    [Key]
    public string Id
    {
      get;
      set;
    }
    public string Name
    {
      get;
      set;
    }
    public string Password
    {
      get;
      set;
    }
    public string Notes
    {
      get;
      set;
    }
  }
}
