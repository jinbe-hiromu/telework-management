using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using WorkScheduleServer.Data;
using WorkScheduleServer.Models;
using WorkScheduleServer.Models.WorkScheduleDb;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorkScheduleServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        public class AccountInfo
        {
            public string UserName;
            public string Password;
        };

        private readonly WorkScheduleDbContext workScheduleDbContext;

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IWebHostEnvironment env;

        public WorkScheduleController(IWebHostEnvironment env, WorkScheduleDbContext workScheduleDbContext, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.workScheduleDbContext = workScheduleDbContext;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.env = env;
        }

        // POST <host>/api/WorkSchedule/Login
        // ex. http://127.0.0.1:5000/api/WorkSchedule/Login
        // [Request]
        // Header {
        //   Content-Type: application/json
        //   AccessToken: <アクセストークン>
        // }
        //  Body {
        //      "AccountInfo" ; {
        //        "username" : "<ユーザー名>",
        //        "password" : "<パスワード>"
        //      }
        //  }
        // [Response]
        //  Body {
        //      "AccessToken" : "<アクセストークン>"
        //  }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AccountInfo account)
        {
            if (!string.IsNullOrEmpty(account.UserName) && !string.IsNullOrEmpty(account.Password))
            {
                var result = await signInManager.PasswordSignInAsync(account.UserName, account.Password, false, false);
                if (result.Succeeded)
                {
                    // Return access_token
                    var ResponseBody = new { AccessToken = AccessTokenManager.CreateAccessToken(account.UserName) };
                    return Ok(ResponseBody);
                }
            }

            // Invalid user or password
            // 401:Unauthorized
            return Unauthorized("Invalid user or password.");
        }

        // POST <host>/api/WorkSchedule/Logout
        // ex. http://127.0.0.1:5000/api/WorkSchedule/Logout
        // [Request]
        // Header {
        //   Content-Type: application/json
        //   AccessToken: <アクセストークン>
        // }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromHeader] string accessToken)
        {
            var user = await AccessTokenManager.GetUserFormAccessToken(accessToken, signInManager, userManager, roleManager);

            if (user == null)
            {
                return BadRequest("Invalid access token.");
            }

            AccessTokenManager.ReleaseAccessToken(accessToken);
            await signInManager.SignOutAsync();

            return Ok($"User[{user.UserName}] logout.");
        }

        // GET <host>/api/WorkSchedule/<year>/<month>/<day>
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // [Request]
        // Header {
        //   Content-Type: application/json
        //   AccessToken: <アクセストークン>
        // }
        // [Response]
        // Body {
        //  "WorkScheduleItem" : {　// WorkScheduleServer.Models.WorkScheduleItem
        //    "Date"        : "2023/1/30",
        //    "StartTime"   : "2023/1/30 08:40",
        //    "EndTime"     : "2023/1/30 17:40",
        //    "WorkStyle"   : "出社",　// 出張,テレワーク,有休
        //    "WorkingPlace"   : "阿久比"　// 刈谷,自宅,その他
        //  }
        // }
        [HttpGet("{year:int}/{month:int}/{day:int}")]
        public async Task<IActionResult> Get([FromHeader] string accessToken, int year, int month, int day)
        {
            var user = await AccessTokenManager.GetUserFormAccessToken(accessToken, signInManager, userManager, roleManager);

            if (user == null)
            {
                return BadRequest("Invalid access token.");
            }

            try
            {
                DateTime date = new DateTime(year, month, day);
                var workSchedule = workScheduleDbContext.WorkSchedules
                    .FirstOrDefault<WorkSchedule>(i => i.Date == date);
                if (workSchedule == null)
                {
                    return BadRequest($"[{date}] No schedule. It is empty.");
                }

                WorkScheduleItem item = new WorkScheduleItem()
                {
                    Date = workSchedule.Date,
                    StartTime = workSchedule.StartTime,
                    EndTime = workSchedule.EndTime,
                    WorkStyle = workSchedule.WorkStyle,
                    WorkingPlace = workSchedule.WorkingPlace
                };

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Invalid request.");
            }
        }

        // POST <host>/api/WorkSchedule/<year>/<month>/<day>
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // Header {
        //   Content-Type: application/json
        //   AccessToken: <アクセストークン>
        // }
        //Body {
        //  "WorkScheduleItem" : {　// WorkScheduleServer.Models.WorkScheduleItem
        //    "Date"        : "2023/1/30",
        //    "StartTime"   : "2023/1/30 08:40",
        //    "EndTime"     : "2023/1/30 17:40",
        //    "WorkStyle"   : "出社",　// 出張,テレワーク,有休
        //    "WorkingPlace"   : "阿久比"　// 刈谷,自宅,その他
        //  }
        // }
        [HttpPost("{year:int}/{month:int}/{day:int}")]
        public async Task<IActionResult> Post(
            [FromHeader] string accessToken,
            int year, int month, int day,
            [FromBody] WorkScheduleItem item)
        {
            var user = await AccessTokenManager.GetUserFormAccessToken(accessToken, signInManager, userManager, roleManager);

            if (user == null)
            {
                return BadRequest("Invalid access token.");
            }

            try
            {
                DateTime date = new DateTime(year, month, day);
                var workSchedule = workScheduleDbContext.WorkSchedules
                    .FirstOrDefault<WorkSchedule>(i => i.Date == date);

                if (workSchedule != null)
                {
                    // Already exists
                    // workSchedule.Id = workSchedule.Id;
                    workSchedule.StartTime = item.StartTime;
                    workSchedule.EndTime = item.EndTime;
                    workSchedule.WorkStyle = item.WorkStyle;
                    workSchedule.WorkingPlace = item.WorkingPlace;

                    // Update
                    workScheduleDbContext.WorkSchedules.Update(workSchedule);
                    await workScheduleDbContext.SaveChangesAsync();
                }
                else
                {
                    int id = workScheduleDbContext.WorkSchedules.Max<WorkSchedule>(i => i.Id);
                    workSchedule = new WorkSchedule()
                    {
                        Id = id + 1,
                        Date = date,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        WorkStyle = item.WorkStyle,
                        WorkingPlace = item.WorkingPlace
                    };

                    // Add
                    await workScheduleDbContext.WorkSchedules.AddAsync(workSchedule);
                    await workScheduleDbContext.SaveChangesAsync();
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
        [HttpPut("{year:int}/{month:int}/{day:int}")]
        // Header {
        //   Content-Type: application/json
        //   AccessToken: <アクセストークン>
        // }
        //Body {
        //  "WorkScheduleItem" : {　// WorkScheduleServer.Models.WorkScheduleItem
        //    "Date"        : "2023/1/30",
        //    "StartTime"   : "2023/1/30 08:40",
        //    "EndTime"     : "2023/1/30 17:40",
        //    "WorkStyle"   : "出社",　// 出張,テレワーク,有休
        //    "WorkingPlace"   : "阿久比"　// 刈谷,自宅,その他
        //  }
        // }
        public async Task<IActionResult> Put([FromHeader] string accessToken,
            int year, int month, int day,
            [FromBody] WorkScheduleItem item)
        {
            return await Post(accessToken, year, month, day, item);
        }

        // DELETE <host>/api/WorkSchedule/<year>/<month>/<day>
        // ex. http://127.0.0.1:5000/api/WorkSchedule/2023/1/30
        // Header {
        //   Content-Type: application/json
        //   AccessToken: <アクセストークン>
        // }
        [HttpDelete("{year:int}/{month:int}/{day:int}")]
        public async Task<IActionResult> Delete([FromHeader] string accessToken, int year, int month, int day)
        {
            var user = await AccessTokenManager.GetUserFormAccessToken(accessToken, signInManager, userManager, roleManager);

            if (user == null)
            {
                return BadRequest("Invalid access token.");
            }

            try
            {
                DateTime date = new DateTime(year, month, day);
                var workSchedule = workScheduleDbContext.WorkSchedules
                    .FirstOrDefault<WorkSchedule>(i => i.Date == date);

                if (workSchedule != null)
                {
                    // Exists, then remove it.
                    workScheduleDbContext.WorkSchedules.Remove(workSchedule);
                    await workScheduleDbContext.SaveChangesAsync();
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
