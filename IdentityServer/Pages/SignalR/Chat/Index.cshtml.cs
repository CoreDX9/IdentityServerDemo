using IdentityServer.HttpHandlerBase;
using IdentityServer.Hubs;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace IdentityServer.Pages.SignalR.Chat
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly IHubContext<ChatHub, IChatClient> _chatHubContext;

        public IndexModel(IHubContext<ChatHub, IChatClient> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }
        public void OnGet()
        {

        }

        public void OnGetClearOnlineUsersChatBoard()
        {
            _chatHubContext.Clients.All.ReceiveClearChatBoardCommand(User.GetDisplayName());
        }
    }
}