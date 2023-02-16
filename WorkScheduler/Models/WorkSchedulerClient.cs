using RestSharp;
using System.Net;
using System.Security.Authentication;

namespace WorkScheduler.Models
{
    public class WorkSchedulerClient : IWorkSchedulerClient
    {
        private readonly RestClient _client;

        public WorkSchedulerClient() : this("http://localhost:5000") { }

        public WorkSchedulerClient(string endPoint) {
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
            var request = new RestRequest($"/account/logout?redirectUrl=", Method.Post);
            var response = await _client.PostAsync(request);
            
            if (response.StatusCode == HttpStatusCode.OK && response.Cookies.Count > 0)
            {
                return;
            }
            throw new AuthenticationException("failed to logout. ");
        }

        public async Task DeleteScheduleAsync(DateTime day)
        {
            var request = new RestRequest($"/api/workschedule/{day.Year}/{day.Month}/{day.Day}");
            await _client.DeleteAsync(request);
        }

        public async Task<IEnumerable<Schedule>> GetScheduleAsync(DateTime day)
        {
            var request = new RestRequest($"/api/workschedule/{day.Year}/{day.Month}/{day.Day}");
            return await _client.GetAsync<IEnumerable<Schedule>>(request);
        }

        public async Task AddScheduleAsync(InputDetailsContact addSchedule)
        {
            var eventInfo = new EventInfo
            {
                Date = addSchedule.Date.ToString("yyyy-MM-dd"),
                StartTime = addSchedule.Date.ToString("yyyy-MM-dd") + "T" + addSchedule.StartTime.ToString(@"hh\:mm"),        // Format:"2023-01-30T08:40"
                EndTime = addSchedule.Date.ToString("yyyy-MM-dd") + "T" + addSchedule.EndTime.ToString(@"hh\:mm"),
                WorkStyle = addSchedule.WorkStyle,
                WorkingPlace = addSchedule.WorkingPlace,
            };

            var request = new RestRequest($"/api/workschedule/{addSchedule.Date.Year}/{addSchedule.Date.Month}/{addSchedule.Date.Day}");
            request.AddBody(eventInfo);
            var response = await _client.PostAsync(request);
            if (!response.IsSuccessful){
                throw new HttpRequestException();
            }
        }
    }
}
