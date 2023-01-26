using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkScheduleServer.Data;

namespace WorkScheduleServer
{
    public partial class ExportWorkScheduleDbController : ExportController
    {
        private readonly WorkScheduleDbContext context;
        private readonly WorkScheduleDbService service;
        public ExportWorkScheduleDbController(WorkScheduleDbContext context, WorkScheduleDbService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/WorkScheduleDb/acounts/csv")]
        [HttpGet("/export/WorkScheduleDb/acounts/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportAcountsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAcounts(), Request.Query), fileName);
        }

        [HttpGet("/export/WorkScheduleDb/acounts/excel")]
        [HttpGet("/export/WorkScheduleDb/acounts/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportAcountsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAcounts(), Request.Query), fileName);
        }
        [HttpGet("/export/WorkScheduleDb/actionitems/csv")]
        [HttpGet("/export/WorkScheduleDb/actionitems/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportActionItemsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetActionItems(), Request.Query), fileName);
        }

        [HttpGet("/export/WorkScheduleDb/actionitems/excel")]
        [HttpGet("/export/WorkScheduleDb/actionitems/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportActionItemsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetActionItems(), Request.Query), fileName);
        }
        [HttpGet("/export/WorkScheduleDb/workschedules/csv")]
        [HttpGet("/export/WorkScheduleDb/workschedules/csv(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportWorkSchedulesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetWorkSchedules(), Request.Query), fileName);
        }

        [HttpGet("/export/WorkScheduleDb/workschedules/excel")]
        [HttpGet("/export/WorkScheduleDb/workschedules/excel(fileName='{fileName}')")]
        public async System.Threading.Tasks.Task<FileStreamResult> ExportWorkSchedulesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetWorkSchedules(), Request.Query), fileName);
        }
    }
}
