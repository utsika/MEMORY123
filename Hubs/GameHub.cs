using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace MEMORY.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinRoom(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        }

        public async Task GameUpdated(string roomCode)
        {
            await Clients.Group(roomCode).SendAsync("RefreshGame");
        }
    }
}
