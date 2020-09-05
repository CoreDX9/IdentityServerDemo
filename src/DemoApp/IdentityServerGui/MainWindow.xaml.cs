using IdentityServer;
using IdentityServerGui.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace IdentityServerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        private static int ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
            return 0;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ClearMemory();
        }

        #endregion

        private IHost Host { get; set; }
        private PerformanceMonitor PerformanceMonitor { get; set; }
        private TextBoxWriter TextBoxWriter { get; }
        public bool IsHostRunning => Host != null;

        public class Data : INotifyPropertyChanged
        {
            public AppSettings Settings { get; set; }
            public bool CanStartHost { get; set; }
            public bool CanStopHost { get; set; }
            public string HostState { get; set; }
            public float CpuCounter { get; set; }
            public float RamCounter { get; set; }
            public float NetSend { get; set; }
            public float NetRecv { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public Data MyData { get; }

        private void UpdateHostState()
        {
            MyData.CanStartHost = Host == null;
            MyData.CanStopHost = Host != null;
        }

        private async Task TryStopHostAsync()
        {
            try
            {
                using var _ = Host;
                using var stopTask = Host?.StopAsync();

                if (stopTask != null)
                {
                    await stopTask.ConfigureAwait(false); ;
                }
            }
            catch (OperationCanceledException e)
            {
                //SignalR有连接的时候停止网站会报这个错，不知道为什么，不管。。。
            }
            finally
            {
                Host = null;
                UpdateHostState();
            }

        }

        public async void StopHostAndClose()
        {
            await TryStopHostAsync().ConfigureAwait(false);
            Close();
        }

        public MainWindow(IOptions<AppSettings> settings, PerformanceMonitor performanceMonitor)
        {
            InitializeComponent();

            PerformanceMonitor = performanceMonitor;

            TextBoxWriter = new TextBoxWriter(this, txtConsoleOut);
            Console.SetOut(TextBoxWriter);

            MyData = new Data()
            {
                Settings = settings.Value,
                CanStartHost = true,
                CanStopHost = false,
                HostState = "网站未运行"
            };
            winMainWindow.DataContext = MyData;
            PerformanceMonitor?.Start(MyData);

            tbiNotify.DoubleClickCommand = new ShowMainWindowCommand(this, tbiNotify);
            tbiNotify.DoubleClickCommandParameter = null;
        }

        private async void btnStartWebHost_ClickAsync(object sender, RoutedEventArgs e)
        {
            MyData.CanStartHost = false;
            MyData.HostState = "正在启动网站";
            txtConsoleOut.Clear();

            await Task.Run(async () =>
            {
                IHost host = null;

                try
                {
                    host = (Application.Current as App).ScopedService.GetRequiredService<IHost>();
                    await Program.EnsureSeedDataAsync(host).ConfigureAwait(false); //初始化数据库
                    await host.StartAsync().ConfigureAwait(false);
                    Host = host;
                    MyData.HostState = "网站运行中";
                }
                catch (Exception e)
                {
                    MyData.HostState = "正在停止网站";
                    host?.Dispose();
                    await TryStopHostAsync().ConfigureAwait(false);
                    MyData.HostState = "网站未运行（启动失败）";
                    MessageBox.Show("启动网站时发生错误：" + e.Message);

                    ClearMemory();
                }
                finally
                {
                    UpdateHostState();
                }
            }).ConfigureAwait(false);
        }

        private async void btnStopWebHost_ClickAsync(object sender, RoutedEventArgs e)
        {
            MyData.HostState = "正在停止网站";
            MyData.CanStopHost = false;
            await TryStopHostAsync().ConfigureAwait(false);
            MyData.HostState = "网站未运行（已停止）";

            UpdateHostState();

            ClearMemory();
        }

        private void MyMainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (MyData.CanStopHost || !MyData.CanStartHost)
            {
                e.Cancel = true;

                Hide();
                tbiNotify.Visibility = Visibility.Visible;
                tbiNotify.ShowBalloonTip(MyData.Settings.AppName, $"{MyData.HostState}。如要关闭应用请先停止网站。双击通知图标显示主窗口。", tbiNotify.Icon, true);
            }
        }

        private async void MyMainWindow_Closed(object sender, EventArgs e)
        {
            await TryStopHostAsync().ConfigureAwait(false);
            var v = TextBoxWriter?.DisposeAsync();
            if (v.HasValue) await v.Value.ConfigureAwait(false);
            PerformanceMonitor.Stop();
            Application.Current.Shutdown(0);
        }
    }
}
