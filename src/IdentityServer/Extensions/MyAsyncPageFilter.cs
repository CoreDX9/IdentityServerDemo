using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Extensions
{
    public class MyAsyncPageFilter : IAsyncPageFilter
    {
        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            //可以在这里模拟Action级别的Authorize授权，Razor Pages的Authorize授权只支持到PageModel级别
            //context.HandlerMethod.MethodInfo
            //context.Result

            var executedContext = await next.Invoke();

            //从这里开始相当于在IPageFilter的OnPageHandlerExecuted方法中执行代码
            //executedContext就是执行结果上下文

            await Task.CompletedTask;
        }
    }
}
