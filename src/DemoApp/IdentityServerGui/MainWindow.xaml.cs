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

namespace IdentityServerGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IHost Host { get; set; }
        private Task HostTask { get; set; }

        public class Data : INotifyPropertyChanged
        {
            public AppSettings Settings { get; set; }
            public bool CanStartHost { get; set; }
            public bool CanStopHost { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public Data MyData { get; }

        private void UpdateHostState()
        {
            MyData.CanStartHost = HostTask == null;
            MyData.CanStopHost = HostTask != null;
        }

        private async Task StopHostAsync()
        {
            using (Host)
            using (var stopTask = Host.StopAsync())
            {
                await stopTask;
                HostTask = null;
            }
            UpdateHostState();
        }

        public MainWindow(IOptions<AppSettings> settings)
        {
            InitializeComponent();
            txbHostState.Text = "网站未运行";

            Console.SetOut(new TextBoxWriter(this, txtConsoleOut));

            MyData = new Data()
            {
                Settings = settings.Value,
                CanStartHost = true,
                CanStopHost = false
            };
            MyMainWindow.DataContext = MyData;
        }

        private async void btnStartWebHost_ClickAsync(object sender, RoutedEventArgs e)
        {
            MyData.CanStartHost = false;
            txbHostState.Text = "正在启动网站";
            txtConsoleOut.Clear();

            await Task.Run(async () => {
                Host = (Application.Current as App).ServiceProvider.GetRequiredService<IHost>();

                try
                {
                    await SeedData.EnsureSeedDataAsync(Host.Services);
                    HostTask = Host.StartAsync();
                    await HostTask;
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        txbHostState.Text = "网站运行中";
                    }, System.Windows.Threading.DispatcherPriority.Normal);
                }
                catch(Exception e)
                {
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        txbHostState.Text = "正在停止网站";
                    }, System.Windows.Threading.DispatcherPriority.Normal);
                    HostTask?.Dispose();
                    HostTask = null;
                    _ = Dispatcher.InvokeAsync(() =>
                    {
                        txbHostState.Text = "网站未运行（启动失败）";
                    }, System.Windows.Threading.DispatcherPriority.Normal);
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
            txbHostState.Text = "正在停止网站";
            MyData.CanStopHost = false;
            await StopHostAsync();
            Host?.Dispose();
            Host = null;
            txbHostState.Text = "网站未运行（已停止）";
        }

        private async void MyMainWindow_ClosingAsync(object sender, CancelEventArgs e)
        {
            txbHostState.Text = "正在停止网站";
            if (MyData.CanStopHost)
            {
                await StopHostAsync();
                txbHostState.Text = "网站未运行（已停止）";
            }
        }

        private void MyMainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
