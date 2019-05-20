using System;
using System.Threading.Tasks;

namespace IdentityServer.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
        Task ReceiveMessage(string message);
        Task ReceiveOnlineNotice(string user);
        Task ReceiveOfflineNotice(string user);
        Task ReceiveClearChatBoardCommand(string user);
    }
}