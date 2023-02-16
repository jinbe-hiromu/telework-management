namespace WorkScheduler.Models
{
    public interface IWorkSchedulerClient
    {
        public Task LoginAsync(string userName, string password);
        public Task LogoutAsync();
        public Task DeleteScheduleAsync(DateTime day);
        public Task<IEnumerable<Schedule>> GetScheduleAsync(DateTime day);
        public Task AddScheduleAsync(InputDetailsContact addSchedule);
    }
}