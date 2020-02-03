using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Areas.IdentityServer.ViewComponents
{
    public class IdentityServerLinkViewComponent : ViewComponent
    {
        private readonly IRootConfiguration _configuration;

        public IdentityServerLinkViewComponent(IRootConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            var identityServerUrl = _configuration.AdminConfiguration.IdentityServerBaseUrl;

            return View(model: identityServerUrl);
        }
    }
}






