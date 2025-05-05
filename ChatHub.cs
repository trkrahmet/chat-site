using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Proje
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();

        public async Task RegisterUser(string username)
        {
            var connectionId = Context.ConnectionId;
            Users[connectionId] = username;
            await Clients.All.SendAsync("UserJoined", username);
        }

        public async Task SendMessage(string message)
        {
            var connectionId = Context.ConnectionId;
            if (Users.TryGetValue(connectionId, out var username))
            {
                await Clients.All.SendAsync("ReceiveMessage", username, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Users.TryRemove(Context.ConnectionId, out var username))
            {
                await Clients.All.SendAsync("UserLeft", username);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}