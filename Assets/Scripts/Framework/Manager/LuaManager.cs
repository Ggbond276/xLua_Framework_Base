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
    // TODO: 这里的MonoBehaviour 应该做成Mono单例使用的
    public class LuaManager : MonoBehaviour
    {
        /// <summary>
        /// 委托：用来通知外界Lua脚本全部加载到内存中了
        /// </summary>
        public Action OninitComplete;
        // lua这边的容器需要先准备好 ResourcesManager才能准备给我们吐数据
        private List<string> luaNames = new List<string>();
        // 真正缓存内存中的Lua字节码的字典
        private Dictionary<string, byte[]> m_LuaScripts = new Dictionary<string, byte[]>();

        // DEFENSE: 内存泄漏问题 - 静态变量持有了实例方法
        // 架构师剖析：Loader 是 LuaManager 这个类的实例方法（因为它不是 static 的）
        // 当你把它传给静态的 LuaEnv 时，底层的 C# 委托就会死死抓住当前这个 LuaManager 的实例（this 指针）
        // 泄漏后果：因为 LuaEnv 是 static 的，它的生命周期等同于整个游戏的运行时间。
        // 使有一天玩家切换了场景，Unity 把挂载 LuaManager 的 GameObject 给销毁了（触发了 OnDestroy）
        // 但因为静态的 LuaEnv 还抓着 Loader 不放，这个已经被销毁的 LuaManager 实例在内存中永远无法被 GC 回收！ 这就是教科书级别的“委托泄漏”
        public LuaEnv LuaEnv;


        public void Init()
        {
            AppLog.LogDone("LUA", "Lua虚拟机 | 创建完成");
            LuaEnv = new LuaEnv();
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
        // DEFENSE: 内存泄漏问题 - 重型容器和委托没有主动清空
        private void OnDestroy()
        {
            if(luaNames != null)
            {
                luaNames.Clear();
                luaNames = null;
            }

            if(m_LuaScripts != null)
            {
                m_LuaScripts.Clear();
                m_LuaScripts = null;
            }

            OninitComplete = null;

            if (LuaEnv != null)
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
        
        // DEFENSE: 内存泄漏问题 - 异步加载与内存泄漏问题
        // 重点记住如何处理异步与内存的处理问题即可
        public void LoadLuaScript()
        {
            int totalNeedLoad = luaNames.Count;      // 总数
            int loadedCount = 0;                    // 已加载计数
            
            // 1.对大名单进行扫描
            foreach (string name in luaNames)
            {
                // 2. 将大名单中的lua文件全部从硬盘中拉出来
                GameManager.Resources.LoadLua(name, (UnityEngine.Object obj) =>
                {
                    if (this == null || luaNames == null) return;
                    // 加工数据 将TextAsset转化为纯字节数组 存入内存中
                    AddLuaScript(name, (obj as TextAsset).bytes);
                    
                    loadedCount++;                  // 每完成一个递增计数
                    if(loadedCount >= totalNeedLoad) // 等所有都加载完成才触发
                    {
                        luaNames.Clear();
                        AppLog.LogDone("LUA", "脚本预加载 | 完成 | 虚拟机就绪");
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

        public byte[] GetLuaScript(string luaName)
        {
            luaName = luaName.Replace(".", "/");
            // TODO: 现在的资源加载路径是指向
            string fileName = PathUtil.GetLuaPath(luaName);

            byte[] luaScript = null;

            if(!m_LuaScripts.TryGetValue(fileName, out luaScript))
            {
                AppLog.LogError("LUA", $"Lua脚本不存在 | {fileName}");
            }
            return luaScript;
        }

        // ================= 开发者工具模式 (旁路拦截) =================
        public void EditorLoadLuaScript()
        {
            string[] luaFiles = Directory.GetFiles(PathUtil.BuildResourcesLuaPath, "*.bytes", SearchOption.AllDirectories);

            for(int i = 0; i < luaFiles.Length; i++)
            {
                string fileName = PathUtil.GetStandardPath(luaFiles[i]);
                byte[] file = File.ReadAllBytes(fileName);

                AddLuaScript(PathUtil.GetUnityPath(fileName), file);
            }
            AppLog.LogDone("LUA", $"开发者模式 | 加载{luaFiles.Length}个Lua脚本");
            OninitComplete?.Invoke();
        }

    }
}
