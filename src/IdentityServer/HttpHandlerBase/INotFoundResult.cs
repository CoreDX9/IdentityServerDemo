using IdentityServer.Extensions;

namespace IdentityServer.HttpHandlerBase
{
    public interface INotFoundResult
    {
        NotFoundViewResult NotFoundView();
        NotFoundViewResult NotFoundView(object model);
        NotFoundViewResult NotFoundView(string viewName);
        NotFoundViewResult NotFoundView(string viewName, object model);
    }
}