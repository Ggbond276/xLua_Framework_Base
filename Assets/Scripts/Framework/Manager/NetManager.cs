using Assets.Scripts.Framework.Network;
using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    public class NetManager : MonoBehaviour
    {
        NetClient m_NetClient;
        Queue<KeyValuePair<int, string>> m_MessageQueue = new Queue<KeyValuePair<int, string>>();
        XLua.LuaFunction ReceiveMessage;

        public void Init()
        {
            AppLog.LogDone("NetManager", "初始化成功");
            m_NetClient = new NetClient();
            ReceiveMessage = GameManager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("ReceiveMessage");
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="message"></param>
        public void SendMessage(int messageId, string message)
        {
            m_NetClient.SendMessage(messageId, message);
        }

        public void ConnectedServer(string post, int port)
        {
            m_NetClient.OnConnecServer(post, port);
        }

        /// <summary>
        /// 网络连接
        /// </summary>
        public void OnNetConnected()
        {

        }

        /// <summary>
        ///  服务器断开连接
        /// </summary>
        public void OnDisConnected()
        {

        }

        /// <summary>
        /// 接收到数据
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="message"></param>
        public void Receive(int msgId, string message)
        {
            m_MessageQueue.Enqueue(new KeyValuePair<int, string>(msgId, message));
        }

        private void Update()
        {
            if(m_MessageQueue.Count > 0)
            {
                KeyValuePair<int, string> msg = m_MessageQueue.Dequeue();
                ReceiveMessage?.Call(msg.Key, msg.Value);
            }
        }
    }
}
