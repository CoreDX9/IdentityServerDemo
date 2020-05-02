using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace CoreDX.DependencyInjection.DynamicProxyExtensions
{
    public interface IProxyService<out TService> where TService : class
    {
        TService Proxy { get; }
    }

    internal sealed class ProxyService<TService> : IProxyService<TService> where TService : class
    {
        public ProxyService(TService service)
        {
            Proxy = service;
        }

        public TService Proxy { get; }
    }

    public static class CastleDynamicProxyDependencyInjectionExtensions
    {
        public static IServiceCollection AddProxySingleton(this IServiceCollection services, Type serviceType, Type ImplementationType, params Type[] interceptorTypes)
        {
            services.TryAddSingleton<ProxyGenerator>();

            foreach (var interceptorType in interceptorTypes)
            {
                services.TryAdd(new ServiceDescriptor(interceptorType, interceptorType, ServiceLifetime.Singleton));
            }

            services.TryAdd(new ServiceDescriptor(serviceType, ImplementationType, ServiceLifetime.Singleton));

            Func<IServiceProvider, object> factory = provider =>
            {
                var target = provider.GetRequiredService(serviceType);
                var interceptors = interceptorTypes.Select(t => provider.GetRequiredService(t) as IInterceptor).ToArray();
                var proxy = provider.GetRequiredService<ProxyGenerator>().CreateClassProxyWithTarget(ImplementationType, target, interceptors);

                return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(serviceType), proxy);
            };

            services.Add(new ServiceDescriptor(typeof(IProxyService<>).MakeGenericType(serviceType), factory, ServiceLifetime.Singleton));

            return services;
        }

        public static IServiceCollection AddProxySingleton<TService, TImplementation>(this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddProxySingleton(typeof(TService), typeof(TImplementation), interceptorTypes);
        }

        public static IServiceCollection AddProxySingleton(this IServiceCollection services, Type serviceType, object instance, params Type[] interceptorTypes)
        {
            services.TryAddSingleton<ProxyGenerator>();

            foreach (var interceptorType in interceptorTypes)
            {
                services.TryAdd(new ServiceDescriptor(interceptorType, interceptorType, ServiceLifetime.Singleton));
            }

            services.TryAdd(new ServiceDescriptor(serviceType, instance));

            Func<IServiceProvider, object> factory = provider =>
            {
                var target = provider.GetRequiredService(serviceType);
                var interceptors = interceptorTypes.Select(t => provider.GetRequiredService(t) as IInterceptor).ToArray();
                var proxy = provider.GetRequiredService<ProxyGenerator>().CreateClassProxyWithTarget(serviceType, target, interceptors);

                return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(serviceType), proxy);
            };

            services.Add(new ServiceDescriptor(typeof(IProxyService<>).MakeGenericType(serviceType), factory, ServiceLifetime.Singleton));

            return services;
        }

        public static IServiceCollection AddProxySingleton<TService>(this IServiceCollection services, TService instance, params Type[] interceptorTypes)
            where TService : class
        {
            return services.AddProxySingleton(typeof(TService), instance, interceptorTypes);
        }

        public static IServiceCollection AddProxyService(this IServiceCollection services, Type serviceType, Type ImplementationType, ServiceLifetime serviceLifetime, params Type[] interceptorTypes)
        {
            services.TryAddSingleton<ProxyGenerator>();

            foreach (var interceptorType in interceptorTypes)
            {
                services.TryAdd(new ServiceDescriptor(interceptorType, interceptorType, serviceLifetime));
            }

            services.TryAdd(new ServiceDescriptor(serviceType, ImplementationType, serviceLifetime));

            Func<IServiceProvider, object> factory = provider =>
            {
                var target = provider.GetRequiredService(serviceType);
                var interceptors = interceptorTypes.Select(t => provider.GetRequiredService(t) as IInterceptor).ToArray();
                var proxy = provider.GetRequiredService<ProxyGenerator>().CreateClassProxyWithTarget(ImplementationType, target, interceptors);

                return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(serviceType), proxy);
            };

            services.Add(new ServiceDescriptor(typeof(IProxyService<>).MakeGenericType(serviceType), factory, serviceLifetime));

            return services;
        }

        public static IServiceCollection AddProxyService<TService, TImplementation>(this IServiceCollection services, ServiceLifetime serviceLifetime, params Type[] interceptorTypes)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddProxyService(typeof(TService), typeof(TImplementation), serviceLifetime, interceptorTypes);
        }

        public static IServiceCollection AddProxyTransient<TService, TImplementation>(this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddProxyService(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient, interceptorTypes);
        }

        public static IServiceCollection AddProxyScoped<TService, TImplementation>(this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
            where TImplementation : class, TService
        {
            return services.AddProxyService(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped, interceptorTypes);
        }

        public static IServiceCollection AddProxyService(this IServiceCollection services, Type serviceType, ServiceLifetime serviceLifetime, params Type[] interceptorTypes)
        {
            services.TryAddSingleton<ProxyGenerator>();

            foreach (var interceptorType in interceptorTypes)
            {
                services.TryAdd(new ServiceDescriptor(interceptorType, interceptorType, serviceLifetime));
            }

            services.TryAdd(new ServiceDescriptor(serviceType, serviceType, serviceLifetime));

            Func<IServiceProvider, object> factory = provider =>
            {
                var target = provider.GetRequiredService(serviceType);
                var interceptors = interceptorTypes.Select(t => provider.GetRequiredService(t) as IInterceptor).ToArray();
                var proxy = provider.GetRequiredService<ProxyGenerator>().CreateClassProxyWithTarget(serviceType, target, interceptors);

                return Activator.CreateInstance(typeof(ProxyService<>).MakeGenericType(serviceType), proxy);
            };

            services.Add(new ServiceDescriptor(typeof(IProxyService<>).MakeGenericType(serviceType), factory, serviceLifetime));

            return services;
        }

        public static IServiceCollection AddProxyService<TService>(this IServiceCollection services, ServiceLifetime serviceLifetime, params Type[] interceptorTypes)
            where TService : class
        {
            return services.AddProxyService(typeof(TService), serviceLifetime, interceptorTypes);
        }

        public static IServiceCollection AddProxyTransient<TService>(this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
        {
            return services.AddProxyService(typeof(TService), ServiceLifetime.Transient, interceptorTypes);
        }

        public static IServiceCollection AddProxyScoped<TService>(this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
        {
            return services.AddProxyService(typeof(TService), ServiceLifetime.Scoped, interceptorTypes);
        }

        public static IServiceCollection AddProxySingleton<TService>(this IServiceCollection services, params Type[] interceptorTypes)
            where TService : class
        {
            return services.AddProxyService(typeof(TService), ServiceLifetime.Singleton, interceptorTypes);
        }
    }
}
