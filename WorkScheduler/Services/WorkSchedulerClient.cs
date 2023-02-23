using RestSharp;
using System.Diagnostics;
using System.Net;
using System.Security.Authentication;
using WorkScheduler.Models;

namespace WorkScheduler.Services
{
    public class WorkSchedulerClient : IWorkSchedulerClient
    {
        private readonly RestClient _client;

        public WorkSchedulerClient() : this("http://localhost:5000") { }

        public WorkSchedulerClient(string endPoint)
        {
            _client = new RestClient(endPoint);
        }

        public async Task LoginAsync(string userName, string password)
        {
            var request = new RestRequest($"/account/login?redirectUrl=", Method.Post);
            request.AddParameter("Username", userName);
            request.AddParameter("Password", password);

            var response = await _client.PostAsync(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Cookies.Count > 0)
            {
                var cookie = response.Cookies.First();
                _client.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
                return;
            }

            throw new AuthenticationException("failed to login. wrong username or wrong password");
        }

        public async Task LogoutAsync()
        {
            var request = new RestRequest($"/account/logout", Method.Post);
            var response = await _client.PostAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            Debug.Assert(false, response.ErrorMessage);
        }

        public async Task DeleteScheduleAsync(DateTime target)
        {
            var request = new RestRequest($"/api/workschedule/{target.Year}/{target.Month}/{target.Day}");
            await _client.DeleteAsync(request);
        }

        public async Task<IEnumerable<Schedule>> GetScheduleAsync(DateTime target)
        {
            var request = new RestRequest($"/api/workschedule/{target.Year}/{target.Month}/{target.Day}");
            return await _client.GetAsync<IEnumerable<Schedule>>(request);
        }

        public async Task AddScheduleAsync(InputDetailsContact addSchedule)
        {
            var eventInfo = new EventInfo
            {
                Date = addSchedule.StartTime.ToString("yyyy-MM-dd"),
                StartTime = addSchedule.StartTime.ToString("yyyy-MM-ddTHH:mm"), // Format:"2023-01-30T08:40"
                EndTime = addSchedule.EndTime.ToString("yyyy-MM-ddTHH:mm"),
                WorkStyle = addSchedule.WorkStyle,
                WorkingPlace = addSchedule.WorkingPlace,
            };

            var target = addSchedule.StartTime.Date;
            var request = new RestRequest($"/api/workschedule/{target.Year}/{target.Month}/{target.Day}");
            request.AddBody(eventInfo);
            var response = await _client.PostAsync(request);
            if (!response.IsSuccessful)
            {
                throw new HttpRequestException();
            }
        }
    }
}
