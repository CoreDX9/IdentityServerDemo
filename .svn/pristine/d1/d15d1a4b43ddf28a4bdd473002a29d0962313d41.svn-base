using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public class ControllerInfo
        {
            public string Name { get; set; }
            public string TypeFullName { get; set; }
            public string Desc { get; set; }
            public string Area { get; set; }
            public List<ActionInfo> Actions { get; set; } = new List<ActionInfo>();

            public class ActionInfo
            {
                public string Name { get; set; }
                public string ActionName { get; set; }
                public string SignName { get; set; }
                public string Desc { get; set; }
                public string Url { get; set; }
                public List<string> HttpMethodLimits { get; set; } = new List<string>();
            }
        }

        public class PageInfo
        {
            public string Area { get; set; }
            public string Name { get; set; }
            public string HandlerTypeFullName { get; set; }
            public List<PageHandlerInfo> PageHandlers { get; set; } = new List<PageHandlerInfo>();

            public class PageHandlerInfo
            {
                public string Name { get; set; }
                public string SignName { get; set; }
                public string Url { get; set; }
                public string HttpMethod { get; set; }
            }
        }
    }
}
