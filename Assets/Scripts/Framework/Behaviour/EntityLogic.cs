using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Behaviour
{
    public class EntityLogic : LuaBehaviour
    {

        Action m_LuaOnShow;
        Action m_LuaOnHide;
        public override void Init(string luaName)
        {
            base.Init(luaName);
            base.m_ScriptEnv.Get("OnShow", out m_LuaOnShow);
            base.m_ScriptEnv.Get("OnHide", out m_LuaOnHide);
        }

        public void OnShow()
        {
            m_LuaOnShow?.Invoke();
        }

        public void OnHide()
        {
            m_LuaOnHide?.Invoke();
        }

        protected override void Clear()
        {
            base.Clear();
            m_LuaOnShow = null;
            m_LuaOnHide = null;
        }
    }
}
