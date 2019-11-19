using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Extensions
{
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult()
        {
            StatusCode = (int)HttpStatusCode.NotFound;
            ViewName = "_404NotFound";
        }
    }
}
