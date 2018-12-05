using System.Threading.Tasks;

namespace IdentityServer.Hubs
{
    public interface IChatServer
    {
        Task SendMessage(string message);
        Task SendMessageToCaller(string message);
        Task SendMessageToGroups(string message);
        Task SendClearOnlineUsersChatBoardCommand();
    }
}
