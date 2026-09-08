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
        // 核心黑魔法：在场景加载之前，强行执行此方法
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitFramwork()
        {
            AppLog.LogSys("GameApp", "引擎生命周期劫持成功，开始装配底层框架...");

            // 1. 凭空造物
            GameObject frameworkRoot = new GameObject("[Framework_Root]");

            // 2. 赋予永生
            GameObject.DontDestroyOnLoad(frameworkRoot);

            // 3. 挂载热更管线控制器
            frameworkRoot.AddComponent<HotUpdate>();

            // （未来你的 NetworkManager、AudioManager 都可以写在这里自动挂载）

            AppLog.LogSys("GameApp", "框架装配完毕！等待 HotUpdate 接管流程...");
        }
    }
}
