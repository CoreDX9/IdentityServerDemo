using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace IdentityServer.DynamicProxy
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger<LoggingInterceptor> logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            this.logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            //调用业务方法
            invocation.Proceed();

            logger.LogTrace($"{invocation.Method.Name} 已执行");
            invocation.ReturnValue = $"{invocation.ReturnValue}\r\n动态代理拦截器已执行，拦截到 {invocation.Method.Name} 方法。";
        }
    }

    public interface IServiceWithProxy
    {
        string ProxyMethod();
    }

    public class ServiceWithProxy : IServiceWithProxy
    {
        public virtual string ProxyMethod() => "代理服务已执行";
    }
}
