using System;
using UnityEngine;
using XLua;

namespace Assets.Scripts.Framework.Util
{
    /// <summary>
    /// 企业级日志系统
    /// 格式: [LEVEL] [MODULE] 操作主体 | 动作 | 状态
    /// </summary>
    [LuaCallCSharp]
    public static class AppLog
    {
        // 工业标准色号
        private const string COLOR_SYS = "#00FFFF";    // 青色 (系统)
        private const string COLOR_SVC = "#00FF00";    // 绿色 (服务)
        private const string COLOR_IO = "#FFD700";     // 金色 (资源)
        private const string COLOR_NET = "#FF00FF";    // 品红 (网络)
        private const string COLOR_ERR = "#FF4444";    // 红色 (错误)
        private const string COLOR_WRN = "#FFA500";    // 橙色 (警告)

        // 级别标签
        private const string LV_INIT = "INIT";
        private const string LV_DONE = "DONE";
        private const string LV_OK = "OK";
        private const string LV_WARN = "WARN";
        private const string LV_ERR = "ERROR";
        private const string LV_STEP = "STEP";

        /// <summary>
        /// 系统初始化日志
        /// </summary>
        public static void LogSys(string module, string msg)
        {
            Debug.Log($"<color={COLOR_SYS}>[{LV_INIT}] [{module}] {msg}</color>");
        }

        /// <summary>
        /// 业务服务日志
        /// </summary>
        public static void LogService(string module, string msg)
        {
            Debug.Log($"<color={COLOR_SVC}>[{LV_OK}] [{module}] {msg}</color>");
        }

        /// <summary>
        /// 资源IO日志
        /// </summary>
        public static void LogIO(string module, string msg)
        {
            Debug.Log($"<color={COLOR_IO}>[{LV_STEP}] [{module}] {msg}</color>");
        }

        /// <summary>
        /// 网络通信日志
        /// </summary>
        public static void LogNet(string module, string msg)
        {
            Debug.Log($"<color={COLOR_NET}>[{LV_STEP}] [{module}] {msg}</color>");
        }

        /// <summary>
        /// 完成状态日志
        /// </summary>
        public static void LogDone(string module, string msg)
        {
            Debug.Log($"<color={COLOR_SVC}>[{LV_DONE}] [{module}] {msg}</color>");
        }

        // ============ 警告与错误 ============
        public static void LogWarning(string module, string msg)
        {
            Debug.LogWarning($"<color={COLOR_WRN}>[{LV_WARN}] [{module}] {msg}</color>");
        }

        public static void LogError(string module, string msg)
        {
            Debug.LogError($"<color={COLOR_ERR}>[{LV_ERR}] [{module}] {msg}</color>");
        }
    }
}
