using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace WebAPI.Hubs
{
    public class ChatHub : Hub
    {
        // Gửi tin nhắn đến 1 conversation (group)
        public async Task SendMessageToConversation(string conversationId, string sender, string message)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", sender, message);
        }

        // Tham gia vào 1 conversation (group)
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        // Rời khỏi 1 conversation (group)
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }
    }
}