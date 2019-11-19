using IdentityServer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServer.HttpHandlerBase
{
    //[MyAuthrize]
    public abstract class PageModelBase : PageModel , INotFoundResult
    {
        /// <summary>
        /// Creates a <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object that renders a view to the response.
        /// </summary>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object for the response.</returns>
        [NonAction]
        public virtual NotFoundViewResult NotFoundView()
        {
            return NotFoundView("_404NotFound");
        }

        /// <summary>
        /// Creates a <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object by specifying a <paramref name="viewName" />.
        /// </summary>
        /// <param name="viewName">The name or path of the view that is rendered to the response.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object for the response.</returns>
        [NonAction]
        public virtual NotFoundViewResult NotFoundView(string viewName)
        {
            return NotFoundView(viewName, ViewData.Model);
        }

        /// <summary>
        /// Creates a <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object by specifying a <paramref name="model" />
        /// to be rendered by the view.
        /// </summary>
        /// <param name="model">The model that is rendered by the view.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object for the response.</returns>
        [NonAction]
        public virtual NotFoundViewResult NotFoundView(object model)
        {
            return NotFoundView(null, model);
        }

        /// <summary>
        /// Creates a <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object by specifying a <paramref name="viewName" />
        /// and the <paramref name="model" /> to be rendered by the view.
        /// </summary>
        /// <param name="viewName">The name or path of the view that is rendered to the response.</param>
        /// <param name="model">The model that is rendered by the view.</param>
        /// <returns>The created <see cref="T:Microsoft.AspNetCore.Mvc.ViewResult" /> object for the response.</returns>
        [NonAction]
        public virtual NotFoundViewResult NotFoundView(string viewName, object model)
        {
            ViewData.Model = model;
            return new NotFoundViewResult
            {
                ViewName = viewName,
                ViewData = ViewData,
                TempData = TempData
            };
        }
    }
}
