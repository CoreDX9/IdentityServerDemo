using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace CoreDX.vJoy.Wrapper
{
    public interface IVJoyController
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
        bool ButtonDown(uint btnNo);
        bool ButtonUp(uint btnNo);
        bool PressButton(uint btnNo, int milliseconds);
        bool SetContPov(int value, uint povNo);
        bool SetDiscPov(int value, uint povNo);
    }

    public class VJoyControllerManager
    {
        private static readonly bool _is64BitRuntime = IntPtr.Size == 8;
        private static readonly object _locker = new object();
        private static VJoyControllerManager _manager = null;

        private readonly VJoyAssemblyLoadContext _vJoyAssemblyLoadContext;
        private readonly Assembly _vJoyInterfaceWrapAssembly;
        private readonly object _joystick;
        private readonly Type _vJoyType;
        private readonly Type _VjdStatEnumType;
        private readonly Type _hidUsagesEnumType;
        private readonly object[] _axisEnumValues;

        private readonly Delegate _getVJDStatusFunc;
        private readonly Delegate _getVJDAxisExist;
        private readonly Func<uint, int> _getVJDButtonNumber;
        private readonly Func<uint, int> _getVJDContPovNumber;
        private readonly Func<uint, int> _getVJDDiscPovNumber;
        private readonly Func<bool> _resetAll;

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

            IsVJoyEnabled = (bool)_vJoyType.GetMethod("vJoyEnabled").Invoke(_joystick, null);
            VJoyManufacturerString = IsVJoyEnabled ? (string)_vJoyType.GetMethod("GetvJoyManufacturerString").Invoke(_joystick, null) : null;
            VJoyProductString = IsVJoyEnabled ? (string)_vJoyType.GetMethod("GetvJoyProductString").Invoke(_joystick, null) : null;
            VJoySerialNumberString = IsVJoyEnabled ? (string)_vJoyType.GetMethod("GetvJoySerialNumberString").Invoke(_joystick, null) : null;

            _getVJDButtonNumber = (Func<uint, int>)_vJoyType.GetMethod("GetVJDButtonNumber").CreateDelegate(typeof(Func<uint, int>), _joystick);
            _getVJDContPovNumber = (Func<uint, int>)_vJoyType.GetMethod("GetVJDContPovNumber").CreateDelegate(typeof(Func<uint, int>), _joystick);
            _getVJDDiscPovNumber = (Func<uint, int>)_vJoyType.GetMethod("GetVJDDiscPovNumber").CreateDelegate(typeof(Func<uint, int>), _joystick);
            _resetAll = (Func<bool>)_vJoyType.GetMethod("ResetAll").CreateDelegate(typeof(Func<bool>), _joystick);

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
                        _manager = new VJoyControllerManager();

            return _manager;
        }

        public static void ReleaseManager()
        {
            if (_manager != null)
                lock (_locker)
                    if (_manager != null)
                    {
                        _manager.UnLoadContext();
                        _manager = null;
                    }

        }

        private void UnLoadContext()
        {
            _vJoyAssemblyLoadContext.Unload();
        }

        public object GetVJDStatus(uint id)
        {
            return _getVJDStatusFunc.DynamicInvoke(id);
        }

        public int GetVJDButtonNumber(uint id) => _getVJDButtonNumber(id);

        public int GetVJDContPovNumber(uint id) => _getVJDContPovNumber(id);

        public int GetVJDDiscPovNumber(uint id) => _getVJDDiscPovNumber(id);

        public bool GetVJDAxisExist(uint id, USAGES usages) => (bool)_getVJDAxisExist.DynamicInvoke(new object[] { id, _axisEnumValues[(int)usages] });

        public bool ResetAll() => _resetAll();

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
                    acquireSuccessed = (bool)_vJoyType.GetMethod("AcquireVJD").Invoke(_joystick, new object[] { id });
                    break;
                case "VJD_STAT_BUSY":
                    goto default;
                case "VJD_STAT_MISS":
                    goto default;
                default:
                    return null;
            };

            if (!acquireSuccessed) return null;

            return new VJoyController(id, _manager);
        }

        private class VJoyAssemblyLoadContext : AssemblyLoadContext
        {
            public VJoyAssemblyLoadContext() : base(isCollectible: true)
            {
            }

            protected override Assembly Load(AssemblyName name)
            {
                return null;
            }
        }

        private class VJoyController : IVJoyController
        {
            private readonly Type _vJoyType;
            private readonly Type _hidUsagesEnumType;
            private readonly object _joystick;
            private readonly object[] _axisEnumValues;
            private readonly Delegate _setAxisFunc;
            private readonly Func<bool, uint, uint, bool> _setBtn;
            private readonly Func<int, uint, uint, bool> _setContPov;
            private readonly Func<int, uint, uint, bool> _setDiscPov;
            private readonly Func<uint, bool> _reset;
            private readonly Func<uint, bool> _resetButtons;
            private readonly Func<uint, bool> _resetPovs;
            private readonly Action<uint> _relinquish;

            public VJoyController(uint id, VJoyControllerManager manager)
            {
                Id = id;
                _vJoyType = manager._vJoyType;
                _joystick = manager._joystick;
                _hidUsagesEnumType = manager._hidUsagesEnumType;
                _axisEnumValues = manager._axisEnumValues;

                // Check which axes are supported

                HasAxisX = manager.GetVJDAxisExist(id, USAGES.X);
                HasAxisY = manager.GetVJDAxisExist(id, USAGES.Y);
                HasAxisZ = manager.GetVJDAxisExist(id, USAGES.Z);
                HasAxisRx = manager.GetVJDAxisExist(id, USAGES.Rx);
                HasAxisRy = manager.GetVJDAxisExist(id, USAGES.Ry);
                HasAxisRz = manager.GetVJDAxisExist(id, USAGES.Rz);
                HasSlider0 = manager.GetVJDAxisExist(id, USAGES.Slider0);
                HasSlider1 = manager.GetVJDAxisExist(id, USAGES.Slider1);
                HasWheel = manager.GetVJDAxisExist(id, USAGES.Wheel);
                // Get the number of buttons and POV Hat switchessupported by this vJoy device
                ButtonCount = manager.GetVJDButtonNumber(id);
                ContPovCount = manager.GetVJDContPovNumber(id);
                DiscPovCount = manager.GetVJDDiscPovNumber(id);

                var args = new object[] { Id, _axisEnumValues[0], 0L };
                var hasAxis = (bool)_vJoyType.GetMethod("GetVJDAxisMax").Invoke(_joystick, args);
                AxisMaxValue = hasAxis ? (long)args[2] : (long?)null;

                _reset = (Func<uint, bool>)_vJoyType.GetMethod("ResetVJD").CreateDelegate(typeof(Func<uint, bool>), _joystick);
                _reset(Id);

                _setBtn = (Func<bool, uint, uint, bool>)_vJoyType.GetMethod("SetBtn").CreateDelegate(typeof(Func<bool, uint, uint, bool>), _joystick);
                _setContPov = (Func<int, uint, uint, bool>)_vJoyType.GetMethod("SetContPov").CreateDelegate(typeof(Func<int, uint, uint, bool>), _joystick);
                _setDiscPov = (Func<int, uint, uint, bool>)_vJoyType.GetMethod("SetDiscPov").CreateDelegate(typeof(Func<int, uint, uint, bool>), _joystick);
                _relinquish = (Action<uint>)_vJoyType.GetMethod("RelinquishVJD").CreateDelegate(typeof(Action<uint>), _joystick);
                _resetButtons = (Func<uint, bool>)_vJoyType.GetMethod("ResetButtons").CreateDelegate(typeof(Func<uint, bool>), _joystick);
                _resetPovs = (Func<uint, bool>)_vJoyType.GetMethod("ResetPovs").CreateDelegate(typeof(Func<uint, bool>), _joystick);

                var funcType = typeof(Func<,,,>).MakeGenericType(new Type[] { typeof(int), typeof(uint), _hidUsagesEnumType, typeof(bool) });
                _setAxisFunc = _vJoyType.GetMethod("SetAxis").CreateDelegate(funcType, _joystick);
            }

            public uint Id { get; }
            public bool HasRelinquished { get; private set; } = false;
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

            public void Relinquish()
            {
                HasRelinquished = true;
                _relinquish(Id);
            }

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

            public bool ButtonDown(uint btnNo)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                return _setBtn(true, Id, btnNo);
            }

            public bool ButtonUp(uint btnNo)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                return _setBtn(false, Id, btnNo);
            }

            public bool PressButton(uint btnNo, int milliseconds = 50)
            {
                if (HasRelinquished || btnNo == 0 || btnNo > ButtonCount) return false;
                _setBtn(true, Id, btnNo);
                System.Threading.Thread.Sleep(milliseconds);
                return _setBtn(false, Id, btnNo);
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
