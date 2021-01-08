﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// This project demonstrates how to write a simple vJoy feeder in C#
//
// You can compile it with either #define ROBUST OR #define EFFICIENT
// The fuctionality is similar - 
// The ROBUST section demonstrate the usage of functions that are easy and safe to use but are less efficient
// The EFFICIENT ection demonstrate the usage of functions that are more efficient
//
// Functionality:
//	The program starts with creating one joystick object. 
//	Then it petches the device id from the command-line and makes sure that it is within range
//	After testing that the driver is enabled it gets information about the driver
//	Gets information about the specified virtual device
//	This feeder uses only a few axes. It checks their existence and 
//	checks the number of buttons and POV Hat switches.
//	Then the feeder acquires the virtual device
//	Here starts and endless loop that feedes data into the virtual device
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////
//#define ROBUST
//#define EFFICIENT

using System;
using System.Threading.Tasks;
using System.Threading;
using CoreDX.vJoy.Wrapper;
//using vJoyInterfaceWrap;

//需要先安装 vJoy http://vjoystick.sourceforge.net/site/index.php/download-a-install/download
//原版代码需要根据系统复制 x86 或 x64 文件夹中的程序集到编译的程序文件夹并添加程序集引用
//Win+R 运行后面的命令能打开游戏控制器设置（win10 把 vJoy 识别为未知类别的设备，无法在设备上右键打开） control.exe /name Microsoft.GameControllers
namespace vJoyDemo
{
    class Program
    {
        static public uint id = 1;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Press Win+R to open control board with commond: control.exe /name Microsoft.GameControllers");
            Console.WriteLine("");

            #region 动态代理，根据运行环境载入 x86 或 x64 的程序集

            // Device ID can only be in the range 1-16
            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
                id = Convert.ToUInt32(args[0]);
            if (id <= 0 || id > 16)
            {
                Console.WriteLine("Illegal device ID {0}\nExit!", id);
                return;
            }

