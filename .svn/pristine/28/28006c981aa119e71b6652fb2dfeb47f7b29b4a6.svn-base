using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace IdentityServer
{
    /// <summary>
    /// DisplayName-related extensions for <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper" /> and <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper`1" />.
    /// </summary>
    public static class HtmlHelperDisplayNameExtensions
    {
        /// <summary>
        /// Returns the display name for the specified <paramref name="expression" />
        /// if the current model represents a collection.
        /// </summary>
        /// <param name="htmlHelper">
        /// The <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper`1" /> of <see cref="T:System.Collections.Generic.IEnumerable`1" /> instance this method extends.
        /// </param>
        /// <param name="expression">An expression to be evaluated against an item in the current model.</param>
        /// <typeparam name="TModelItem">The type of items in the model collection.</typeparam>
        /// <typeparam name="TResult">The type of the <paramref name="expression" /> result.</typeparam>
        /// <returns>A <see cref="T:System.String" /> containing the display name.</returns>
        public static string DisplayNameFor<TModelItem, TResult>(this IHtmlHelper<IPagedList<TModelItem>> htmlHelper, Expression<Func<TModelItem, TResult>> expression)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException(nameof(htmlHelper));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            return htmlHelper.DisplayNameForInnerType(expression);
        }
    }
}
