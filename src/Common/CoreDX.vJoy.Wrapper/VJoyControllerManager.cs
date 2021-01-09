using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.vJoy.Wrapper
{
    public class VJoyControllerManager : IDisposable
    {
        private static readonly bool _is64BitRuntime = IntPtr.Size == 8;
        private static readonly object _locker = new object();
        private static VJoyControllerManager _manager = null;
        private static WeakReference alcWeakRef;

        private VJoyAssemblyLoadContext _vJoyAssemblyLoadContext;
        private Assembly _vJoyInterfaceWrapAssembly;
        private object _joystick;
        private Type _vJoyType;
        private Type _VjdStatEnumType;
        private Type _hidUsagesEnumType;
        private object[] _axisEnumValues;
        private IVJoyController[] _controllers;

        private Delegate _getVJDStatusFunc;
        private Delegate _getVJDAxisExist;
        private Func<uint, int> _getVJDButtonNumber;
        private Func<uint, int> _getVJDContPovNumber;
        private Func<uint, int> _getVJDDiscPovNumber;
        private Func<bool> _resetAll;
        private Func<uint, bool> _acquireVJD;
        private Action<uint> _relinquishVJD;

        public static bool IsDriverLoaded => alcWeakRef?.IsAlive == true;
        public bool IsVJoyEnabled { get; }
        public string VJoyManufacturerString { get; }
        public string VJoyProductString { get; }
        public string VJoySerialNumberString { get; }
        public bool DriverMatch { get; }
        public uint DllVer { get; }
        public uint DrvVer { get; }

        private VJoyControllerManager()
        {
            var path = Process.GetCurrentProcess().MainModule.FileName;
            var filePath = $@"{path.Substring(0, path.LastIndexOf('\\'))}\{(_is64BitRuntime ? "x64" : "x86")}\vJoyInterfaceWrap.dll";

            _vJoyAssemblyLoadContext = new VJoyAssemblyLoadContext();
            _vJoyInterfaceWrapAssembly = _vJoyAssemblyLoadContext.LoadFromAssemblyPath(filePath);
            _joystick = Activator.CreateInstance(_vJoyInterfaceWrapAssembly.GetTypes().Single(t => t.Name == "vJoy"));
            _vJoyType = _joystick.GetType();

            IsVJoyEnabled = (bool)_vJoyType.GetMethod("vJoyEnabled").Invoke(_joystick, null);
            if (!IsVJoyEnabled) return;

            _VjdStatEnumType = _vJoyInterfaceWrapAssembly.GetType("VjdStat");
            _hidUsagesEnumType = _vJoyInterfaceWrapAssembly.GetType("HID_USAGES");
            _axisEnumValues = new[]
                {
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_X"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_Y"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_Z"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_RX"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_RY"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_RZ"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_SL0"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_SL1"),
                    Enum.Parse(_hidUsagesEnumType, "HID_USAGE_WHL")
                };
            _controllers = new IVJoyController[16];

            VJoyManufacturerString = (string)_vJoyType.GetMethod("GetvJoyManufacturerString").Invoke(_joystick, null);
            VJoyProductString = (string)_vJoyType.GetMethod("GetvJoyProductString").Invoke(_joystick, null);
            VJoySerialNumberString = (string)_vJoyType.GetMethod("GetvJoySerialNumberString").Invoke(_joystick, null);

            _getVJDButtonNumber = (Func<uint, int>)_vJoyType.GetMethod("GetVJDButtonNumber").CreateDelegate(typeof(Func<uint, int>), _joystick);
            _getVJDContPovNumber = (Func<uint, int>)_vJoyType.GetMethod("GetVJDContPovNumber").CreateDelegate(typeof(Func<uint, int>), _joystick);
            _getVJDDiscPovNumber = (Func<uint, int>)_vJoyType.GetMethod("GetVJDDiscPovNumber").CreateDelegate(typeof(Func<uint, int>), _joystick);
            _resetAll = (Func<bool>)_vJoyType.GetMethod("ResetAll").CreateDelegate(typeof(Func<bool>), _joystick);
            _acquireVJD = (Func<uint, bool>)_vJoyType.GetMethod("AcquireVJD").CreateDelegate(typeof(Func<uint, bool>), _joystick);
            _relinquishVJD = (Action<uint>)_vJoyType.GetMethod("RelinquishVJD").CreateDelegate(typeof(Action<uint>), _joystick);

            var funcType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(uint), _VjdStatEnumType });
            _getVJDStatusFunc = _vJoyType.GetMethod("GetVJDStatus").CreateDelegate(funcType, _joystick);

            funcType = typeof(Func<,,>).MakeGenericType(new Type[] { typeof(uint), _hidUsagesEnumType, typeof(bool) });
            _getVJDAxisExist = _vJoyType.GetMethod("GetVJDAxisExist").CreateDelegate(funcType, _joystick);

            var args = new object[] { 0u, 0u };
            DriverMatch = (bool)_vJoyType.GetMethod("DriverMatch").Invoke(_joystick, args);
            DllVer = (uint)args[0];
            DrvVer = (uint)args[1];
        }

        public static VJoyControllerManager GetManager()
        {
            if (_manager == null)
                lock (_locker)
                    if (_manager == null)
                    {
                        _manager = new VJoyControllerManager();
                        alcWeakRef = new WeakReference(_manager._vJoyAssemblyLoadContext, trackResurrection: true);
                    }

            return _manager;
        }

        public static void ReleaseManager()
        {
            if (_manager != null)
                lock (_locker)
                    if (_manager != null)
                    {
                        var inUse = _manager._controllers.Where(c => c?.HasRelinquished == false);
                        if (inUse.Any())
                        {
                            throw new InvalidOperationException($"Controllers with index ({string.Join(", ", inUse.Select(x => x.Id))}) are in use. Please relinquish first.");
                        }

                        _manager._axisEnumValues = null;
                        _manager._getVJDAxisExist = null;
                        _manager._getVJDButtonNumber = null;
                        _manager._getVJDContPovNumber = null;
                        _manager._getVJDDiscPovNumber = null;
                        _manager._getVJDStatusFunc = null;
                        _manager._hidUsagesEnumType = null;
                        _manager._joystick = null;
                        _manager._resetAll = null;
                        _manager._acquireVJD = null;
                        _manager._relinquishVJD = null;
                        _manager._VjdStatEnumType = null;
                        _manager._vJoyInterfaceWrapAssembly = null;
                        _manager._vJoyType = null;
                        _manager._controllers = null;

                        _manager.UnloadContext();
                        _manager = null;
                    }

        }

        public static bool UnloadDriver(int retryCount = 3)
        {
            if (!IsDriverLoaded) return false;

            ReleaseManager();
            for (int i = 0; IsDriverLoaded && (i < retryCount); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            if (alcWeakRef is { IsAlive: false }) alcWeakRef = null;
            return !IsDriverLoaded;
        }

        private void UnloadContext()
        {
            _vJoyAssemblyLoadContext.Unload();
            _vJoyAssemblyLoadContext = null;
        }

        public object GetVJDStatus(uint id) => IsVJoyEnabled ? _getVJDStatusFunc.DynamicInvoke(id) : null;

        public int GetVJDButtonNumber(uint id) => IsVJoyEnabled ? _getVJDButtonNumber(id) : 0;

        public int GetVJDContPovNumber(uint id) => IsVJoyEnabled ? _getVJDContPovNumber(id) : 0;

        public int GetVJDDiscPovNumber(uint id) => IsVJoyEnabled ? _getVJDDiscPovNumber(id) : 0;

        public bool GetVJDAxisExist(uint id, USAGES usages) => IsVJoyEnabled ? (bool)_getVJDAxisExist.DynamicInvoke(new object[] { id, _axisEnumValues[(int)usages] }) : false;

        public bool ResetAll() => _resetAll?.Invoke() ?? false;

        public IVJoyController AcquireController(uint id)
        {
            if (!IsVJoyEnabled) return null;
            if (id == 0 || id > 16) return null;

            var status = _getVJDStatusFunc.DynamicInvoke(id).ToString();
            bool acquireSuccessed;
            switch (status)
            {
                case "VJD_STAT_OWN":
                    goto case "VJD_STAT_FREE";
                case "VJD_STAT_FREE":
                    acquireSuccessed = _acquireVJD(id);
                    break;
                case "VJD_STAT_BUSY":
                    goto default;
                case "VJD_STAT_MISS":
                    goto default;
                default:
                    return null;
            };

            if (!acquireSuccessed) return null;

            var controller = new VJoyController(id, _manager);
            _manager._controllers[id - 1] = controller;
            return controller;
        }

        public void RelinquishController(IVJoyController controller)
        {
            if (!controller.HasRelinquished)
                lock (controller)
                    if (!controller.HasRelinquished)
                    {
                        _relinquishVJD(controller.Id);
                        (controller as VJoyController).SetHasRelinquished();
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
                    ReleaseManager();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~VJoyControllerManager()
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

        private class VJoyAssemblyLoadContext : AssemblyLoadContext
        {
            public VJoyAssemblyLoadContext() : base(isCollectible: true) { }

            protected override Assembly Load(AssemblyName name) => null;
        }

        private class VJoyController : IVJoyController
        {
            private VJoyControllerManager _manager;
            private bool _hasRelinquished = false;
            private Type _vJoyType;
            private Type _hidUsagesEnumType;
            private object _joystick;
            private object[] _axisEnumValues;
            private Delegate _setAxisFunc;
            private Func<bool, uint, uint, bool> _setBtn;
            private Func<int, uint, uint, bool> _setContPov;
            private Func<int, uint, uint, bool> _setDiscPov;
            private Func<uint, bool> _reset;
            private Func<uint, bool> _resetButtons;
            private Func<uint, bool> _resetPovs;

            public VJoyController(uint id, VJoyControllerManager manager)
            {
                Id = id;
                _manager = manager;
                _vJoyType = _manager._vJoyType;
                _joystick = _manager._joystick;
                _hidUsagesEnumType = _manager._hidUsagesEnumType;
                _axisEnumValues = _manager._axisEnumValues;

                // Check which axes are supported

                HasAxisX = _manager.GetVJDAxisExist(id, USAGES.X);
                HasAxisY = _manager.GetVJDAxisExist(id, USAGES.Y);
                HasAxisZ = _manager.GetVJDAxisExist(id, USAGES.Z);
                HasAxisRx = _manager.GetVJDAxisExist(id, USAGES.Rx);
                HasAxisRy = _manager.GetVJDAxisExist(id, USAGES.Ry);
                HasAxisRz = _manager.GetVJDAxisExist(id, USAGES.Rz);
                HasSlider0 = _manager.GetVJDAxisExist(id, USAGES.Slider0);
                HasSlider1 = _manager.GetVJDAxisExist(id, USAGES.Slider1);
                HasWheel = _manager.GetVJDAxisExist(id, USAGES.Wheel);
                // Get the number of buttons and POV Hat switchessupported by this vJoy device
                ButtonCount = _manager.GetVJDButtonNumber(id);
                ContPovCount = _manager.GetVJDContPovNumber(id);
                DiscPovCount = _manager.GetVJDDiscPovNumber(id);

                var args = new object[] { Id, _axisEnumValues[0], 0L };
                var hasAxis = (bool)_vJoyType.GetMethod("GetVJDAxisMax").Invoke(_joystick, args);
                AxisMaxValue = hasAxis ? (long)args[2] : (long?)null;

                _reset = (Func<uint, bool>)_vJoyType.GetMethod("ResetVJD").CreateDelegate(typeof(Func<uint, bool>), _joystick);
                _reset(Id);

                _setBtn = (Func<bool, uint, uint, bool>)_vJoyType.GetMethod("SetBtn").CreateDelegate(typeof(Func<bool, uint, uint, bool>), _joystick);
                _setContPov = (Func<int, uint, uint, bool>)_vJoyType.GetMethod("SetContPov").CreateDelegate(typeof(Func<int, uint, uint, bool>), _joystick);
                _setDiscPov = (Func<int, uint, uint, bool>)_vJoyType.GetMethod("SetDiscPov").CreateDelegate(typeof(Func<int, uint, uint, bool>), _joystick);
                _resetButtons = (Func<uint, bool>)_vJoyType.GetMethod("ResetButtons").CreateDelegate(typeof(Func<uint, bool>), _joystick);
                _resetPovs = (Func<uint, bool>)_vJoyType.GetMethod("ResetPovs").CreateDelegate(typeof(Func<uint, bool>), _joystick);

                var funcType = typeof(Func<,,,>).MakeGenericType(new Type[] { typeof(int), typeof(uint), _hidUsagesEnumType, typeof(bool) });
                _setAxisFunc = _vJoyType.GetMethod("SetAxis").CreateDelegate(funcType, _joystick);
            }

            public uint Id { get; }
            public bool HasRelinquished => _hasRelinquished;
            public bool HasAxisX { get; }
            public bool HasAxisY { get; }
            public bool HasAxisZ { get; }
            public bool HasAxisRx { get; }
            public bool HasAxisRy { get; }
            public bool HasAxisRz { get; }
            public bool HasSlider0 { get; }
            public bool HasSlider1 { get; }
            public bool HasWheel { get; }
            public int ButtonCount { get; }
            public int ContPovCount { get; }
            public int DiscPovCount { get; }
            public long? AxisMaxValue { get; }

            public bool Reset() => _reset(Id);

            public bool ResetButtons() => _resetButtons(Id);

            public bool ResetPovs() => _resetPovs(Id);

            public bool SetAxisX(int value)
            {
                if (HasRelinquished || !HasAxisX || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[0]);
            }

            public bool SetAxisY(int value)
            {
                if (HasRelinquished || !HasAxisY || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[1]);
            }

            public bool SetAxisZ(int value)
            {
                if (HasRelinquished || !HasAxisZ || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[2]);
            }

            public bool SetAxisRx(int value)
            {
                if (HasRelinquished || !HasAxisRx || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[3]);
            }

            public bool SetAxisRy(int value)
            {
                if (HasRelinquished || !HasAxisRy || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[4]);
            }

            public bool SetAxisRz(int value)
            {
                if (HasRelinquished || !HasAxisRz || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[5]);
            }

            public bool SetSlider0(int value)
            {
                if (HasRelinquished || !HasSlider0 || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[6]);
            }

            public bool SetSlider1(int value)
            {
                if (HasRelinquished || !HasSlider1 || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[7]);
            }

            public bool SetWheel(int value)
            {
                if (HasRelinquished || !HasWheel || value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[8]);
            }

            public bool PressButton(uint btnNo)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                return _setBtn(true, Id, btnNo);
            }

            public bool ReleaseButton(uint btnNo)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                return _setBtn(false, Id, btnNo);
            }

            public bool ClickButton(uint btnNo, int milliseconds = 50)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                var result = _setBtn(true, Id, btnNo);
                Thread.Sleep(milliseconds);
                return _setBtn(false, Id, btnNo) && result;
            }

            public async Task<bool> ClickButtonAsync(uint btnNo, int milliseconds, CancellationToken token = default)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                if (token.IsCancellationRequested) return false;
                var result = _setBtn(true, Id, btnNo);
                await Task.Delay(milliseconds);
                if (token.IsCancellationRequested) return false;
                return _setBtn(false, Id, btnNo) && result;
            }

            public bool SetContPov(int value, uint povNo)
            {
                if (HasRelinquished || ContPovCount < 1 || value > AxisMaxValue) return false;
                return _setContPov(value, Id, povNo);
            }

            public bool SetDiscPov(int value, uint povNo)
            {
                if (HasRelinquished || DiscPovCount < 1 || value > AxisMaxValue) return false;
                return _setDiscPov(value, Id, povNo);
            }

            internal void SetHasRelinquished() => _hasRelinquished = true;

            private void Relinquish()
            {
                _manager.RelinquishController(this);
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
                        ResetPovs();
                        ResetButtons();
                        Reset();
                        Relinquish();

                        _manager = null;
                        _vJoyType = null;
                        _hidUsagesEnumType = null;
                        _joystick = null;
                        _axisEnumValues = null;
                        _setAxisFunc = null;
                        _setBtn = null;
                        _setContPov = null;
                        _setDiscPov = null;
                        _reset = null;
                        _resetButtons = null;
                        _resetPovs = null;
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                    // TODO: 将大型字段设置为 null。

                    disposedValue = true;
                }
            }

            // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
            // ~VJoyController()
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

        public enum USAGES
        {
            X = 0,
            Y,
            Z,
            Rx,
            Ry,
            Rz,
            Slider0,
            Slider1,
            Wheel
        }
    }
}
