using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace NetPacketDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 要先安装 Npcap https://nmap.org/download.html
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Print SharpPcap version
            string ver = SharpPcap.Version.VersionString;
            Console.WriteLine("SharpPcap {0}, Example3.BasicCap.cs", ver);

            // Retrieve the device list
            var devices = CaptureDeviceList.Instance;

            // If no devices were found print an error
            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            int i = 0;

            // Print out the devices
            foreach (var dev in devices)
            {
                /* Description */
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = int.Parse(Console.ReadLine());

            var device = devices[i];

            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival +=
                new PacketArrivalEventHandler(new Program().device_OnPacketArrival);

            // Open the device for capturing
            int readTimeoutMilliseconds = 1000;
            if (device is NpcapDevice)
            {
                var nPcap = device as NpcapDevice;
                nPcap.Open(OpenFlags.DataTransferUdp | OpenFlags.NoCaptureLocal, readTimeoutMilliseconds);
            }
            else if (device is LibPcapLiveDevice)
            {
                var livePcapDevice = device as LibPcapLiveDevice;
                livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            }
            else
            {
                throw new InvalidOperationException("unknown device type of " + device.GetType().ToString());
            }

            //(device as NpcapDevice).KernelBufferSize = 104857600;
            device.Filter = "ip and tcp";

            Console.WriteLine();
            Console.WriteLine("-- Listening on {0} {1}, hit 'Enter' to stop...",
                device.Name, device.Description);

            // Start the capturing process
            device.StartCapture();

            // Wait for 'Enter' from the user.
            Console.ReadLine();

            // Stop the capturing process
            device.StopCapture();

            Console.WriteLine("-- Capture stopped.");

            // Print out the device statistics
            Console.WriteLine(device.Statistics.ToString());

            // Close the pcap device
            device.Close();
        }

        private int packetId = 0;
        private int packTimeMax = 1000;
        private Dictionary<uint, int[]> packTimeRest = new Dictionary<uint, int[]>();
        private Dictionary<uint, List<PacketWrapper>> packDic = new Dictionary<uint, List<PacketWrapper>>();
        /// <summary>
        /// Prints the time and length of each received packet
        /// </summary>
        private void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            // 抓取 http 包并在控制台显示，https 有加密不行
            PacketWrapper packetWrapper = new PacketWrapper(packetId++, e.Packet);

            uint ackn = packetWrapper.tcpPacket.AcknowledgmentNumber;
            if (!packDic.ContainsKey(ackn))
            {
                packDic.Add(ackn, new List<PacketWrapper>());
                packTimeRest.Add(ackn, new int[] { packTimeMax });
            }
            packDic[ackn].Add(packetWrapper);

            List<uint> nd = new List<uint>();
            foreach (var p in packTimeRest)
            {
                packTimeRest[p.Key][0]--;
                if (packTimeRest[p.Key][0] <= 0)
                {
                    nd.Add(p.Key);
                }
            }

            foreach (var p in nd)
            {
                packTimeRest.Remove(p);
                packDic.Remove(p);
            }

            if (!packetWrapper.tcpPacket.Push) return;

            byte[] data = CombinePacket(packDic, ackn);

            if (data == null) return;

            string s = Encoding.GetEncoding("GB18030").GetString(data);

            int index = s.IndexOf("\r\n\r\n");
            if (index < 0) return;
            index += 4;

            string header = s.Substring(0, index);

            if (!HttpCheck(header)) return;

            Console.WriteLine(header);
            Console.WriteLine("datalen" + data.Length);

            //string body = s.Substring(index);
            //Console.WriteLine(body);

            byte[] bodyBytes = null;

            for (int i = 0; i < data.Length - 1; i++)
            {
                if (data[i] == 13 && data[i + 2] == 13 && data[i + 1] == 10 && data[i + 3] == 10)
                {
                    i += 4;
                    bodyBytes = new byte[data.Length - i];
                    for (int t = 0; t < bodyBytes.Length; t++, i++)
                    {
                        bodyBytes[t] = data[i];
                    }
                    break;
                }
            }

            if (bodyBytes == null) return;

            byte[] bodyUncompressedBytes = Decompress(bodyBytes);

            if (bodyUncompressedBytes == null) return;
            string json = Encoding.GetEncoding("GB18030").GetString(bodyUncompressedBytes);

            Console.WriteLine(json);
        }

        private static byte[] CombinePacket(Dictionary<uint, List<PacketWrapper>> packDic, uint acknowledgmentNumber)
        {
            try
            {
                List<PacketWrapper> pwl = new List<PacketWrapper>(packDic[acknowledgmentNumber].ToArray());
                pwl.Sort(PackSort);
                PackDereplication(pwl);
                if (PackCheck(pwl))
                {
                    List<byte> data = new List<byte>();
                    for (int i = 0; i < pwl.Count; i++)
                    {
                        data.AddRange(pwl[i].tcpPacket.PayloadData);
                    }
                    if (data.Count < 64)
                    {
                        return null;
                    }
                    return data.ToArray();
                }
            }
            catch { }
            return null;
        }

        private static bool PackCheck(List<PacketWrapper> pwl)
        {
            for (int i = 0; i < pwl.Count - 1; i++)
            {
                var tp1 = pwl[i].tcpPacket;
                var tp2 = pwl[i + 1].tcpPacket;

                if (tp2.SequenceNumber != tp1.SequenceNumber + tp1.PayloadData.Length)
                {
                    return false;
                }
            }
            return true;
        }

        private static void PackDereplication(List<PacketWrapper> pwl)
        {
            if (pwl.Count < 2) return;
            for (int i = pwl.Count - 2; i >= 0; i--)
            {
                var tp1 = pwl[i].tcpPacket;
                var tp2 = pwl[i + 1].tcpPacket;

                if (tp1.SequenceNumber == tp2.SequenceNumber)
                {
                    if (tp1.PayloadData.Length >= tp2.PayloadData.Length)
                    {
                        pwl.RemoveAt(i + 1);
                    }
                    else
                    {
                        pwl.RemoveAt(i);
                    }
                }
            }
        }

        private static int PackSort(PacketWrapper x, PacketWrapper y)
        {
            if (x.tcpPacket.SequenceNumber > y.tcpPacket.SequenceNumber)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private static bool HttpCheck(string s)
        {
            return s.IndexOf("HTTP") == 0 || s.IndexOf("GET") == 0 || s.IndexOf("POST") == 0;
        }

        /// <summary>
        /// GZIP解压
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
                MemoryStream outBuffer = new MemoryStream();
                byte[] block = new byte[1024];
                while (true)
                {
                    int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                        break;
                    else
                        outBuffer.Write(block, 0, bytesRead);
                }
                compressedzipStream.Close();
                return outBuffer.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("解压失败,数据 " + data.Length + " 字节");
            }
            return null;
        }
    }

    public class PacketWrapper
    {
        public int id;
        public TcpPacket tcpPacket;
        public IPPacket ipPacket;
        public PacketWrapper(int id, RawCapture p)
        {
            this.id = id;
            Packet pac = Packet.ParsePacket(p.LinkLayerType, p.Data);
            tcpPacket = pac.Extract<TcpPacket>();
            ipPacket = pac.Extract<IPPacket>();
        }
    }
}
