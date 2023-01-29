using Radzen;
using System;
using System.Web;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Data;
using System.Text.Encodings.Web;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using WorkScheduleServer.Data;

namespace WorkScheduleServer
{
    public partial class WorkScheduleDbService
    {
        WorkScheduleDbContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly WorkScheduleDbContext context;
        private readonly NavigationManager navigationManager;

        public WorkScheduleDbService(WorkScheduleDbContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public async Task ExportWorkSchedulesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/workscheduledb/workschedules/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/workscheduledb/workschedules/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportWorkSchedulesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/workscheduledb/workschedules/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/workscheduledb/workschedules/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnWorkSchedulesRead(ref IQueryable<Models.WorkScheduleDb.WorkSchedule> items);

        public async Task<IQueryable<Models.WorkScheduleDb.WorkSchedule>> GetWorkSchedules(Query query = null)
        {
            var items = Context.WorkSchedules.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnWorkSchedulesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnWorkScheduleCreated(Models.WorkScheduleDb.WorkSchedule item);
        partial void OnAfterWorkScheduleCreated(Models.WorkScheduleDb.WorkSchedule item);

        public async Task<Models.WorkScheduleDb.WorkSchedule> CreateWorkSchedule(Models.WorkScheduleDb.WorkSchedule workSchedule)
        {
            OnWorkScheduleCreated(workSchedule);

            var existingItem = Context.WorkSchedules
                              .Where(i => i.Id == workSchedule.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.WorkSchedules.Add(workSchedule);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(workSchedule).State = EntityState.Detached;
                throw;
            }

            OnAfterWorkScheduleCreated(workSchedule);

            return workSchedule;
        }

        partial void OnWorkScheduleDeleted(Models.WorkScheduleDb.WorkSchedule item);
        partial void OnAfterWorkScheduleDeleted(Models.WorkScheduleDb.WorkSchedule item);

        public async Task<Models.WorkScheduleDb.WorkSchedule> DeleteWorkSchedule(int? id)
        {
            var itemToDelete = Context.WorkSchedules
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnWorkScheduleDeleted(itemToDelete);

            Context.WorkSchedules.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterWorkScheduleDeleted(itemToDelete);

            return itemToDelete;
        }

        partial void OnWorkScheduleGet(Models.WorkScheduleDb.WorkSchedule item);

        public async Task<Models.WorkScheduleDb.WorkSchedule> GetWorkScheduleById(int? id)
        {
            var items = Context.WorkSchedules
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            var itemToReturn = items.FirstOrDefault();

            OnWorkScheduleGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        public async Task<Models.WorkScheduleDb.WorkSchedule> CancelWorkScheduleChanges(Models.WorkScheduleDb.WorkSchedule item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnWorkScheduleUpdated(Models.WorkScheduleDb.WorkSchedule item);
        partial void OnAfterWorkScheduleUpdated(Models.WorkScheduleDb.WorkSchedule item);

        public async Task<Models.WorkScheduleDb.WorkSchedule> UpdateWorkSchedule(int? id, Models.WorkScheduleDb.WorkSchedule workSchedule)
        {
            OnWorkScheduleUpdated(workSchedule);

            var itemToUpdate = Context.WorkSchedules
                              .Where(i => i.Id == id)
                              .FirstOrDefault();
            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(workSchedule);
            entryToUpdate.State = EntityState.Modified;
            Context.SaveChanges();       

            OnAfterWorkScheduleUpdated(workSchedule);

            return workSchedule;
        }
    }
}
