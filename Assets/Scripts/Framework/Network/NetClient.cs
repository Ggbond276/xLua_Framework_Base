using Assets.Scripts.Framework.Manager;
using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Framework.Network
{
    public class NetClient
    {
        private TcpClient m_Client;
        private NetworkStream m_TcpStream;
        private const int BufferSize = 1024 * 64;
        private byte[] m_Buffer = new byte[BufferSize];
        private MemoryStream m_MemStream;
        private BinaryReader m_BinaryReader;

        public NetClient()
        {
            m_MemStream = new MemoryStream();
            m_BinaryReader = new BinaryReader(m_MemStream);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口号</param>
        public void OnConnecServer(string host, int port)
        {
            try
            {
                IPAddress[] addresses = Dns.GetHostAddresses(host);
                if(addresses.Length == 0)
                {
                    AppLog.LogNet("NetClient", "host invalid");
                    return;
                }
                if (addresses[0].AddressFamily == AddressFamily.InterNetworkV6) // 判断网络环境是不是IPV6
                    m_Client = new TcpClient(AddressFamily.InterNetworkV6); // 如果是IPV6就创建一个IPV6的TcpClient
                else
                    m_Client = new TcpClient(AddressFamily.InterNetwork); // 如果是IPV4就创建一个IPV4的TCPClient
                m_Client.SendTimeout = 1000;
                m_Client.ReceiveTimeout = 1000;
                m_Client.NoDelay = true;
                m_Client.BeginConnect(host, port, OnConnect, null);
            }
            catch (Exception e)
            {

                AppLog.LogError("NetClient", e.Message);
            }
        }

        private void OnConnect(IAsyncResult asyncResult)
        {
            if(m_Client == null || !m_Client.Connected)
            {
                AppLog.LogError("NetClient", "connect server error!!!");
                return;
            }

            m_TcpStream = m_Client.GetStream();
            m_TcpStream.BeginRead(m_Buffer, 0, BufferSize, OnRead, null); // OnRead是一个异步回调
        }

        private void OnRead(IAsyncResult async)
        {
            try
            {
                if (m_Client == null || m_TcpStream == null)
                    return;
                if(m_Buffer.Length < 1)
                {
                    OnDisConnected();
                    return;
                }
                ReceiveData();
                lock (m_TcpStream)
                {
                    Array.Clear(m_Buffer, 0, m_Buffer.Length);
                    m_TcpStream.BeginRead(m_Buffer, 0, BufferSize, OnRead, null);
                }
            } catch (Exception e)
            {
                AppLog.LogError("NetClient", e.Message);
                OnDisConnected();
            }
        }



        private void ReceiveData()
        {
            m_MemStream.Seek(0, SeekOrigin.End);
            m_MemStream.Write(m_Buffer, 0, m_Buffer.Length);
            m_MemStream.Seek(0, SeekOrigin.Begin);
            while(RemainingBytesLength() > 8)
            {
                int msgId = m_BinaryReader.ReadInt32();
                int msgLen = m_BinaryReader.ReadInt32();
                if(RemainingBytesLength() >= msgLen)
                {
                    byte[] data = m_BinaryReader.ReadBytes(msgLen);
                    string message = System.Text.Encoding.UTF8.GetString(data);

                    // 转到Lua 先注释 之后写了再开放
                    GameManager.Net.Receive(msgId, message);
                }
                else
                {
                    m_MemStream.Position = m_MemStream.Position - 8;
                    break;
                }
            }
        }

        private int RemainingBytesLength()
        {
            return (int)(m_MemStream.Length - m_MemStream.Position);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgID"></param>
        /// <param name="message"></param>
        public void SendMessage(int msgID, string message)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Position = 0;
                BinaryWriter bw = new BinaryWriter(ms);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                bw.Write(msgID);
                bw.Write((int)data.Length);
                bw.Write(data);
                bw.Flush();
                if (m_Client != null && m_Client.Connected)
                {
                    byte[] sendData = ms.ToArray();
                    m_TcpStream.BeginWrite(sendData, 0, sendData.Length, OnEndSend, null);
                }
                else
                {
                    AppLog.LogError("NetClient", "服务器未连接");
                }
            }
        }

        private void OnEndSend(IAsyncResult ar)
        {
            try
            {
                m_TcpStream.EndWrite(ar);
            }
            catch (Exception ex)
            {

                OnDisConnected();
                AppLog.LogError("NetClient", ex.Message);
            }
        }

        private void OnDisConnected()
        {
            if(m_Client != null && m_Client.Connected)
            {
                m_Client.Close();
                m_Client = null;

                m_TcpStream.Close();
                m_TcpStream = null;
            }
            GameManager.Net.OnDisConnected();
        }
    }
}
