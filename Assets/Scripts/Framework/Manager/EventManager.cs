using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    // TODO: 复习EventManager
    [XLua.LuaCallCSharp]
    public class EventManager : MonoBehaviour
    {
        public delegate void EvnetHandler(object args);

        Dictionary<int, EventHandler> m_Events = new Dictionary<int, EventHandler>();

        public void Init()
        {

        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
        public void Subscribe(int id, EventHandler e)
        {
            if (m_Events.ContainsKey(id))
                m_Events[id] += e;
            else
                m_Events.Add(id, e);
        }

        /// <summary>
        /// 取消事件订阅
        /// </summary>
        /// <param name="id"></param>
        /// <param name="e"></param>
        public void UnSubscribe(int id, EventHandler e)
        {
            if(m_Events.ContainsKey(id))
            {
                if (m_Events[id] != null)
                    m_Events[id] -= e;

                if(m_Events[id] == null)
                    m_Events.Remove(id);
            }
        }
    }
}
