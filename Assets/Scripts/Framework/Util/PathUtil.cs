using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Framework.Util
{
    public static class PathUtil
    {
        public static readonly string DataPath = Application.dataPath;

        public static readonly string StreamingAssetsPath = Application.streamingAssetsPath;

        public static readonly string BuildResourcesPath = DataPath + "/BuildResources";

        public static readonly string BuildResourcesLuaPath = DataPath + "/BuildResources/LuaScripts";

        // ==============================================================================
        // TODO: 【架构优化补丁】当前的热更存储策略属于“偷懒的暴力全量拷贝”版本。
        // 
        // [当前痛点]：
        // 现阶段只要触发热更，就会把只读区 (StreamingAssets) 的原始包全量拷贝到沙盒区。
        // 这会导致游戏首包在玩家手机里的硬盘占用瞬间翻倍（极度消耗存储空间），且拷贝过程极慢。
        // 
        // [未来强化方向 - Smart Routing (智能路由双轨制)]：
        // 1. 废除首包全量拷贝机制，只读区资源原封不动。
        // 2. 将下方的沙盒就绪开关，升级为“按需寻址器”：
        //    - 当业务层请求资源时，优先去沙盒区 (persistentDataPath) 查找是否有最新的增量热更补丁。
        //    - 如果沙盒里没有，则 Fallback（自动回退）去只读区 (StreamingAssets) 读取出厂包。
        // ==============================================================================

        // 判断是否是热更新模式
        public static bool IsSandboxReady = false;

        // 获取资源加载路径
        public static string BundleResourcesPath
        {
            get
            {
                // 补丁：未来这里不再是简单的 if-else，而应该传入 assetName，进行真实的文件存在性校验
                if (IsSandboxReady)
                {
                    return Application.persistentDataPath;
                }
                return Application.streamingAssetsPath;
            }
        }


        public static string GetStandardPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            return path.Replace("\\", "/");
        }

        public static string GetUnityPath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return string.Empty;

            string standardPath = GetStandardPath(absolutePath);
            int assetIndex = standardPath.IndexOf("Assets/");
            if (assetIndex == -1) return standardPath;

            return standardPath.Substring(assetIndex);
        }


        /// <summary>
        /// 获取lua脚本路径（这些路径本质其实都是bundle包的标签）
        /// </summary>
        /// <param name="name">Lua脚本的名称</param>
        /// <returns></returns>
        public static string GetLuaPath(string name)
        {
            return string.Format("Assets/BuildResources/LuaScripts/{0}.bytes", name);
        }

        /// <summary>
        /// 获取UI预制体路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetUIPath(string name)
        {
            return string.Format("Assets/BuildResources/UI/Prefab/{0}.prefab", name);
        }

        /// <summary>
        /// 获取音乐路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetMusicPath(string name)
        {
            return string.Format("Assets/BuildResources/Audio/Music/{0}.mp3", name);
        }

        /// <summary>
        /// 获取音效路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSoundPath(string name)
        {
            return string.Format("Assets/BuildResources/Audio/Sound/{0}.mp3", name);
        }

        /// <summary>
        /// 获取特效预制体路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetEffectPath(string name)
        {
            return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
        }
    }
}