using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XLua;

namespace Assets.Scripts.Framework.Manager
{
    [LuaCallCSharp]
    public class GameManager : MonoBehaviour
    {
        // 1. 点击开始之后 热更新加载完毕 GameManager脚本就会被挂载到Root并走初始化流程
        // 2. 然后就是ResourcesManager被挂载并初始化
        // 3. 然后就是LuaManager被挂载并初始化

        // ============================================================
        // 全局管理器引用
        // ============================================================

        private static ResourcesManager _resources;
        public static ResourcesManager Resources
        {
            get { return _resources;  }
        }


        private static LuaManager _lua;
        public static LuaManager Lua
        {
            get { return _lua; }
        }


        private static UIManager _ui;
        public static UIManager UI
        {
            get { return _ui; }
        }


        private static EntityManager _entity;
        public static EntityManager Entity
        {
            get { return _entity; }
        }

        private static MySceneManager _scene;
        private static MySceneManager Scene
        {
            get { return _scene;  }
        }


        // ============================================================
        // 游戏启动
        // ============================================================
        /// <summary>
        /// Awake
        /// LuaManager初始化 （加载出Lua文件名单，和Lua脚本到内存中）
        /// ResourcesManager初始化（加载出资源文件名单）
        /// </summary>
        private void Awake()
        {
            // 对于Unity脚本 我们可以直接将 挂载 = 初始化
            // 这个时候大家都做初始化工作 都不允许干活！

            // --------------------------------------------------------
            // 第一阶段：创建所有核心管理器
            // --------------------------------------------------------
            _resources = this.gameObject.AddComponent<ResourcesManager>();
            _lua = this.gameObject.AddComponent<LuaManager>();
            _ui = this.gameObject.AddComponent<UIManager>();
            _entity = this.gameObject.AddComponent<EntityManager>();
            _scene = this.gameObject.AddComponent<MySceneManager>();

            // 为什么是lua先初始化呢 因为_resources在初始化的时候会给_lua吐数据
            // 所以我们需要_lua先准备好容器准备接收

            // --------------------------------------------------------
            // 第二阶段：初始化管理器
            //
            // Lua 必须先初始化，因为 ResourcesManager
            // 后面会向 LuaManager 提供 Lua 资源。
            // --------------------------------------------------------
            _lua.Init();
            _resources.Init();



            // --------------------------------------------------------
            // 第三阶段：进入 Lua 主流程
            // --------------------------------------------------------

            bool isEditorMode = false;
#if UNITY_EDITOR
            // 现在是编辑模式 全都从buildingResources读取数据 我们现在要将buildingResources里面的数据全部读取到内存里面去
            isEditorMode = EditorPrefs.GetBool("isEditorMode", true);
#endif
            if(isEditorMode)
            {
                // 开发模式下 从buildingResources将脚本读入内存中
                AppLog.LogSys("GameManager", "[管线] 直读模式：抛弃清单，直接暴力扫盘...");
                _lua.EditorLoadLuaScript();


                // 扫盘不是异步加载 是同步的
                EnterLuaMain();
            } else
            {
                AppLog.LogSys("GameManager", "[管线] AB包模式：按清单预加载 Lua 资源...");
                // 走世界线 A：这是你漏掉的极其致命的一步！
                // 必须呼叫 LuaManager 根据刚刚 Resources 吐出来的清单去读 AB 包！

                _lua.OninitComplete += EnterLuaMain;

                _lua.LoadLuaScript();
            }
            
        }

        // ======================== 接下来运行lua脚本 ==========================

        // 在这里王权正式产生交接，整个游戏的所有的主流程全部移交Lua进行，lua会编写整个程序的所有业务流程
        // 从这里开始就实现了Lua对于整个Unity客户端的全部操控
        private void EnterLuaMain()
        {
            AppLog.LogSys("GameManager", "框架底层全部就绪，准备进入 Lua 主循环...");

            _lua.LuaEnv.DoString("require('Main')");

            Action luaMainFunc = _lua.LuaEnv.Global.Get<Action>("Main");

            if(luaMainFunc != null)
            {
                luaMainFunc.Invoke();
            } else
            {
                AppLog.LogError("GameManager", "致命错误：在 Lua 中找不到 Main 函数！");
            }
        }
    }
}