            using (var vJoyManager = VJoyControllerManager.GetManager())
            {
                // Get the driver attributes (Vendor ID, Product ID, Version Number)
                if (!vJoyManager.IsVJoyEnabled)
                {
                    Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                    Console.WriteLine("\npress enter to exit");
                    Console.ReadKey(true);
                    return;
                }
                else
                    Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", vJoyManager.VJoyManufacturerString, vJoyManager.VJoyProductString, vJoyManager.VJoySerialNumberString);

                // Get the state of the requested device
                var status = vJoyManager.GetVJDStatus(id).ToString();
                switch (status)
                {
                    case "VJD_STAT_OWN":
                        Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
                        break;
                    case "VJD_STAT_FREE":
                        Console.WriteLine("vJoy Device {0} is free\n", id);
                        break;
                    case "VJD_STAT_BUSY":
                        Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
                        return;
                    case "VJD_STAT_MISS":
                        Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
                        return;
                    default:
                        Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
                        return;
                };

                // Acquire the target
                var joyCon = vJoyManager.AcquireController(id);
                if (joyCon == null)
                {
                    Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
                    return;
                }
                else
                    Console.WriteLine("Acquired: vJoy device number {0}.\n", id);

                using (joyCon)
                {
                    // Check which axes are supported
                    bool AxisX = joyCon.HasAxisX;
                    bool AxisY = joyCon.HasAxisY;
                    bool AxisZ = joyCon.HasAxisZ;
                    bool AxisRX = joyCon.HasAxisRx;
                    bool AxisRY = joyCon.HasAxisRy;
                    bool AxisRZ = joyCon.HasAxisRz;
                    bool Slider0 = joyCon.HasSlider0;
                    bool Slider1 = joyCon.HasSlider1;
                    // Get the number of buttons and POV Hat switchessupported by this vJoy device
                    int nButtons = joyCon.ButtonCount;
                    int ContPovNumber = joyCon.ContPovCount;
                    int DiscPovNumber = joyCon.DiscPovCount;

                    // Print results
                    Console.WriteLine("\nvJoy Device {0} capabilities:\n", id);
                    Console.WriteLine("Numner of buttons\t\t{0}\n", nButtons);
                    Console.WriteLine("Numner of Continuous POVs\t{0}\n", ContPovNumber);
                    Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber);
                    Console.WriteLine("Axis X\t\t{0}\n", AxisX ? "Yes" : "No");
                    Console.WriteLine("Axis Y\t\t{0}\n", AxisX ? "Yes" : "No");
                    Console.WriteLine("Axis Z\t\t{0}\n", AxisX ? "Yes" : "No");
                    Console.WriteLine("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No");
                    Console.WriteLine("Axis Ry\t\t{0}\n", AxisRY ? "Yes" : "No");
                    Console.WriteLine("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No");
                    Console.WriteLine("Slider0\t\t{0}\n", Slider0 ? "Yes" : "No");
                    Console.WriteLine("Slider1\t\t{0}\n", Slider1 ? "Yes" : "No");

                    // Test if DLL matches the driver
                    bool match = vJoyManager.DriverMatch;
                    if (match)
                        Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", vJoyManager.DllVer);
                    else
                        Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", vJoyManager.DrvVer, vJoyManager.DllVer);

                    Console.WriteLine("\npress enter to stat feeding");
                    Console.ReadKey(true);

                    Console.WriteLine("\nfeeding……");

                    var cancel = new CancellationTokenSource();
                    var task = Task.Run(() =>
                    {
                        int X, Y, Z, ZR, YR, XR, S0, S1;
                        uint count = 0;
                        long maxval = joyCon.AxisMaxValue ?? 0;
                        var successed = false;

                        X = 20;
                        Y = 30;
                        Z = 40;
                        XR = 60;
                        YR = 80;
                        ZR = 100;
                        S0 = 120;
                        S1 = 140;

                        // Feed the device in endless loop
                        while (!cancel.IsCancellationRequested)
                        {
                            // Set position of 4 axes
                            successed = joyCon.SetAxisX(X);
                            successed = joyCon.SetAxisY(Y);
                            successed = joyCon.SetAxisZ(Z);
                            successed = joyCon.SetAxisRx(XR);
                            successed = joyCon.SetAxisRy(YR);
                            successed = joyCon.SetAxisRz(ZR);
                            successed = joyCon.SetSlider0(S0);
                            successed = joyCon.SetSlider1(S1);

                            // Press/Release Buttons
                            successed = joyCon.PressButton(count / 50);
                            successed = joyCon.ReleaseButton(1 + count / 50);

                            // If Continuous POV hat switches installed - make them go round
                            // For high values - put the switches in neutral state
                            if (joyCon.ContPovCount > 0)
                            {
                                if ((count * 70) < 30000)
                                {
                                    successed = joyCon.SetContPov(((int)count * 70), 1);
                                    successed = joyCon.SetContPov(((int)count * 70) + 2000, 2);
                                    successed = joyCon.SetContPov(((int)count * 70) + 4000, 3);
                                    successed = joyCon.SetContPov(((int)count * 70) + 6000, 4);
                                }
                                else
                                {
                                    successed = joyCon.SetContPov(-1, 1);
                                    successed = joyCon.SetContPov(-1, 2);
                                    successed = joyCon.SetContPov(-1, 3);
                                    successed = joyCon.SetContPov(-1, 4);
                                };
                            };

                            // If Discrete POV hat switches installed - make them go round
                            // From time to time - put the switches in neutral state
                            if (joyCon.DiscPovCount > 0)
                            {
                                if (count < 550)
                                {
                                    successed = joyCon.SetDiscPov((((int)count / 20) + 0) % 4, 1);
                                    successed = joyCon.SetDiscPov((((int)count / 20) + 1) % 4, 2);
                                    successed = joyCon.SetDiscPov((((int)count / 20) + 2) % 4, 3);
                                    successed = joyCon.SetDiscPov((((int)count / 20) + 3) % 4, 4);
                                }
                                else
                                {
                                    successed = joyCon.SetDiscPov(-1, 1);
                                    successed = joyCon.SetDiscPov(-1, 2);
                                    successed = joyCon.SetDiscPov(-1, 3);
                                    successed = joyCon.SetDiscPov(-1, 4);
                                };
                            };

                            Thread.Sleep(20);
                            X += 150; if (X > maxval) X = 0;
                            Y += 250; if (Y > maxval) Y = 0;
                            Z += 350; if (Z > maxval) Z = 0;
                            XR += 240; if (XR > maxval) XR = 0;
                            YR += 220; if (YR > maxval) YR = 0;
                            ZR += 200; if (ZR > maxval) ZR = 0;
                            S0 += 180; if (S0 > maxval) S0 = 0;
                            S1 += 160; if (S1 > maxval) S1 = 0;

                            count++;
                            if (count > 640) count = 0;
                        }
                    }, cancel.Token);

                    Console.WriteLine("\npress enter to stop feeding");
                    Console.ReadKey(true);
                    cancel.Cancel();

                    await task;
                }
            }

