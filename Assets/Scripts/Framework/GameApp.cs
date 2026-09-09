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

            // 默认在真机上，永远是开启热更的
            bool enableHotUpdate = true;

#if UNITY_EDITOR
            enableHotUpdate = UnityEditor.EditorPrefs.GetBool("IsEnableHotUpdate", true);
#endif

            if (enableHotUpdate)
            {
                // 精准说明模式和寻址目标
                AppLog.LogSys("GameApp", "[管线] 热更模式: 开启。寻址目标: 云端与沙盒区。");
                InitHotUpdate();
            }
            else
            {
                // 精准说明模式和寻址目标
                AppLog.LogSys("GameApp", "[管线] 热更模式: 关闭 (编辑器环境)。寻址目标: 本地只读光盘区。");
                InitManager();
            }
        }

        /// <summary>
        /// 初始化热更新
        /// </summary>
        private static void InitHotUpdate()
        {
            // 3. 挂载热更管线控制器
            AppLog.LogSys("GameApp", "[事件] 监听热更完成信号...");
            HotUpdate.OnUpdateComplete += OnHotUpdateDone;
            frameworkRoot.AddComponent<HotUpdate>();
        }

        /// <summary>
        /// 【核心改动 3】：专门写一个接收热更完毕信号的接力站
        /// </summary>
        private static void OnHotUpdateDone()
        {
            // 第一步：卸磨杀驴！立刻注销事件，防止未来重复触发导致内存泄漏
            HotUpdate.OnUpdateComplete -= OnHotUpdateDone;

            AppLog.LogSys("GameApp", "[管线] 热更完毕，转入管家系统初始化。");

            // 第二步：在这个绝对安全的时机，才去初始化管家！
            InitManager();
        }

        private static void InitManager()
        {
            frameworkRoot.AddComponent<GameManager>();
            AppLog.LogSys("GameApp", "[初始化] 管家系统装配完毕，进入游戏主逻辑。");
        }

    }
}
