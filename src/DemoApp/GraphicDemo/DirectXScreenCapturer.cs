using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GraphicDemo
{
    public class DirectXScreenCapturer : IDisposable
    {
        private Factory1 factory;
        private Adapter1 adapter;
        private SharpDX.Direct3D11.Device device;
        private Output output;
        private Output1 output1;
        private Texture2DDescription textureDesc;
        //2D 纹理，存储截屏数据
        private Texture2D screenTexture;

        public DirectXScreenCapturer()
        {
            // 获取输出设备（显卡、显示器），这里是主显卡和主显示器
            factory = new Factory1();
            adapter = factory.GetAdapter1(0);
            device = new SharpDX.Direct3D11.Device(adapter);
            output = adapter.GetOutput(0);
            output1 = output.QueryInterface<Output1>();

            //设置纹理信息，供后续使用（截图大小和质量）
            textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = output.Description.DesktopBounds.Right,
                Height = output.Description.DesktopBounds.Bottom,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            screenTexture = new Texture2D(device, textureDesc);
        }

        public Result ProcessFrame(Action<DataBox, Texture2DDescription> processAction, int timeoutInMilliseconds = 5)
        {
            //截屏，可能失败
            using OutputDuplication duplicatedOutput = output1.DuplicateOutput(device);
            var result = duplicatedOutput.TryAcquireNextFrame(timeoutInMilliseconds, out OutputDuplicateFrameInformation duplicateFrameInformation, out SharpDX.DXGI.Resource screenResource);

            if (!result.Success) return result;

            using Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>();

            //复制数据
            device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);
            DataBox mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

            processAction?.Invoke(mapSource, textureDesc);

            //释放资源
            device.ImmediateContext.UnmapSubresource(screenTexture, 0);
            screenResource.Dispose();
            duplicatedOutput.ReleaseFrame();

            return result;
        }

        public (Result result, bool isBlackFrame, Image image) GetFrameImage(int timeoutInMilliseconds = 5)
        {
            //生成 C# 用图像
            Bitmap image = new Bitmap(textureDesc.Width, textureDesc.Height, PixelFormat.Format24bppRgb);
            bool isBlack = true;
            var result = ProcessFrame(ProcessImage);

            if (!result.Success) image.Dispose();

            return (result, isBlack, result.Success ? image : null);

            void ProcessImage(DataBox dataBox, Texture2DDescription texture)
            {
                BitmapData data = image.LockBits(new Rectangle(0, 0, texture.Width, texture.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* dataHead = (byte*)dataBox.DataPointer.ToPointer();

                    for (int x = 0; x < texture.Width; x++)
                    {
                        for (int y = 0; y < texture.Height; y++)
                        {
                            byte* pixPtr = (byte*)(data.Scan0 + y * data.Stride + x * 3);

                            int pos = x + y * texture.Width;
                            pos *= 4;

                            byte r = dataHead[pos + 2];
                            byte g = dataHead[pos + 1];
                            byte b = dataHead[pos + 0];

                            if (isBlack && (r != 0 || g != 0 || b != 0)) isBlack = false;

                            pixPtr[0] = b;
                            pixPtr[1] = g;
                            pixPtr[2] = r;
                        }
                    }
                }

                image.UnlockBits(data);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    factory.Dispose();
                    adapter.Dispose();
                    device.Dispose();
                    output.Dispose();
                    output1.Dispose();
                    screenTexture.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                factory = null;
                adapter = null;
                device = null;
                output = null;
                output1 = null;
                screenTexture = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DirectXScreenCapturer()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
