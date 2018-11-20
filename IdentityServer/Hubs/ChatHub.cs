using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdentityServer.Hubs
{
    public interface IChatServer
    {
        Task SendMessage(string message);
        Task SendMessageToCaller(string message);
        Task SendMessageToGroups(string message);
        Task SendClearOnlineUsersChatBoardCommand();
    }

    [Authorize]
    public class ChatHub : Hub<IChatClient>, IChatServer
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.ReceiveMessage(Context.User.GetDisplayName(), message);
        }

        public Task SendMessageToCaller(string message)
        {
            return Clients.Caller.ReceiveMessage(message);
        }

        public Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            return Clients.Groups(groups).ReceiveMessage(message);
        }

        public Task SendClearOnlineUsersChatBoardCommand()
        {
            return Clients.All.ReceiveClearChatBoardCommand(Context.User.GetDisplayName());
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.Others.ReceiveOnlineNotice(Context.User.GetDisplayName());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.Others.ReceiveOfflineNotice(Context.User.GetDisplayName());
            await base.OnDisconnectedAsync(exception);
        }
    }
}