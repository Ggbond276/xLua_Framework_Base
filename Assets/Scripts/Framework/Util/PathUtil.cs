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

        // 判断是否是热更新模式
        public static bool IsOnlineUpdateMode = false;

        // 获取资源加载路径
        public static string BundleResourcesPath
        {
            get
            {
                if (IsOnlineUpdateMode)
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