using Microsoft.AspNetCore.SignalR;
using ScavengeRUs.Data;

namespace ScavengeRUs.Hubs
{
    public class BucStatsHub : Hub
    {
        private readonly GetCompletedTasksForUserQuery _query;
        public BucStatsHub(GetCompletedTasksForUserQuery getCompletedTasksForUserquery)
        {
            _query = getCompletedTasksForUserquery;
        }
        public async Task SendMessage(string user, string message)
        {
            var hunt = await _query.GetAsync(14);
            await Clients.All.SendAsync("ReceiveMessage", user, message);
            var x = 0;
        }
    }
}