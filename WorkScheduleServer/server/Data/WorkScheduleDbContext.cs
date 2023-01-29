using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

using WorkScheduleServer.Models.WorkScheduleDb;

namespace WorkScheduleServer.Data
{
  public partial class WorkScheduleDbContext : Microsoft.EntityFrameworkCore.DbContext
  {
    public WorkScheduleDbContext(DbContextOptions<WorkScheduleDbContext> options):base(options)
    {
    }

    public WorkScheduleDbContext()
    {
    }

    partial void OnModelBuilding(ModelBuilder builder);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        this.OnModelBuilding(builder);
    }


    public DbSet<WorkScheduleServer.Models.WorkScheduleDb.WorkSchedule> WorkSchedules
    {
      get;
      set;
    }
  }
}
