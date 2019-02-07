using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPagesCart.Hubs
{
    public class ChatHub : /*Hub*/ Hub<IChatClient> //Generic overload with strongly typed methods
    {
        public async Task ReceiverMethodOnServer(string user, string message)
        {
            await Clients.All.ReceivedMessageFromServer(user, message);
        }

        //public Task SendMessageToCaller(string message)
        //{
        //    return Clients.Caller.SendAsync("ReceiveMessage", message);
        //}

        //public Task SendMessageToGroups(string message)
        //{
        //    List<string> groups = new List<string>() { "SignalR Users" };
        //    return Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        //}

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }

    public interface IChatClient
    {
        Task ReceivedMessageFromServer(string user, string message);
        //Task ReceiveMessage(string message);
    }
}
