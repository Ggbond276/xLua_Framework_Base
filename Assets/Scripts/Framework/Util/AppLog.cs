using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Util
{
    class AppLog
    {
        // 预定义工业标准色号
        private const string Color_Sys = "#00FFFF";     // 青色 (系统/底层)
        private const string Color_Service = "#00FF00"; // 绿色 (业务/服务)
        private const string Color_IO = "#FFD700";      // 金色 (资源/IO)
        private const string Color_Net = "#FF00FF";     // 品红 (网络/通信)

        /// <summary>
        /// 打印系统/底层逻辑日志
        /// </summary>
        public static void LogSys(string module, string msg)
        {
            Debug.Log($"<color={Color_Sys}>[{module}] {msg}</color>");
        }

        /// <summary>
        /// 打印业务/服务层日志
        /// </summary>
        public static void LogService(string module, string msg)
        {
            Debug.Log($"<color={Color_Service}>[{module}] {msg}</color>");
        }

        /// <summary>
        /// 打印文件读写/资源加载日志
        /// </summary>
        public static void LogIO(string module, string msg)
        {
            Debug.Log($"<color={Color_IO}>[{module}] {msg}</color>");
        }

        /// <summary>
        /// 打印网络请求/数据同步日志
        /// </summary>
        public static void LogNet(string module, string msg)
        {
            Debug.Log($"<color={Color_Net}>[{module}] {msg}</color>");
        }

        // ==========================================
        // 警告与报错：保持原生调用，确保图标正确显示
        // ==========================================
        public static void LogWarning(string module, string msg)
        {
            Debug.LogWarning($"[{module}] {msg}");
        }

        public static void LogError(string module, string msg)
        {
            Debug.LogError($"[{module}] {msg}");
        }
    }
}
