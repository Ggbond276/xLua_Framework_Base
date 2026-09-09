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

        private static GameObject frameworkRoot = null;

        // 核心黑魔法：在场景加载之前，强行执行此方法
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitFramwork()
        {
            AppLog.LogSys("GameApp", "引擎生命周期劫持成功，开始装配底层框架...");
            frameworkRoot = new GameObject("[Framework_Root]");
            GameObject.DontDestroyOnLoad(frameworkRoot);

            // 初始化入口 只有热更新结束之后才能加载Manager
            InitHotUpdate();
        }

        /// <summary>
        /// 初始化热更新
        /// </summary>
        private static void InitHotUpdate()
        {
            // 3. 挂载热更管线控制器
            frameworkRoot.AddComponent<HotUpdate>();

            // （未来你的 NetworkManager、AudioManager 都可以写在这里自动挂载）

            AppLog.LogSys("GameApp", "框架装配完毕！等待 HotUpdate 接管流程...");
        }

        /// <summary>
        /// 【核心改动 3】：专门写一个接收热更完毕信号的接力站
        /// </summary>
        private static void OnHotUpdateDone()
        {
            // 第一步：卸磨杀驴！立刻注销事件，防止未来重复触发导致内存泄漏
            HotUpdate.OnUpdateComplete -= OnHotUpdateDone;

            AppLog.LogSys("GameApp", "收到热更管线完工信号！接力棒交回主框架，开始装配 GameManager...");

            // 第二步：在这个绝对安全的时机，才去初始化管家！
            InitManager();
        }

        private static void InitManager()
        {
            frameworkRoot.AddComponent<GameManager>();
            AppLog.LogSys("GameApp", "框架装配彻底完毕！正式进入游戏！");
        }

    }
}
