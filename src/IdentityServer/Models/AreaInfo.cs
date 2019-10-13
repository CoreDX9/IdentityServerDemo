using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Newtonsoft.Json;

namespace IdentityServer.Models
{
    /// <summary>
    /// Area、控制器和Razor Page信息类
    /// </summary>
    public class AreaInfo
    {
        public string Name { get; set; }
        public List<ControllerInfo> Controllers { get; set; } = new List<ControllerInfo>();
        public List<PageInfo> Pages { get; set; } = new List<PageInfo>();

        [JsonIgnore]
        public List<HandlerTypeInfo> HandlerTypes =>
            Controllers.Cast<HandlerTypeInfo>().Concat(Pages.Cast<HandlerTypeInfo>()).ToList();
        [JsonIgnore]
        public List<HandlerInfo> Handlers => HandlerTypes.SelectMany(t => t.Handlers).ToList();

        public abstract class HandlerTypeInfo
        {
            public string Area { get; set; }
            public string Name { get; set; }
            public string TypeFullName { get; set; }
            public string Description { get; set; }
            [JsonIgnore]
            public abstract IEnumerable<HandlerInfo> Handlers { get; }
        }

        public abstract class HandlerInfo
        {
            public string Name { get; set; }
            public string Signature { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string HandlerIdentification { get; set; }
            [JsonIgnore]
            public ActionDescriptor ActionDescriptor { get; set; }
            [JsonIgnore]
            public MethodInfo MethodInfo { get; set; }
            [JsonIgnore]
            public abstract IEnumerable<string> SupportedHttpMethods { get; }
        }

        public class ControllerInfo : HandlerTypeInfo
        {
            public List<ActionInfo> Actions { get; set; } = new List<ActionInfo>();
            [JsonIgnore]
            public override IEnumerable<HandlerInfo> Handlers => Actions;

            public class ActionInfo : HandlerInfo
            {
                public string ActionName { get; set; }
                public List<string> HttpMethodLimits { get; set; } = new List<string>();

                public override IEnumerable<string> SupportedHttpMethods => HttpMethodLimits;
            }
        }

        public class PageInfo : HandlerTypeInfo
        {
            public List<PageHandlerInfo> PageHandlers { get; set; } = new List<PageHandlerInfo>();
            [JsonIgnore]
            public override IEnumerable<HandlerInfo> Handlers => PageHandlers;

            public class PageHandlerInfo : HandlerInfo
            {
                public string HttpMethod { get; set; }
                public override IEnumerable<string> SupportedHttpMethods => new[] {HttpMethod};
            }
        }
    }
}
