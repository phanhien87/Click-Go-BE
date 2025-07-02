using Microsoft.AspNetCore.SignalR;

namespace Click_Go.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            Console.WriteLine($"User connected: {userId}");
            await base.OnConnectedAsync();
        }
    }
}
