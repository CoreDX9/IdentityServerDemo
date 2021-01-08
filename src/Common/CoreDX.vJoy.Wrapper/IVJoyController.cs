using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.vJoy.Wrapper
{
    public interface IVJoyController : IDisposable
    {
        uint Id { get; }
        bool HasRelinquished { get; }
        bool HasAxisX { get; }
        bool HasAxisY { get; }
        bool HasAxisZ { get; }
        bool HasAxisRx { get; }
        bool HasAxisRy { get; }
        bool HasAxisRz { get; }
        bool HasSlider0 { get; }
        bool HasSlider1 { get; }
        bool HasWheel { get; }
        int ButtonCount { get; }
        int ContPovCount { get; }
        int DiscPovCount { get; }
        long? AxisMaxValue { get; }

        bool Reset();
        bool ResetButtons();
        bool ResetPovs();
        void Relinquish();
        bool SetAxisX(int value);
        bool SetAxisY(int value);
        bool SetAxisZ(int value);
        bool SetAxisRx(int value);
        bool SetAxisRy(int value);
        bool SetAxisRz(int value);
        bool SetSlider0(int value);
        bool SetSlider1(int value);
        bool SetWheel(int value);
        bool PressButton(uint btnNo);
        bool ReleaseButton(uint btnNo);
        bool ClickButton(uint btnNo, int milliseconds);
        Task<bool> ClickButtonAsync(uint btnNo, int milliseconds, CancellationToken token);
        bool SetContPov(int value, uint povNo);
        bool SetDiscPov(int value, uint povNo);
    }
}
