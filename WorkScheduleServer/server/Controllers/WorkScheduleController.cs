using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkScheduleServer.Data;
using WorkScheduleServer.Models;
using WorkScheduleServer.Models.WorkScheduleDb;

namespace WorkScheduleServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly WorkScheduleDbContext _dbContext;

        public WorkScheduleController(WorkScheduleDbContext workScheduleDbContext)
        {
            _dbContext = workScheduleDbContext;
        }

        // GET <host>/api/WorkSchedule/<year>/<month>[/<day>]
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // [Request]
        // Header {
        //   Content-Type: application/json
        // }
        // [Response]
        // Body [{
        //    "Date"        : "2023-01-30",
        //    "StartTime"   : "2023-01-30T08:40",
        //    "EndTime"     : "2023-01-30T17:40",
        //    "WorkStyle"   : "出社",　// 出張,テレワーク,有休
        //    "WorkingPlace"   : "阿久比"　// 刈谷,自宅,その他
        // }]
        [HttpGet("{year:int}/{month:int}/{day:int?}")]
        [Authorize]
        public async Task<IActionResult> Get(int year, int month, int? day)
        {
            var username = User.Identity.Name;

            SetDateParameter(year, month, day, out var dateBegin, out var dateEnd);

            var workScheduleList = _dbContext.WorkSchedules
                .Where(i => (dateBegin <= i.Date && i.Date < dateEnd) && i.User == username);

            var items = new List<WorkScheduleItem>();
            foreach (var workSchedule in workScheduleList)
            {
                items.Add(
                    new WorkScheduleItem()
                    {
                        Date = workSchedule.Date,
                        StartTime = workSchedule.StartTime,
                        EndTime = workSchedule.EndTime,
                        WorkStyle = workSchedule.WorkStyle,
                        WorkingPlace = workSchedule.WorkingPlace
                    }
                );
            }
            return Ok(items);

            static void SetDateParameter(int year, int month, int? day, out DateTime dateBegin, out DateTime dateEnd)
            {
                if (day.HasValue)
                {
                    dateBegin = new DateTime(year, month, day.Value);
                    dateEnd = dateBegin.AddDays(1);
                }
                else
                {
                    dateBegin = new DateTime(year, month, 1);
                    dateEnd = dateBegin.AddMonths(1);
                }
            }
        }

        // POST <host>/api/WorkSchedule/<year>/<month>/<day>
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // Header {
        //   Content-Type: application/json
        // }
        //Body {
        //    "Date"        : "2023-01-30",
        //    "StartTime"   : "2023-01-30T08:40",
        //    "EndTime"     : "2023-01-30T17:40",
        //    "WorkStyle"   : "出社",　// 出張,テレワーク,有休
        //    "WorkingPlace"   : "阿久比"　// 刈谷,自宅,その他
        // }
        [HttpPost("{year:int}/{month:int}/{day:int}")]
        [Authorize]
        public async Task<IActionResult> Post(int year, int month, int day, [FromBody] WorkScheduleItem workScheduleItem)
        {
            var username = User.Identity.Name;

            try
            {
                DateTime date = new DateTime(year, month, day);
                var workSchedule = _dbContext.WorkSchedules
                    .FirstOrDefault<WorkSchedule>(i => i.Date == date && i.User == username);

                if (workSchedule != null)
                {
                    // Already exists
                    // workSchedule.Id = workSchedule.Id;
                    workSchedule.StartTime = workScheduleItem.StartTime;
                    workSchedule.EndTime = workScheduleItem.EndTime;
                    workSchedule.WorkStyle = workScheduleItem.WorkStyle;
                    workSchedule.WorkingPlace = workScheduleItem.WorkingPlace;

                    // Update
                    _dbContext.WorkSchedules.Update(workSchedule);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    int id = _dbContext.WorkSchedules.Max<WorkSchedule>(i => i.Id);
                    workSchedule = new WorkSchedule()
                    {
                        Id = id + 1,
                        Date = date,
                        StartTime = workScheduleItem.StartTime,
                        EndTime = workScheduleItem.EndTime,
                        WorkStyle = workScheduleItem.WorkStyle,
                        WorkingPlace = workScheduleItem.WorkingPlace,
                        User = username
                    };

                    // Add
                    await _dbContext.WorkSchedules.AddAsync(workSchedule);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid request.");
            }
        }

        // PUT <host>/api/WorkSchedule/<year>/<month>/<day>
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // Header {
        //   Content-Type: application/json
        // }
        //Body {
        //    "Date"        : "2023-01-30",
        //    "StartTime"   : "2023-01-30T09:00",
        //    "EndTime"     : "2023-01-30T18:00",
        //    "WorkStyle"   : "出社",　// 出張,テレワーク,有休
        //    "WorkingPlace"   : "阿久比"　// 刈谷,自宅,その他
        // }
        [HttpPut("{year:int}/{month:int}/{day:int}")]
        [Authorize]
        public async Task<IActionResult> Put(int year, int month, int day, [FromBody] WorkScheduleItem workScheduleItem)
        {
            return await Post(year, month, day, workScheduleItem);
        }

        // DELETE <host>/api/WorkSchedule/<year>/<month>/<day>
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // Header {
        //   Content-Type: application/json
        // }
        [HttpDelete("{year:int}/{month:int}/{day:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int year, int month, int day)
        {
            var username = User.Identity.Name;

            try
            {
                DateTime date = new DateTime(year, month, day);
                var workSchedule = _dbContext.WorkSchedules
                    .FirstOrDefault<WorkSchedule>(i => i.Date == date && i.User == username);

                if (workSchedule != null)
                {
                    // Exists, then remove it.
                    _dbContext.WorkSchedules.Remove(workSchedule);
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    // No data to delete.
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid request.");
            }
        }
    }
}
