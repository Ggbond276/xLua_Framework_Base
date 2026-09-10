using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace Assets.Scripts.Framework.Manager
{
    internal class LuaManager : MonoBehaviour
    {
        /// <summary>
        /// 委托：用来通知外界Lua脚本全部加载到内存中了
        /// </summary>
        public Action OninitComplete;
        // lua这边的容器需要先准备好 ResourcesManager才能准备给我们吐数据
        private List<string> luaNames = new List<string>();
        // 真正混村内存中的Lua字节码的字典
        private Dictionary<string, byte[]> m_LuaScripts = new Dictionary<string, byte[]>();
        public LuaEnv LuaEnv;


        public void Init()
        {
            LuaEnv = new LuaEnv();

            // 这里告诉Lua 以后require找不到文件的时候 使用这个Loader拿文件
            LuaEnv.AddLoader(Loader);
        }

        // 开启lua虚拟机的GC垃圾清理
        public void Update()
        {
            if(LuaEnv != null)
            {
                LuaEnv.Tick();
            }
        }

        // 游戏关闭的时候，对虚拟机进行销毁，释放内存
        private void OnDestroy()
        {
            if(LuaEnv != null)
            {
                LuaEnv.Dispose();
                LuaEnv = null;
            }
        }

        public void AddLuaName(string luaName)
        {
            luaNames.Add(luaName);
        }

        // ====================== 将lua ab包从硬盘里读出来 转成字节流 存入容器 ====================
        public void LoadLuaScript()
        {
            // 1.对大名单进行扫描
            foreach(string name in luaNames)
            {
                // 2. 将大名单中的lua文件全部从硬盘中拉出来
                GameManager.Resources.LoadLua(name, (UnityEngine.Object obj) =>
                {
                    // 加工数据 将TextAsset转化为纯字节数组 存入内存中
                    AddLuaScript(name, (obj as TextAsset).bytes);
                    
                    if(m_LuaScripts.Count >= luaNames.Count)
                    {
                        luaNames.Clear();
                        luaNames = null;

                        AppLog.LogSys("LuaManager", "[预加载] 所有 Lua 脚本预加载完成，虚拟机就绪！");


                        OninitComplete?.Invoke();
                    }
                });
            }
        }

        public void AddLuaScript(string assetsName, byte[] luaScript)
        {
            m_LuaScripts[assetsName] = luaScript;
        }

        // ====================== 让Xlua框架使用Require的时候可以读取lua文件 ==========================
        private byte[] Loader(ref string name)
        {
            return GetLuaScript(name);
        }

        public byte[] GetLuaScript(string name)
        {
            name = name.Replace(".", "/");

            string fileName = PathUtil.GetLuaPath(name);

            byte[] luaScript = null;

            if(!m_LuaScripts.TryGetValue(fileName, out luaScript))
            {
                AppLog.LogError("LuaManager", "致命错误：Lua 脚本不存在！试图请求: " + fileName);
            }
            return luaScript;
        }

        // ================= 开发者工具模式 (旁路拦截) =================
#if UNITY_EDITOR
        public void EditorLoadLuaScript()
        {
            string[] luaFiles = Directory.GetFiles(PathUtil.BuildResourcesLuaPath, "*.bytes", SearchOption.AllDirectories);

            for(int i = 0; i < luaFiles.Length; i++)
            {
                string fileName = PathUtil.GetStandardPath(luaFiles[i]);
                byte[] file = File.ReadAllBytes(fileName);

                AddLuaScript(PathUtil.GetUnityPath(fileName), file);
            }
            AppLog.LogSys("LuaManager", $"[开发者模式] 已强读本地 {luaFiles.Length} 个 Lua 脚本完毕！");

            OninitComplete?.Invoke();
        }
#endif
    }
}
