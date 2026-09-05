using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LibrarySystem.API.Hubs
{
   
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ConnectionTest", Context.ConnectionId);
            await base.OnConnectedAsync();
        }


        public async Task SendToAll(string user, string message)
        {
            // Save Message to Database

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendToGroup(string groupName, string user, string message)
        {
            await Clients.Group(groupName)
                .SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
