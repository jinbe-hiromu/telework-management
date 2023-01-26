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


        builder.Entity<WorkScheduleServer.Models.WorkScheduleDb.ActionItem>()
              .Property(p => p.Id)
              .HasDefaultValueSql("'0'").ValueGeneratedNever();

        builder.Entity<WorkScheduleServer.Models.WorkScheduleDb.ActionItem>()
              .Property(p => p.EventName)
              .HasDefaultValueSql("''");

        this.OnModelBuilding(builder);
    }


    public DbSet<WorkScheduleServer.Models.WorkScheduleDb.Acount> Acounts
    {
      get;
      set;
    }

    public DbSet<WorkScheduleServer.Models.WorkScheduleDb.ActionItem> ActionItems
    {
      get;
      set;
    }

    public DbSet<WorkScheduleServer.Models.WorkScheduleDb.WorkSchedule> WorkSchedules
    {
      get;
      set;
    }
  }
}