            Console.WriteLine("\nfeeding stoped");

            if (VJoyControllerManager.IsDriverLoaded)
            {
                Console.WriteLine("\nunloading driver...");
                VJoyControllerManager.UnloadDriver();

                if (!VJoyControllerManager.IsDriverLoaded) Console.WriteLine("unload driver successed");
                else Console.WriteLine("unload driver failed");
            }

            Console.WriteLine("\npress enter to exit");
            Console.ReadKey(true);

            #endregion

            #region 原版代码

            // Declaring one joystick (Device id 1) and a position structure. 
            //static public vJoy joystick;
            //static public vJoy.JoystickState iReport;

            // Create one joystick object and a position structure.
            //            joystick = new vJoy();
            //            iReport = new vJoy.JoystickState();


            //            // Device ID can only be in the range 1-16
            //            if (args.Length > 0 && !String.IsNullOrEmpty(args[0]))
            //                id = Convert.ToUInt32(args[0]);
            //            if (id <= 0 || id > 16)
            //            {
            //                Console.WriteLine("Illegal device ID {0}\nExit!", id);
            //                return;
            //            }

            //            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            //            if (!joystick.vJoyEnabled())
            //            {
            //                Console.WriteLine("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
            //                return;
            //            }
            //            else
            //                Console.WriteLine("Vendor: {0}\nProduct :{1}\nVersion Number:{2}\n", joystick.GetvJoyManufacturerString(), joystick.GetvJoyProductString(), joystick.GetvJoySerialNumberString());

            //            // Get the state of the requested device
            //            VjdStat status = joystick.GetVJDStatus(id);
            //            switch (status)
            //            {
            //                case VjdStat.VJD_STAT_OWN:
            //                    Console.WriteLine("vJoy Device {0} is already owned by this feeder\n", id);
            //                    break;
            //                case VjdStat.VJD_STAT_FREE:
            //                    Console.WriteLine("vJoy Device {0} is free\n", id);
            //                    break;
            //                case VjdStat.VJD_STAT_BUSY:
            //                    Console.WriteLine("vJoy Device {0} is already owned by another feeder\nCannot continue\n", id);
            //                    return;
            //                case VjdStat.VJD_STAT_MISS:
            //                    Console.WriteLine("vJoy Device {0} is not installed or disabled\nCannot continue\n", id);
            //                    return;
            //                default:
            //                    Console.WriteLine("vJoy Device {0} general error\nCannot continue\n", id);
            //                    return;
            //            };

            //            // Check which axes are supported
            //            bool AxisX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            //            bool AxisY = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            //            bool AxisZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            //            bool AxisRX = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            //            bool AxisRZ = joystick.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            //            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            //            int nButtons = joystick.GetVJDButtonNumber(id);
            //            int ContPovNumber = joystick.GetVJDContPovNumber(id);
            //            int DiscPovNumber = joystick.GetVJDDiscPovNumber(id);

