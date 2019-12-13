using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using static IdentityServerGui.MainWindow;

namespace IdentityServerGui
{
    public class PerformanceMonitor
    {
        private Process process;
        private CancellationTokenSource tokenSource;
        private Task task;

        public PerformanceMonitor()
        {
            process = Process.GetCurrentProcess();
        }

        public void Start(Data data)
        {

            tokenSource = new CancellationTokenSource();
            task = Task.Run(() => {
                var name = this.process.ProcessName;
                var cpuCounter = new PerformanceCounter("Process", "% Processor Time", name, true);
                var ramCounter = new PerformanceCounter("Process", "Working Set - Private", name, true);

                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                var netRecvCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", adapters[0].Description, true);
                var netSendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", adapters[0].Description, true);

                while (!tokenSource.Token.IsCancellationRequested)
                {
                    data.CpuCounter = cpuCounter.NextValue() / Environment.ProcessorCount;
                    data.RamCounter = ramCounter.NextValue() / 1024 / 1024;
                    data.NetRecv = netRecvCounter.NextValue() / 1024 * 8;
                    data.NetSend = netSendCounter.NextValue() / 1024 * 8;

                    Thread.Sleep(1000);
                }

                using(cpuCounter)
                using (ramCounter)
                using (netRecvCounter)
                using (netSendCounter)
                {
                    cpuCounter.Close();
                    ramCounter.Close();
                    netRecvCounter.Close();
                    netSendCounter.Close();
                }
            }, tokenSource.Token);
        }

        public void Stop()
        {
            using(task)
            using (tokenSource)
            {
                tokenSource.Cancel();
            }
            task = null;
            tokenSource = null;
        }
    }
}
