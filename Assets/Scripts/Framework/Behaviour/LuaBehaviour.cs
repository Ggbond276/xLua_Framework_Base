using Assets.Scripts.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Assets.Scripts.Framework.Behaviour
{
    public class LuaBehaviour : MonoBehaviour
    {
        // 开辟空间
        // 实现双向调用

        // 拿到lua虚拟机 这里在内存中开辟了一片空间
        protected LuaEnv m_LuaEnv = GameManager.Lua.LuaEnv;
        protected LuaTable m_ScriptEnv;
        protected string luaName;

        protected Action m_LuaAwake;
        protected Action m_LuaStart;
        protected Action m_LuaUpdate;
        protected Action m_LuaDestory;



        public virtual void Init(string luaName)
        {
            this.luaName = luaName;

            // 准备好沙盒环境
            m_ScriptEnv = m_LuaEnv.NewTable();
            LuaTable meta = m_LuaEnv.NewTable();
            meta.Set("__index", m_LuaEnv.Global);
            m_ScriptEnv.SetMetaTable(meta);
            meta.Dispose();

            // 将对应的Lua脚本加载进来
            byte[] luaScript = GameManager.Lua.GetLuaScript(luaName);
            // 三个参数 参数1 ：lua脚本文件   参数2 ：lua脚本名字  参数3 ： 运行的沙盒环境
            m_LuaEnv.DoString(luaScript, luaName, m_ScriptEnv);

            // C#把自己扔进去
            m_ScriptEnv.Set("self", this);

            // 这里Get取出的东西是Action类型的委托
            m_ScriptEnv.Get("Awake", out m_LuaAwake);
            m_ScriptEnv.Get("Start", out m_LuaStart);
            m_ScriptEnv.Get("Update", out m_LuaUpdate);
            m_ScriptEnv.Get("OnDestory", out m_LuaDestory);

            m_LuaAwake?.Invoke();
        }


        private void Start()
        {
            m_LuaStart?.Invoke();
        }

        private void Update()
        {
            m_LuaUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            m_LuaDestory?.Invoke();
            Clear();
        }


        // DEFENSE: 内存泄漏问题 - 跨界对象的释放顺序与沙盒销毁
        // NOTE: m_ScriptEnv 是跨界遥控器。必须先调 Dispose() 炸毁 Lua 底层 C 指针（关掉电视），
        //       再将其设为 null 回收 C# 托管堆中的包装类（扔掉遥控器）。缺一不可！
        protected virtual void Clear()
        {
            if (m_ScriptEnv != null)
            {
                m_ScriptEnv.Dispose(); // 第一步：跨域强杀，销毁 Lua 虚拟机中的专属沙盒 Table
                m_ScriptEnv = null;    // 第二步：切断本土强引用，等待 C# GC 自动回收
            }
        }
    }
}
