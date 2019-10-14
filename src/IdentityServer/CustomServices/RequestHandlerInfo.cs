using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CoreDX.Domain.Model.Entity.Security;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;

namespace IdentityServer.CustomServices
{
    public interface IRequestHandlerInfo
    {
        IEnumerable<AreaInfo> GetAreaInfos();
        IDictionary<MethodInfo, string> GetRequestHandlerUrlInfos();

    }
    public class RequestHandlerInfo : IRequestHandlerInfo
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly PageLoader _pageLoader;
        private readonly LinkGenerator _linkGenerator;
        private readonly object _locker;
        private readonly object _locker2;

        private IEnumerable<AreaInfo> _areaInfosCache;
        private IDictionary<MethodInfo, string> _requestHandlerInfosCache;

        public RequestHandlerInfo(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, PageLoader pageLoader, LinkGenerator linkGenerator)
        {
            _locker = new object();
            _locker2 = new object();
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _pageLoader = pageLoader;
            _linkGenerator = linkGenerator;
        }

        public IEnumerable<AreaInfo> GetAreaInfos()
        {
            if (_areaInfosCache == null)
            {
                lock (_locker)
                {
                    if (_areaInfosCache == null)
                    {
                        _areaInfosCache = GetAreaInfos(_actionDescriptorCollectionProvider, _pageLoader,
                            _linkGenerator);
                    }
                }
            }

            return _areaInfosCache;
        }

        public IDictionary<MethodInfo, string> GetRequestHandlerUrlInfos()
        {
            if (_requestHandlerInfosCache == null)
            {
                lock (_locker2)
                {
                    if (_requestHandlerInfosCache == null)
                    {
                        _requestHandlerInfosCache = GetAreaInfos()
                            .SelectMany(t => t.Handlers.Select(h => new { h.MethodInfo, h.Url }))
                            .ToDictionary(o => o.MethodInfo, o => o.Url);
                    }
                }
            }
            return _requestHandlerInfosCache ;
        }

        private IEnumerable<AreaInfo> GetAreaInfos(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, IPageLoader pageLoader, LinkGenerator linkGenerator)
        {
            //根，从area开始
            var result = new List<AreaInfo>();

            //遍历区域
            foreach (var area in actionDescriptorCollectionProvider.ActionDescriptors.Items.GroupBy(item => item.RouteValues["area"]))
            {
                var areaInfo = new AreaInfo { Name = area.Key };
                result.Add(areaInfo);

                //遍历控制器
                foreach (var controllerActions in area.OfType<ControllerActionDescriptor>().GroupBy(c => c.ControllerTypeInfo))
                {
                    //跳过重复控制器，不知道为什么，里面有些重复项目
                    if (areaInfo.Controllers.Any(c => c.TypeFullName == controllerActions.Key.FullName))
                    {
                        continue;
                    }

                    var conInfo = new AreaInfo.ControllerInfo
                    {
                        Name = controllerActions.First().ControllerName,
                        TypeFullName = controllerActions.Key.FullName,
                        Description = (controllerActions.Key.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                            ?.Description,
                        Area = controllerActions.First().RouteValues["area"],
                    };
                    areaInfo.Controllers.Add(conInfo);

                    //遍历动作
                    foreach (var action in controllerActions)
                    {
                        var act = new AreaInfo.ControllerInfo.ActionInfo
                        {
                            Name = action.MethodInfo.Name,
                            ActionName = action.ActionName,
                            HandlerIdentification = (action.MethodInfo.GetCustomAttribute(typeof(RequestHandlerIdentificationAttribute)) as RequestHandlerIdentificationAttribute)
                                ?.UniqueKey,
                            Signature = action.MethodInfo.ToString(),
                            Description = (action.MethodInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                                ?.Description,
                            Url = linkGenerator.GetPathByAction(action.ActionName, conInfo.Name, new { area = conInfo.Area }),
                            HttpMethodLimits = action.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods.ToList(),
                            MethodInfo = action.MethodInfo,
                            ActionDescriptor = action
                        };
                        conInfo.Actions.Add(act);
                    }
                }

                //遍历Razor Page
                foreach (var pageActionDescriptor in area.OfType<PageActionDescriptor>())
                {
                    //载入Razor Page，会从程序集加载或编译page文件并缓存
                    var page = pageLoader.Load(pageActionDescriptor);
                    //也是剔除重复
                    if (areaInfo.Pages.Any(p => p.TypeFullName == page.HandlerTypeInfo.FullName))
                    {
                        continue;
                    }

                    var pageInfo = new AreaInfo.PageInfo
                    {
                        Name = page.DisplayName,
                        Area = page.AreaName,
                        TypeFullName = page.HandlerTypeInfo.FullName,
                        Description = (page.HandlerTypeInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                            ?.Description
                    };

                    foreach (var handlerMethod in page.HandlerMethods)
                    {
                        pageInfo.PageHandlers.Add(new AreaInfo.PageInfo.PageHandlerInfo
                        {
                            HttpMethod = handlerMethod.HttpMethod,
                            Name = handlerMethod.Name,
                            HandlerIdentification = (handlerMethod.MethodInfo.GetCustomAttribute(typeof(RequestHandlerIdentificationAttribute)) as RequestHandlerIdentificationAttribute)
                                ?.UniqueKey,
                            Signature = handlerMethod.MethodInfo.ToString(),
                            Description = (handlerMethod.MethodInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute)
                                ?.Description,
                            Url = linkGenerator.GetPathByPage(page.DisplayName, handlerMethod.Name, new { area = page.AreaName }),
                            MethodInfo = handlerMethod.MethodInfo,
                            ActionDescriptor = page
                        });
                    }
                    areaInfo.Pages.Add(pageInfo);
                }
            }

            return result;
        }
    }
}
