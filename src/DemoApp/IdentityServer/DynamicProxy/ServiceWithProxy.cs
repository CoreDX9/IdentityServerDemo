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

            logger.LogInformation($"{invocation.Method.Name} 已执行");
        }
    }

    public interface IServiceWithProxy
    {
        void ProxyMethod();
    }

    public class ServiceWithProxy : IServiceWithProxy
    {
        public virtual void ProxyMethod() { }
    }
}
