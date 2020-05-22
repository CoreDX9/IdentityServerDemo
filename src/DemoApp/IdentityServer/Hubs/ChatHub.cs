using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdentityServer.Hubs
{
    //[Authorize] //安卓连接的时候只能用 jwt 认证，但是目前已经使用了 cookie 认证，两边会打架，有一个不起作用，先取消强制认证
    public class ChatHub : Hub<IChatClient>, IChatServer
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.ReceiveMessage(GetDisplayName(), message);
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
            return Clients.All.ReceiveClearChatBoardCommand(GetDisplayName());
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.Others.ReceiveOnlineNotice(GetDisplayName());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.Others.ReceiveOfflineNotice(GetDisplayName());
            await base.OnDisconnectedAsync(exception);
        }

        private string GetDisplayName() => Context?.User?.GetDisplayName() ?? Context.ConnectionId;
    }
}