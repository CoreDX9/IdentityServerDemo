using System;
using System.Reflection;
using System.Runtime.Loader;

namespace IdentityServer.Extensions
{
    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        //程序集依赖解析器
        private AssemblyDependencyResolver _resolver;

        public CollectibleAssemblyLoadContext(string mainAssemblyToLoadPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(mainAssemblyToLoadPath);
        }

        protected override Assembly Load(AssemblyName name)
        {
            //此依赖解析器解析程序集路径时依赖 .deps.json 文件，此文件会在生成项目时由编译器自动生成
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                //这种载入方法会导致dll被锁定，如果需要实现热更新，可以使用流加载。在更新dll后卸载整个AssemblyLoadContext，新建Context加载新的dll
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
