using IdentityServer.HttpHandlerBase;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage
{
    public class ShowRecoveryCodesModel : PageModelBase
    {

        [TempData]
        public string[] RecoveryCodes { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = "Error : Your recovery codes are already shown once before.";
            }
        }
    }
}