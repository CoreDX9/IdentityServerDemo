using IdentityServer.HttpHandlerBase;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordConfirmationModel : PageModelBase
    {
        public void OnGet()
        {

        }
    }
}
