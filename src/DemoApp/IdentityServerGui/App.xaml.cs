using IdentityServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

#if !DEBUG
using System.Diagnostics;
#endif

namespace IdentityServerGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            //通过特殊手段运行应用可能导致工作目录与程序文件所在目录不一致，需要调整，否则配置文件和其他数据无法加载（仅限发布模式，调试模式修改工作目录也可能导致配置和其他数据无法加载）
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            Directory.SetCurrentDirectory(pathToContentRoot);
#endif

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("guiAppsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            this.MainWindow = mainWindow;
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddTransient<MainWindow>();
            services.AddTransient<PerformanceMonitor>();
            services.AddTransient(service =>
            {
                var host = Program.CreateHostBuilderP(Environment.GetCommandLineArgs())
#if DEBUG
                .UseEnvironment("Staging")
#endif
                .Build();
                return host;
            });
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("捕获到未处理异常：" + e.Exception.Message);
            e.Handled = true;
            Shutdown(-1);
        }

        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            string msg = string.Format("{0}. End session?", e.ReasonSessionEnding);
            MessageBoxResult result = MessageBox.Show(msg, "Session Ending", MessageBoxButton.YesNo);

            // End session, if specified
            if (result == MessageBoxResult.Yes)
            {
                (this.MainWindow as MainWindow).StopHostAndClose();
            }
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