            //            // Print results
            //            Console.WriteLine("\nvJoy Device {0} capabilities:\n", id);
            //            Console.WriteLine("Numner of buttons\t\t{0}\n", nButtons);
            //            Console.WriteLine("Numner of Continuous POVs\t{0}\n", ContPovNumber);
            //            Console.WriteLine("Numner of Descrete POVs\t\t{0}\n", DiscPovNumber);
            //            Console.WriteLine("Axis X\t\t{0}\n", AxisX ? "Yes" : "No");
            //            Console.WriteLine("Axis Y\t\t{0}\n", AxisX ? "Yes" : "No");
            //            Console.WriteLine("Axis Z\t\t{0}\n", AxisX ? "Yes" : "No");
            //            Console.WriteLine("Axis Rx\t\t{0}\n", AxisRX ? "Yes" : "No");
            //            Console.WriteLine("Axis Rz\t\t{0}\n", AxisRZ ? "Yes" : "No");

            //            // Test if DLL matches the driver
            //            UInt32 DllVer = 0, DrvVer = 0;
            //            bool match = joystick.DriverMatch(ref DllVer, ref DrvVer);
            //            if (match)
            //                Console.WriteLine("Version of Driver Matches DLL Version ({0:X})\n", DllVer);
            //            else
            //                Console.WriteLine("Version of Driver ({0:X}) does NOT match DLL Version ({1:X})\n", DrvVer, DllVer);


            //            // Acquire the target
            //            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!joystick.AcquireVJD(id))))
            //            {
            //                Console.WriteLine("Failed to acquire vJoy device number {0}.\n", id);
            //                return;
            //            }
            //            else
            //                Console.WriteLine("Acquired: vJoy device number {0}.\n", id);

            //            Console.WriteLine("\npress enter to stat feeding");
            //            Console.ReadKey(true);

            //            int X, Y, Z, ZR, XR;
            //            uint count = 0;
            //            long maxval = 0;

            //            X = 20;
            //            Y = 30;
            //            Z = 40;
            //            XR = 60;
            //            ZR = 80;

            //            joystick.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);

            #endregion

#if ROBUST
                        bool res;
                        // Reset this device to default values
                        joystick.ResetVJD(id);

                        // Feed the device in endless loop
                        while (true)
                        {
                            // Set position of 4 axes
                            res = joystick.SetAxis(X, id, HID_USAGES.HID_USAGE_X);
                            res = joystick.SetAxis(Y, id, HID_USAGES.HID_USAGE_Y);
                            res = joystick.SetAxis(Z, id, HID_USAGES.HID_USAGE_Z);
                            res = joystick.SetAxis(XR, id, HID_USAGES.HID_USAGE_RX);
                            res = joystick.SetAxis(ZR, id, HID_USAGES.HID_USAGE_RZ);

                            // Press/Release Buttons
                            res = joystick.SetBtn(true, id, count / 50);
                            res = joystick.SetBtn(false, id, 1 + count / 50);

                            // If Continuous POV hat switches installed - make them go round
                            // For high values - put the switches in neutral state
                            if (ContPovNumber > 0)
                            {
                                if ((count * 70) < 30000)
                                {
                                    res = joystick.SetContPov(((int)count * 70), id, 1);
                                    res = joystick.SetContPov(((int)count * 70) + 2000, id, 2);
                                    res = joystick.SetContPov(((int)count * 70) + 4000, id, 3);
                                    res = joystick.SetContPov(((int)count * 70) + 6000, id, 4);
                                }
                                else
                                {
                                    res = joystick.SetContPov(-1, id, 1);
                                    res = joystick.SetContPov(-1, id, 2);
                                    res = joystick.SetContPov(-1, id, 3);
                                    res = joystick.SetContPov(-1, id, 4);
                                };
                            };

                            // If Discrete POV hat switches installed - make them go round
                            // From time to time - put the switches in neutral state
                            if (DiscPovNumber > 0)
                            {
                                if (count < 550)
                                {
                                    joystick.SetDiscPov((((int)count / 20) + 0) % 4, id, 1);
                                    joystick.SetDiscPov((((int)count / 20) + 1) % 4, id, 2);
                                    joystick.SetDiscPov((((int)count / 20) + 2) % 4, id, 3);
                                    joystick.SetDiscPov((((int)count / 20) + 3) % 4, id, 4);
                                }
                                else
                                {
                                    joystick.SetDiscPov(-1, id, 1);
                                    joystick.SetDiscPov(-1, id, 2);
                                    joystick.SetDiscPov(-1, id, 3);
                                    joystick.SetDiscPov(-1, id, 4);
                                };
                            };

                            System.Threading.Thread.Sleep(20);
                            X += 150; if (X > maxval) X = 0;
                            Y += 250; if (Y > maxval) Y = 0;
                            Z += 350; if (Z > maxval) Z = 0;
                            XR += 220; if (XR > maxval) XR = 0;
                            ZR += 200; if (ZR > maxval) ZR = 0;
                            count++;

                            if (count > 640)
                                count = 0;

                        } // While (Robust)

#endif // ROBUST
#if EFFICIENT

