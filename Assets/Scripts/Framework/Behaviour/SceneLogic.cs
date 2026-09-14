using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Behaviour
{
    public class SceneLogic : LuaBehaviour
    {
        public string SceneName;

        // 上行 从C# -> Lua
        Action m_LuaActive;
        Action m_LuaInActive;
        Action m_LuaOnEnter;
        Action m_LuaOnQuit;

        public override void Init(string luaName)
        {
            base.Init(luaName);
            // 场景在缓存中的时候使用
            m_ScriptEnv.Get("OnActive", out m_LuaActive);
            m_ScriptEnv.Get("OnInActive", out m_LuaInActive);
            // 场景不在缓存中的时候使用
            m_ScriptEnv.Get("OnEnter", out m_LuaOnEnter);
            m_ScriptEnv.Get("OnQuit", out m_LuaOnQuit);
        }


        public void OnActive()
        {
            m_LuaActive?.Invoke();
        }

        public void OnInAvtice()
        {
            m_LuaInActive?.Invoke();
        }

        public void OnEnter()
        {
            m_LuaOnEnter?.Invoke();
        }

        public void OnQuit()
        {
            m_LuaOnQuit?.Invoke();
        }

        protected override void Clear()
        {
            base.Clear();
            m_LuaInActive = null;
            m_LuaActive = null;
            m_LuaOnEnter = null;
            m_LuaOnQuit = null;
        }
    }
    
}
