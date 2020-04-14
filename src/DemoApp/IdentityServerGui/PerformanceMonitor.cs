using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using static IdentityServerGui.MainWindow;

namespace IdentityServerGui
{
    public class PerformanceMonitor
    {
        private Process process;
        private readonly IConfiguration _configuration;
        private CancellationTokenSource tokenSource;
        private Task task;

        public PerformanceMonitor(IConfiguration configuration)
        {
            process = Process.GetCurrentProcess();
            _configuration = configuration;
        }

        public void Start(Data data)
        {

            tokenSource = new CancellationTokenSource();
            task = Task.Run(() =>
            {
                var name = this.process.ProcessName;
                var cpuCounter = new PerformanceCounter("Process", "% Processor Time", name, true);
                var ramCounter = new PerformanceCounter("Process", "Working Set - Private", name, true);

                NetworkInterface adapter = NetworkInterface.GetAllNetworkInterfaces().Single(x => x.Name == _configuration.GetValue<string>("MonitorNetworkName"));
                var netRecvCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", adapter.Description, true);
                var netSendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", adapter.Description, true);

                while (!tokenSource.Token.IsCancellationRequested)
                {
                    data.CpuCounter = cpuCounter.NextValue() / Environment.ProcessorCount;
                    data.RamCounter = ramCounter.NextValue() / 1024 / 1024;
                    data.NetRecv = netRecvCounter.NextValue() / 1024 * 8;
                    data.NetSend = netSendCounter.NextValue() / 1024 * 8;

                    Thread.Sleep(1000);
                }

                using (cpuCounter)
                using (ramCounter)
                using (netRecvCounter)
                using (netSendCounter)
                {
                    cpuCounter.Close();
                    ramCounter.Close();
                    netRecvCounter.Close();
                    netSendCounter.Close();
                }
            });
        }

        public async void Stop()
        {
            using (task)
            using (tokenSource)
            {
                tokenSource.Cancel();
                await task;
            }
            task = null;
            tokenSource = null;
        }
    }
}
