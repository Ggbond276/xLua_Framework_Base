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
        public static MySceneManager Scene
        {
            get { return _scene;  }
        }


        // ============================================================
        // 游戏启动
        // ============================================================
        /// <summary>
        /// Awake什么都不做
        /// </summary>
        private void Awake()
        {
        }

        /// <summary>
        /// FrameworkBootstrp挂载之后 将管理器实例注入给我管理
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="lua"></param>
        /// <param name="ui"></param>
        /// <param name="entity"></param>
        /// <param name="scene"></param>
        public void Inject(ResourcesManager resources, LuaManager lua, UIManager ui, EntityManager entity, MySceneManager scene)
        {
            _resources = resources;
            _lua = lua;
            _ui = ui;
            _entity = entity;
            _scene = scene;
        }

        /// <summary>
        /// 等待基础框架搭建完毕
        /// </summary>
        public void OnFrameworkReady()
        {
            // --------------------------------------------------------
            // 第三阶段：进入 Lua 主流程
            // --------------------------------------------------------

            bool isEditorMode = false;
#if UNITY_EDITOR
            // 现在是编辑模式 全都从buildingResources读取数据 我们现在要将buildingResources里面的数据全部读取到内存里面去
            isEditorMode = EditorPrefs.GetBool("IsEditorLoadMode", true);
#endif
            if (isEditorMode)
            {
                AppLog.LogIO("CORE", "编辑器模式 | 直读本地资源");
                _lua.EditorLoadLuaScript();
                EnterLuaMain();
            }
            else
            {
                AppLog.LogIO("CORE", "运行时模式 | AB包预加载");
                _lua.OninitComplete += EnterLuaMain;
                _lua.LoadLuaScript();
            }
        }

        private void EnterLuaMain()
        {
            AppLog.LogDone("CORE", "Lua虚拟机 | 初始化完成 | 进入主循环");
            _lua.LuaEnv.DoString("require('Main')");
            Action luaMainFunc = _lua.LuaEnv.Global.Get<Action>("Main");
            if(luaMainFunc != null)
            {
                luaMainFunc.Invoke();
            } else
            {
                AppLog.LogError("CORE", "Lua主入口 | 未找到Main函数");
            }
        }
    }
}
