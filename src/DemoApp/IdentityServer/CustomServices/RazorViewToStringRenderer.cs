using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace IdentityServer.CustomServices
{
    /// <summary>
    /// 将Razor视图渲染为字符串
    /// </summary>
    public class RazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 渲染并返回结果（在非Web环境中不支持依赖UrlHelper的链接生成，如果需要，可以在Asp.Net Core 2.2之后注入并使用LinkGenerator）
        /// </summary>
        /// <typeparam name="TModel">视图模型类型</typeparam>
        /// <param name="viewName">视图名称（或视图路径）</param>
        /// <param name="model">视图模型</param>
        /// <param name="routeData">路由数据，传入实际数据可以使用相对路径查找视图、视图引用的部分视图和其他资源，一般直接传入HttpContext中的RouteData</param>
        /// <param name="actionDescriptor">动作描述信息，传入实际数据可以使用相对路径查找视图、视图引用的部分视图和其他资源，一般直接传入HttpContext中的ActionDescriptor</param>
        /// <param name="httpContext">Http上下文</param>
        /// <param name="modelStateDictionary">模型状态字典</param>
        /// <returns></returns>
        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, RouteData routeData = null, ActionDescriptor actionDescriptor = null, HttpContext httpContext = null, ModelStateDictionary modelStateDictionary = null)
        {
            var actionContext = GetActionContext(routeData, actionDescriptor, httpContext, modelStateDictionary);
            var view = FindView(actionContext, viewName);

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                return output.ToString();
            }
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext(RouteData routeData = null, ActionDescriptor actionDescriptor = null, HttpContext httpContext = null, ModelStateDictionary modelStateDictionary = null)
        {
            var defaultHttpContext = httpContext
                ?? new DefaultHttpContext
                {
                    RequestServices = _serviceProvider,
                };

            return new ActionContext(defaultHttpContext, routeData ?? new RouteData(), actionDescriptor ?? new ActionDescriptor(), modelStateDictionary ?? new ModelStateDictionary());
        }
    }

    public static class RazorViewToStringRendererExtensions
    {
        public static async Task<string> RenderViewToStringAsync<TModel>(this RazorViewToStringRenderer razorViewToStringRenderer,string viewName, TModel model, ControllerContext controllerContext)
        {
            return await razorViewToStringRenderer.RenderViewToStringAsync(viewName, model, controllerContext.RouteData,
                controllerContext.ActionDescriptor, controllerContext.HttpContext, controllerContext.ModelState);
        }
    }
}
