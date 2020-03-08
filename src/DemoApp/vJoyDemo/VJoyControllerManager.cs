using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace vJoyDemo
{
    public interface IVJoyController
    {
        uint Id { get; }
        bool HasAxisX { get; }
        bool HasAxisY { get; }
        bool HasAxisZ { get; }
        bool HasAxisRx { get; }
        bool HasAxisRz { get; }
        int ButtonCount { get; }
        int ContPovCount { get; }
        int DiscPovCount { get; }
        long? AxisMaxValue { get; }

        bool Reset();
        bool SetAxisX(int value);
        bool SetAxisY(int value);
        bool SetAxisZ(int value);
        bool SetAxisRx(int value);
        bool SetAxisRz(int value);
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

        private readonly Delegate _getVJDStatusFunc;

        public string VJoyManufacturerString { get; }
        public string VJoyProductString { get; }
        public string VJoySerialNumberString { get; }
        public bool DriverMatch { get; }
        public uint DllVer { get; }
        public uint DrvVer { get; }

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
            IsVJoyEnabled = (bool)_vJoyType.GetMethod("vJoyEnabled").Invoke(_joystick, null);
            VJoyManufacturerString = IsVJoyEnabled ? (string)_vJoyType.GetMethod("GetvJoyManufacturerString").Invoke(_joystick, null) : null;
            VJoyProductString = IsVJoyEnabled ? (string)_vJoyType.GetMethod("GetvJoyProductString").Invoke(_joystick, null) : null;
            VJoySerialNumberString = IsVJoyEnabled ? (string)_vJoyType.GetMethod("GetvJoySerialNumberString").Invoke(_joystick, null) : null;

            var funcType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(uint), _VjdStatEnumType });
            _getVJDStatusFunc = _vJoyType.GetMethod("GetVJDStatus").CreateDelegate(funcType, _joystick);

            var args = new object[] { 0u, 0u };
            DriverMatch = (bool)_vJoyType.GetMethod("DriverMatch").Invoke(_joystick, args);
            DllVer = (uint)args[0];
            DrvVer = (uint)args[1];
        }

        private void UnLoadContext()
        {
            _vJoyAssemblyLoadContext.Unload();
        }

        public bool IsVJoyEnabled { get; }

        public object GetVJDStatus(uint id)
        {
            return _getVJDStatusFunc.DynamicInvoke(id);
        }

        public IVJoyController GetController(uint id)
        {
            if (!IsVJoyEnabled) return null;

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
            private readonly Delegate _setBtnFunc;
            private readonly Delegate _setContPovFunc;
            private readonly Delegate _setDiscPovFunc;
            private readonly Delegate _resetFunc;

            delegate bool del(int a);

            public VJoyController(uint id, VJoyControllerManager manager)
            {
                Id = id;
                _vJoyType = manager._vJoyType;
                _joystick = manager._joystick;
                _hidUsagesEnumType = manager._hidUsagesEnumType;

                // Check which axes are supported
                _axisEnumValues = new[]
                    {
                        Enum.Parse(_hidUsagesEnumType, "HID_USAGE_X"),
                        Enum.Parse(_hidUsagesEnumType, "HID_USAGE_Y"),
                        Enum.Parse(_hidUsagesEnumType, "HID_USAGE_Z"),
                        Enum.Parse(_hidUsagesEnumType, "HID_USAGE_RX"),
                        Enum.Parse(_hidUsagesEnumType, "HID_USAGE_RZ")
                    };

                HasAxisX = (bool)_vJoyType.GetMethod("GetVJDAxisExist").Invoke(_joystick, new object[] { Id, _axisEnumValues[0] });
                HasAxisY = (bool)_vJoyType.GetMethod("GetVJDAxisExist").Invoke(_joystick, new object[] { Id, _axisEnumValues[1] });
                HasAxisZ = (bool)_vJoyType.GetMethod("GetVJDAxisExist").Invoke(_joystick, new object[] { Id, _axisEnumValues[2] });
                HasAxisRx = (bool)_vJoyType.GetMethod("GetVJDAxisExist").Invoke(_joystick, new object[] { Id, _axisEnumValues[3] });
                HasAxisRz = (bool)_vJoyType.GetMethod("GetVJDAxisExist").Invoke(_joystick, new object[] { Id, _axisEnumValues[4] });
                // Get the number of buttons and POV Hat switchessupported by this vJoy device
                ButtonCount = (int)_vJoyType.GetMethod("GetVJDButtonNumber").Invoke(_joystick, new object[] { Id });
                ContPovCount = (int)_vJoyType.GetMethod("GetVJDContPovNumber").Invoke(_joystick, new object[] { Id });
                DiscPovCount = (int)_vJoyType.GetMethod("GetVJDDiscPovNumber").Invoke(_joystick, new object[] { Id });

                var args = new object[] { Id, _axisEnumValues[0], 0L };
                var hasAxis = (bool)_vJoyType.GetMethod("GetVJDAxisMax").Invoke(_joystick, args);
                AxisMaxValue = hasAxis ? (long)args[2] : (long?)null;

                var funcType = typeof(Func<,>).MakeGenericType(new Type[] { typeof(uint), typeof(bool) });
                _resetFunc = _vJoyType.GetMethod("ResetVJD").CreateDelegate(funcType, _joystick);
                _resetFunc.DynamicInvoke(Id);

                funcType = typeof(Func<,,,>).MakeGenericType(new Type[] { typeof(int), typeof(uint), _hidUsagesEnumType, typeof(bool) });
                _setAxisFunc = _vJoyType.GetMethod("SetAxis").CreateDelegate(funcType, _joystick);

                funcType = typeof(Func<,,,>).MakeGenericType(new Type[] { typeof(bool), typeof(uint), typeof(uint), typeof(bool) });
                _setBtnFunc = _vJoyType.GetMethod("SetBtn").CreateDelegate(funcType, _joystick);

                funcType = typeof(Func<,,,>).MakeGenericType(new Type[] { typeof(int), typeof(uint), typeof(uint), typeof(bool) });
                _setContPovFunc = _vJoyType.GetMethod("SetContPov").CreateDelegate(funcType, _joystick);
                _setDiscPovFunc = _vJoyType.GetMethod("SetDiscPov").CreateDelegate(funcType, _joystick);
            }

            public uint Id { get; }
            public bool HasAxisX { get; }
            public bool HasAxisY { get; }
            public bool HasAxisZ { get; }
            public bool HasAxisRx { get; }
            public bool HasAxisRz { get; }
            public int ButtonCount { get; }
            public int ContPovCount { get; }
            public int DiscPovCount { get; }
            public long? AxisMaxValue { get; }

            public bool Reset()
            {
                return (bool)_resetFunc.DynamicInvoke(Id);
            }

            public bool SetAxisX(int value)
            {
                if (value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[0]);
            }

            public bool SetAxisY(int value)
            {
                if (value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[1]);
            }

            public bool SetAxisZ(int value)
            {
                if (value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[2]);
            }

            public bool SetAxisRx(int value)
            {
                if (value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[3]);
            }

            public bool SetAxisRz(int value)
            {
                if (value > AxisMaxValue) return false;
                return (bool)_setAxisFunc.DynamicInvoke(value, Id, _axisEnumValues[4]);
            }

            public bool ButtonDown(uint btnNo)
            {
                if (btnNo == 0 || btnNo > ButtonCount) return false;
                return (bool)_setBtnFunc.DynamicInvoke(true, Id, btnNo);
            }

            public bool ButtonUp(uint btnNo)
            {
                if (btnNo == 0 || btnNo > ButtonCount) return false;
                return (bool)_setBtnFunc.DynamicInvoke(false, Id, btnNo);
            }

            public bool PressButton(uint btnNo, int milliseconds = 50)
            {
                if (btnNo == 0 || btnNo > ButtonCount) return false;
                _setBtnFunc.DynamicInvoke(true, Id, btnNo);
                System.Threading.Thread.Sleep(milliseconds);
                return (bool)_setBtnFunc.DynamicInvoke(false, Id, btnNo);
            }

            public bool SetContPov(int value, uint povNo)
            {
                return (bool)_setContPovFunc.DynamicInvoke(value, Id, povNo);
            }

            public bool SetDiscPov(int value, uint povNo)
            {
                return (bool)_setDiscPovFunc.DynamicInvoke(value, Id, povNo);
            }
        }
    }
}
