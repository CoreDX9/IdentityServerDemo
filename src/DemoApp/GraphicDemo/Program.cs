using System;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace GraphicDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("按任意键开始DX截图……");
            Console.ReadKey();

            string path = @"E:\截图测试";

            var cancel = new CancellationTokenSource();
            await Task.Run(() =>
            {
                Task.Run(() =>
                {
                    Thread.Sleep(5000);
                    cancel.Cancel();
                    Console.WriteLine("DX截图结束！");
                });
                var savePath = $@"{path}\DX";
                Directory.CreateDirectory(savePath);

                using var dx = new DirectXScreenCapturer();
                Console.WriteLine("开始DX截图……");
                
                while (!cancel.IsCancellationRequested)
                {
                    var (result, isBlackFrame, image) = dx.GetFrameImage();
                    if (result.Success && !isBlackFrame) image.Save($@"{savePath}\{DateTime.Now.Ticks}.jpg", ImageFormat.Jpeg);
                    image?.Dispose();
                }
            }, cancel.Token);

            var windows = WindowEnumerator.FindAll();
            for (int i = 0; i < windows.Count; i++)
            {
                var window = windows[i];
                Console.WriteLine($@"{i.ToString().PadLeft(3, ' ')}. {window.Title}
            {window.Bounds.X}, {window.Bounds.Y}, {window.Bounds.Width}, {window.Bounds.Height}");
            }

            var savePath = $@"{path}\Gdi";
            Directory.CreateDirectory(savePath);
            Console.WriteLine("开始Gdi窗口截图……");

            foreach (var win in windows)
            {
                var image = CaptureWindow.ByHwnd(win.Hwnd);
                image.Save($@"{savePath}\{win.Title.Substring(win.Title.LastIndexOf(@"\") < 0 ? 0 : win.Title.LastIndexOf(@"\") + 1).Replace("/", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace(":", "").Replace("<", "").Replace(">", "").Replace("|", "")}.jpg", ImageFormat.Jpeg);
                image.Dispose();
            }
            Console.WriteLine("Gdi窗口截图结束！");

            Console.ReadKey();
        }
    }
}