            byte[] pov = new byte[4];

      while (true)
            {
            iReport.bDevice = (byte)id;
            iReport.AxisX = X;
            iReport.AxisY = Y;
            iReport.AxisZ = Z;
            iReport.AxisZRot = ZR;
            iReport.AxisXRot = XR;

            // Set buttons one by one
            iReport.Buttons = (uint)(0x1 <<  (int)(count / 20));

        if (ContPovNumber>0)
        {
            // Make Continuous POV Hat spin
            iReport.bHats		= (count*70);
            iReport.bHatsEx1	= (count*70)+3000;
            iReport.bHatsEx2	= (count*70)+5000;
            iReport.bHatsEx3	= 15000 - (count*70);
            if ((count*70) > 36000)
            {
                iReport.bHats =    0xFFFFFFFF; // Neutral state
                iReport.bHatsEx1 = 0xFFFFFFFF; // Neutral state
                iReport.bHatsEx2 = 0xFFFFFFFF; // Neutral state
                iReport.bHatsEx3 = 0xFFFFFFFF; // Neutral state
            };
        }
        else
        {
            // Make 5-position POV Hat spin
            
            pov[0] = (byte)(((count / 20) + 0)%4);
            pov[1] = (byte)(((count / 20) + 1) % 4);
            pov[2] = (byte)(((count / 20) + 2) % 4);
            pov[3] = (byte)(((count / 20) + 3) % 4);

            iReport.bHats		= (uint)(pov[3]<<12) | (uint)(pov[2]<<8) | (uint)(pov[1]<<4) | (uint)pov[0];
            if ((count) > 550)
                iReport.bHats = 0xFFFFFFFF; // Neutral state
        };

        /*** Feed the driver with the position packet - is fails then wait for input then try to re-acquire device ***/
        if (!joystick.UpdateVJD(id, ref iReport))
        {
            Console.WriteLine("Feeding vJoy device number {0} failed - try to enable device then press enter\n", id);
            Console.ReadKey(true);
            joystick.AcquireVJD(id);
        }

        System.Threading.Thread.Sleep(20);
        count++;
        if (count > 640) count = 0;

        X += 150; if (X > maxval) X = 0;
        Y += 250; if (Y > maxval) Y = 0;
        Z += 350; if (Z > maxval) Z = 0;
        XR += 220; if (XR > maxval) XR = 0;
        ZR += 200; if (ZR > maxval) ZR = 0;  
         
      }; // While

#endif // EFFICIENT

        } // Main
    } // class Program
}
