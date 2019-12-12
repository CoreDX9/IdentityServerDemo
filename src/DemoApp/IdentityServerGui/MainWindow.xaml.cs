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

namespace IdentityServerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IHost Host { get; set; }
        //private Task HostTask { get; set; }

        public class Data : INotifyPropertyChanged
        {
            public AppSettings Settings { get; set; }
            public bool CanStartHost { get; set; }
            public bool CanStopHost { get; set; }
            public string HostState { get; set; }

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

        public MainWindow(IOptions<AppSettings> settings)
        {
            InitializeComponent();

            Console.SetOut(new TextBoxWriter(this, txtConsoleOut));

            MyData = new Data()
            {
                Settings = settings.Value,
                CanStartHost = true,
                CanStopHost = false,
                HostState = "网站未运行"
            };
            winMainWindow.DataContext = MyData;

            tbiNotify.DoubleClickCommand = new ShowMainWindowCommand(this, tbiNotify);
            tbiNotify.DoubleClickCommandParameter = null;
        }

        private async void btnStartWebHost_ClickAsync(object sender, RoutedEventArgs e)
        {
            MyData.CanStartHost = false;
            MyData.HostState = "正在启动网站";
            txtConsoleOut.Clear();

            await Task.Run(async () => {
                var host = (Application.Current as App).ServiceProvider.GetRequiredService<IHost>();

                try
                {
                    await SeedData.EnsureSeedDataAsync(host.Services);
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
            Application.Current.Shutdown(0);
        }
    }
}
