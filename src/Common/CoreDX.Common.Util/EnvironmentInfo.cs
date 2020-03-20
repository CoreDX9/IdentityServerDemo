using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CoreDX.Common.Util
{
    [Display(Name = "运行信息")]
    public class ApplicationRunInfo
    {
        private double _UsedMem;
        private double _UsedCPUTime;
        public ApplicationRunInfo()
        {
            var proc = Process.GetCurrentProcess();
            var mem = proc.WorkingSet64;
            var cpu = proc.TotalProcessorTime;
            _UsedMem = mem / 1024.0;
            _UsedCPUTime = cpu.TotalMilliseconds;
        }
        [Display(Name = "进程已使用物理内存(kb)")]
        public double UsedMem { get { return _UsedMem; } }
        [Display(Name = "进程已占耗CPU时间(ms)")]
        public double UsedCPUTime { get { return _UsedCPUTime; } }
    }

    [Display(Name = "系统运行平台")]
    public class SystemPlatformInfo
    {
        [Display(Name = "运行框架")]
        public string FrameworkDescription { get { return RuntimeInformation.FrameworkDescription; } }
        [Display(Name = "操作系统")]
        public string OSDescription { get { return RuntimeInformation.OSDescription; } }
        [Display(Name = "操作系统版本")]
        public string OSVersion { get { return Environment.OSVersion.ToString(); } }
        [Display(Name = "操作系统架构")]
        public string OSArchitecture { get { return RuntimeInformation.OSArchitecture.ToString(); } }
        [Display(Name = "进程架构")]
        public string RuntimeArchitecture { get { return RuntimeInformation.ProcessArchitecture.ToString(); } }
    }

    [Display(Name = "运行环境")]
    public class SystemRunEvnInfo
    {
        [Display(Name = "机器名称")]
        public string MachineName { get { return Environment.MachineName; } }
        [Display(Name = "用户网络域名")]
        public string UserDomainName { get { return Environment.UserDomainName; } }
        [Display(Name = "分区磁盘")]
        public string GetLogicalDrives { get { return string.Join(", ", Environment.GetLogicalDrives()); } }
        [Display(Name = "系统目录")]
        public string SystemDirectory { get { return Environment.SystemDirectory; } }
        [Display(Name = "系统已运行时间(毫秒)")]
        public int TickCount { get { return Environment.TickCount; } }

        [Display(Name = "是否在交互模式中运行")]
        public bool UserInteractive { get { return Environment.UserInteractive; } }
        [Display(Name = "当前关联用户名")]
        public string UserName { get { return Environment.UserName; } }
        [Display(Name = "Web程序核心框架版本")]
        public string Version { get { return Environment.Version.ToString(); } }

        //对Linux无效
        [Display(Name = "磁盘分区")]
        public string SystemDrive { get { return Environment.ExpandEnvironmentVariables("%SystemDrive%"); } }
        //对Linux无效
        [Display(Name = "系统目录")]
        public string SystemRoot { get { return Environment.ExpandEnvironmentVariables("%SystemRoot%"); } }
    }

    /// <summary>
    /// 获取程序运行信息的类
    /// </summary>
    public static class EnvironmentInfo
    {
        #region 获取信息
        /// <summary>
        /// 获取程序运行资源信息
        /// </summary>
        /// <returns></returns>
        public static (string, List<KeyValuePair<string, object>>) GetApplicationRunInfo()
        {
            ApplicationRunInfo info = new ApplicationRunInfo();
            return GetValues(info);
        }

        /// <summary>
        /// 获取系统运行平台信息
        /// </summary>
        /// <returns></returns>
        public static (string, List<KeyValuePair<string, object>>) GetSystemPlatformInfo()
        {
            SystemPlatformInfo info = new SystemPlatformInfo();
            return GetValues(info);
        }

        /// <summary>
        /// 获取系统运行环境信息
        /// </summary>
        /// <returns></returns>
        public static (string, List<KeyValuePair<string, object>>) GetSystemRunEvnInfo()
        {
            SystemRunEvnInfo info = new SystemRunEvnInfo();
            return GetValues(info);
        }

        /// <summary>
        /// 获取系统全部环境变量
        /// </summary>
        /// <returns></returns>
        public static (string, List<KeyValuePair<string, object>>) GetEnvironmentVariables()
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry de in environmentVariables)
            {
                list.Add(new KeyValuePair<string, object>(de.Key.ToString(), de.Value));
            }
            return ("系统环境变量", list);
        }

        #endregion

        #region 工具

        /// <summary>
        /// 获取某个类型的值以及名称
        /// </summary>
        /// <typeparam name="TInfo"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        private static (string, List<KeyValuePair<string, object>>) GetValues<TInfo>(TInfo info)
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            Type type = info.GetType();
            PropertyInfo[] pros = type.GetProperties();
            foreach (var item in pros)
            {
                var name = GetDisplayNameValue(item.GetCustomAttributesData());
                var value = GetPropertyInfoValue(item, info);
                list.Add(new KeyValuePair<string, object>(name, value));
            }
            return
                (GetDisplayNameValue(info.GetType().GetCustomAttributesData()),
                list);
        }

        /// <summary>
        /// 获取 [Display] 特性的属性 Name 的值
        /// </summary>
        /// <param name="attrs"></param>
        /// <returns></returns>
        private static string GetDisplayNameValue(IList<CustomAttributeData> attrs)
        {
            var argument = attrs.FirstOrDefault(x => x.AttributeType.Name == nameof(DisplayAttribute)).NamedArguments;
            return argument.FirstOrDefault(x => x.MemberName == nameof(DisplayAttribute.Name)).TypedValue.Value.ToString();
        }

        /// <summary>
        /// 获取属性的值
        /// </summary>
        /// <param name="info"></param>
        /// <param name="obj">实例</param>
        /// <returns></returns>
        private static object GetPropertyInfoValue(PropertyInfo info, object obj)
        {
            return info.GetValue(obj);
        }

        #endregion
    }
}