using Assets.Scripts.Framework.Manager;
using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework
{
    public static class GameApp
    {

        public static GameObject FrameworkRoot;
        private static GameObject frameworkRoot
        {
            get { return FrameworkRoot; }
            set { FrameworkRoot = value; }
        }

        // 核心黑魔法：在场景加载之前，强行执行此方法
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitFramework()
        {
            AppLog.LogSys("APP", "框架初始化 | 劫持引擎生命周期 | 完成");
            // 创建FrameWork节点
            FrameworkRoot = new GameObject("[Framework_Root]");
            GameObject.DontDestroyOnLoad(FrameworkRoot);
            // 默认在真机上，永远是开启热更的
            bool enableHotUpdate = true;
#if UNITY_EDITOR
            enableHotUpdate = UnityEditor.EditorPrefs.GetBool("IsEnableHotUpdate", true);
#endif
            if (enableHotUpdate)
            {
                AppLog.LogIO("APP", "热更新 | 模式: 启用 | 目标: 云端/沙盒");
                InitHotUpdate();
            }
            else
            {
                AppLog.LogIO("APP", "热更新 | 模式: 禁用 | 目标: 本地资源");
                FrameworkBootstrap.Bootstrap();
            }
        }

        /// <summary>
        /// 初始化热更新
        /// </summary>
        private static void InitHotUpdate()
        {
            AppLog.LogIO("APP", "热更新 | 监听完成信号");
            HotUpdate.OnUpdateComplete += OnHotUpdateDone;
            HotUpdate hotUpdate = frameworkRoot.AddComponent<HotUpdate>();
            hotUpdate.Init();
        }

        /// <summary>
        /// 【核心改动 3】：专门写一个接收热更完毕信号的接力站
        /// </summary>
        private static void OnHotUpdateDone()
        {
            HotUpdate.OnUpdateComplete -= OnHotUpdateDone;
            AppLog.LogIO("APP", "热更新 | 完成 | 转入框架初始化");
            FrameworkBootstrap.Bootstrap();
        }

    }
}
