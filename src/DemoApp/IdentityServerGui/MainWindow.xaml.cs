using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IdentityServer;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using IdentityServerGui.Commands;
using System.Runtime.InteropServices;

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
        private static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            ClearMemory();
        }

        #endregion

        private IHost Host { get; set; }
        private PerformanceMonitor PerformanceMonitor { get; set; }

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

                if(stopTask != null)
                {
                    await stopTask;
                }
            }
            catch(OperationCanceledException e)
            {
                //SignalR有连接的时候停止网站会报这个错，不知道为什么，不管。。。
            }
            finally
            {
                Host = null;
                UpdateHostState();
            }

        }

        public MainWindow(IOptions<AppSettings> settings, PerformanceMonitor performanceMonitor)
        {
            InitializeComponent();

            PerformanceMonitor = performanceMonitor;

            Console.SetOut(new TextBoxWriter(this, txtConsoleOut));

            MyData = new Data()
            {
                Settings = settings.Value,
                CanStartHost = true,
                CanStopHost = false,
                HostState = "网站未运行"
            };
            winMainWindow.DataContext = MyData;
            PerformanceMonitor.Start(MyData);

            tbiNotify.DoubleClickCommand = new ShowMainWindowCommand(this, tbiNotify);
            tbiNotify.DoubleClickCommandParameter = null;
        }

        private async void btnStartWebHost_ClickAsync(object sender, RoutedEventArgs e)
        {
            MyData.CanStartHost = false;
            MyData.HostState = "正在启动网站";
            txtConsoleOut.Clear();

            await Task.Run(async () => {
                IHost host = null;

                try
                {
                    host = (Application.Current as App).ServiceProvider.GetRequiredService<IHost>();
                    await Program.EnsureSeedDataAsync(host); //初始化数据库
                    await host.StartAsync();
                    Host = host;
                    MyData.HostState = "网站运行中";
                }
                catch(Exception e)
                {
                    MyData.HostState = "正在停止网站";
                    host?.Dispose();
                    await TryStopHostAsync();
                    MyData.HostState = "网站未运行（启动失败）";
                    MessageBox.Show("启动网站时发生错误：" + e.Message);

                    ClearMemory();
                }
                finally
                {
                    UpdateHostState();
                }
            });
        }

        private async void btnStopWebHost_ClickAsync(object sender, RoutedEventArgs e)
        {
            MyData.HostState = "正在停止网站";
            MyData.CanStopHost = false;
            await TryStopHostAsync();
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

        private void MyMainWindow_Closed(object sender, EventArgs e)
        {
            PerformanceMonitor.Stop();
            Application.Current.Shutdown(0);
        }
    }
}
