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
    /// <summary>
    /// Framework 启动引导器
    /// 负责创建 FrameworkRoot、Systems、Runtime 基础层级
    /// 并按顺序挂载/初始化所有 Manager
    /// 最后通知 GameManager 进入 Lua Main
    /// </summary>
    public static class FrameworkBootstrap
    {

        private static GameObject _frameworkRoot;
        private static GameObject _systems;
        private static GameObject _runtime;

        /// <summary>
        /// 对外暴露 FrameworkRoot
        /// </summary>
        public static GameObject FrameworkRoot => _frameworkRoot;
        /// <summary>
        /// 对外暴露 Systems 节点(挂载 Manager 的容器)
        /// </summary>
        public static GameObject Systems => _systems;
        /// <summary>
        /// 对外暴露 Runtime 节点(运行时对象的容器)
        /// </summary>
        public static GameObject Runtime => _runtime;

        public static void Bootstrap()
        {
            AppLog.LogSys("BOOT", "框架启动 | 阶段1/4 | 构建层级结构");


            // 第一阶段:创建基础层级
            _frameworkRoot = GameApp.FrameworkRoot;
            _systems = new GameObject("Systems"); // 建 Systems + Runtime 这两个空节点
            _runtime = new GameObject("Runtime");
            _systems.transform.SetParent(_frameworkRoot.transform);
            _runtime.transform.SetParent(_frameworkRoot.transform);

            AppLog.LogIO("BOOT", "层级结构 | Root/Systems/Runtime | 创建完成");


            // 第二阶段:挂载所有 Manager 到 Systems
            PoolManager pool = _systems.AddComponent<PoolManager>(); // 将对象池的优先级提升到最高
            ResourcesManager resources = _systems.AddComponent<ResourcesManager>(); // 挂 5 个 Manager 到 Systems
            LuaManager lua = _systems.AddComponent<LuaManager>();
            UIManager ui = _systems.AddComponent<UIManager>();
            EntityManager entity = _systems.AddComponent<EntityManager>();
            MySceneManager scene = _systems.AddComponent<MySceneManager>();
            SoundManager sound = _systems.AddComponent<SoundManager>();
            EventManager myEvent = _systems.AddComponent<EventManager>();
            GameManager game = _frameworkRoot.AddComponent<GameManager>();
        

            AppLog.LogIO("BOOT", "管理器 | 注册完成 | Resources/Lua/UI/Entity/Scene");

            // 第三阶段:注入引用
            game.Inject(resources, lua, ui, entity, scene, sound, myEvent, pool);

            // 第四阶段:按顺序初始化(注意依赖关系)
            pool.Init();
            lua.Init();
            resources.Init();
            entity.Init();
            ui.Init();
            scene.Init();
            sound.Init();
            myEvent.Init();


            AppLog.LogDone("BOOT", "框架启动 | 阶段4/4 | 所有管理器初始化完成");
            game.OnFrameworkReady();
        }
    }
}
