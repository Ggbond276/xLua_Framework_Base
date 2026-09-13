using Assets.Scripts.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Behaviour
{
    // 这个相当于是继承了LuaBehaviour
    public class UILogic : LuaBehaviour
    {
        // 也就是说UILogic有
        // Init(string luaName) 方法
        // 有各种生命周期方法
        // 我们只需要在这个基础上进行新增即可
        // 增添一些特有的触发方法 比如Open和Close即可

        // 这里你可以直接理解为这就是lua里面有的方法
        private Action m_LuaOpen;
        private Action m_LuaClose;

        /// <summary>
        /// 传入lua脚本的名字，实现lua脚本和UILogic的双向调用
        /// </summary>
        /// <param name="luaName"></param>
        public override void Init(string luaName)
        {
            base.Init(luaName);

            base.m_ScriptEnv.Get("OnOpen", out m_LuaOpen);
            base.m_ScriptEnv.Get("OnClose", out m_LuaClose);
        }

        private void Start()
        {
            base.m_LuaStart?.Invoke();
            m_LuaOpen?.Invoke();
        }



        // ================================================================
        // 【下行】UIManager → UILogic → Lua
        // ================================================================

        /// <summary>
        /// UIManager 调,通知 Lua:"你被打开了"
        /// 翻译官转发给 Lua:OnOpen()
        /// </summary>
        public void OnOpen()
        {
            m_LuaOpen?.Invoke();
        }

        /// <summary>
        /// UIManager 调,通知 Lua:"你被关闭了"
        /// 翻译官转发给 Lua:OnClose()
        /// </summary>
        public void OnClose()
        {
            m_LuaClose?.Invoke();
        }

        // ================================================================
        // 【上行】Lua → UILogic → UIManager
        // ================================================================

        /// <summary>
        /// Lua 调:关闭自己
        /// 调用链: UIShop.lua → self:Close() → 这里 → UIManager.CloseUI()
        /// </summary>
        public void Close()
        {
            string uiName = gameObject.name;
            GameManager.UI.CloseUI(uiName);
        }

        protected override void Clear()
        {
            base.Clear();
            m_LuaOpen = null;
            m_LuaClose = null;
        }
     }
}
