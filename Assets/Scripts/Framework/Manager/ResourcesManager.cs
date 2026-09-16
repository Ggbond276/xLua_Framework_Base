using Assets.Scripts.Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{

    public class ResourcesManager : MonoBehaviour
    {

        // ============================================================
        // 初始化
        // ============================================================

        public void Init()
        {
            bool isEditorMode = false;
#if UNITY_EDITOR
            isEditorMode = EditorPrefs.GetBool("IsEditorLoadMode", false);
#endif
            if (!isEditorMode)
            {
                this.ParseVersionFile();
            }
        }

        // ============================================================
        // 内部数据结构
        // ============================================================

        /// <summary>
        /// Bundle 信息（从 FileList.txt 解析出来）
        /// 记录每个资源对应的 AssetBundle 名称和依赖关系
        /// </summary>
        internal class BundleInfo
        {
            /// <summary>资源名称（用于查询）</summary>
            public string AssetName;
            /// <summary>所属 AssetBundle 名称</summary>
            public string BundleName;
            /// <summary>依赖的 AssetBundle 名称列表</summary>
            public List<string> Dependencies;
        }

        /// <summary>
        /// 已加载的 AssetBundle 记录
        /// 记录内存中的 AssetBundle 及其引用计数
        /// </summary>
        internal class LoadedAssetBundle
        {
            /// <summary>内存中的 AssetBundle 资源</summary>
            public AssetBundle Bundle;
            /// <summary>引用计数器，有人引用就+1，释放就-1</summary>
            public int ReferenceCount;

            public LoadedAssetBundle(AssetBundle bundle)
            {
                Bundle = bundle;
                ReferenceCount = 1;
            }
        }

        // ============================================================
        // 私有成员
        // ============================================================

        /// <summary>
        /// key：资源名称（查询用）  value：Bundle 信息
        /// </summary>
        private Dictionary<string, BundleInfo> m_BundleInfoMap = new Dictionary<string, BundleInfo>();

        /// <summary>
        /// key：AssetBundle 名称  value：已加载的 AssetBundle 记录
        /// 只要引用计数 > 0，Bundle 就会在这里
        /// 引用为 0 时会被 AssetPool 监控，超时后从内存卸载
        /// </summary>
        private Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();

        // ============================================================
        // 生命周期
        // ============================================================

        /// <summary>
        /// 解析版本文件
        /// 格式：资源路径|主包名|依赖包1|依赖包2|...
        /// 示例：Assets/BuildResources/UI/login.prefab|login.ab|common_ui.ab|shader.ab
        /// </summary>
        private void ParseVersionFile()
        {
            string url = Path.Combine(PathUtil.BundleResourcesPath, "FileList.txt");
            string[] lines = File.ReadAllLines(url);

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('|');

                BundleInfo info = new BundleInfo
                {
                    AssetName = parts[0],
                    BundleName = parts[1],
                    Dependencies = new List<string>()
                };

                for (int j = 2; j < parts.Length; j++)
                {
                    info.Dependencies.Add(parts[j]);
                }

                m_BundleInfoMap.Add(parts[0], info);

                // Lua 脚本单独注册到 LuaManager
                if (parts[0].IndexOf("LuaScripts") > 0)
                {
                    GameManager.Lua.AddLuaName(parts[0]);
                }
            }
        }

        // ============================================================
        // 核心加载流程
        // ============================================================

        /// <summary>
        /// 异步加载 AssetBundle 并取出资源
        /// 内部流程：
        /// 1. 加载依赖包
        /// 2. 加载主资源包
        /// 3. 从主包中取出具体资源
        /// </summary>
        private IEnumerator LoadAssetAsync(string assetName, Action<UnityEngine.Object> action)
        {
            BundleInfo info = m_BundleInfoMap[assetName];
            string bundleName = info.BundleName;
            List<string> dependencies = info.Dependencies;

            // ---- Step 1: 加载依赖包 ----
            for (int i = 0; i < dependencies.Count; i++)
            {
                string depName = dependencies[i];
                if (m_LoadedAssetBundles.ContainsKey(depName))
                {
                    continue; // 已加载，跳过
                }

                string depPath = Path.Combine(PathUtil.BundleResourcesPath, depName);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(depPath);
                yield return request;

                m_LoadedAssetBundles.Add(depName, new LoadedAssetBundle(request.assetBundle));
            }

            // ---- Step 2: 加载主资源包 ----
            if (!m_LoadedAssetBundles.ContainsKey(bundleName))
            {
                string path = Path.Combine(PathUtil.BundleResourcesPath, bundleName);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
                yield return request;

                m_LoadedAssetBundles.Add(bundleName, new LoadedAssetBundle(request.assetBundle));
            }

            // ---- Step 3: 从包中取出资源 ----
            AssetBundle mainBundle = m_LoadedAssetBundles[bundleName].Bundle;

            // 场景资源特殊处理：只加载 AB，不激活场景
            if (assetName.Contains("/Scene/") && assetName.EndsWith(".unity"))
            {
                action?.Invoke(null);
                yield break;
            }

            AssetBundleRequest bundleRequest = mainBundle.LoadAssetAsync(assetName);
            yield return bundleRequest;

            // 高亮日志：资源加载完成
            AppLog.LogHighlight("IO", $"★★★ 加载完成 [Asset:{assetName}] ★★★");
            action?.Invoke(bundleRequest.asset);
        }

        // ============================================================
        // 对外接口
        // ============================================================

        /// <summary>
        /// 加载任意资源（编辑器模式走 AssetDatabase，发布模式走 AssetBundle）
        /// </summary>
        public void LoadAssets(string assetName, Action<UnityEngine.Object> action = null)
        {
#if UNITY_EDITOR
            if (EditorPrefs.GetBool("IsEditorLoadMode", true))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
                if (obj == null)
                {
                    AppLog.LogError("IO", $"资源加载失败 | {assetName}");
                }
                action?.Invoke(obj);
                return;
            }
#endif
            StartCoroutine(LoadAssetAsync(assetName, action));
        }

        /// <summary>
        /// 加载 Lua 脚本资源
        /// </summary>
        public void LoadLua(string luaName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(luaName, action);
        }

        /// <summary>
        /// 加载 UI 预制体资源
        /// </summary>
        public void LoadUI(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetUIPath(assetName), action);
        }

        /// <summary>
        /// 加载音乐资源
        /// </summary>
        public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetMusicPath(assetName), action);
        }

        /// <summary>
        /// 加载音效资源
        /// </summary>
        public void LoadSound(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetSoundPath(assetName), action);
        }

        /// <summary>
        /// 加载特效资源
        /// </summary>
        public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetEffectPath(assetName), action);
        }

        /// <summary>
        /// 加载场景资源
        /// </summary>
        public void LoadScene(string sceneName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetScenePath(sceneName), action);
        }

        /// <summary>
        /// 加载模型预制体资源
        /// </summary>
        public void LoadModel(string entityName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetModelPath(entityName), action);
        }

        // ============================================================
        // 引用计数管理（供 AssetPool 和其他 Manager 调用）
        // ============================================================

        /// <summary>
        /// 增加引用计数
        /// 当 AssetPool 或某个 Manager 持有资源时调用
        /// </summary>
        public void AddReference(string assetName)
        {
            if (!m_BundleInfoMap.TryGetValue(assetName, out BundleInfo info))
            {
                return;
            }

            string bundleName = info.BundleName;
            if (m_LoadedAssetBundles.TryGetValue(bundleName, out LoadedAssetBundle lab))
            {
                lab.ReferenceCount++;
            }

            // 依赖包也要增加引用
            for (int i = 0; i < info.Dependencies.Count; i++)
            {
                string depName = info.Dependencies[i];
                if (m_LoadedAssetBundles.TryGetValue(depName, out LoadedAssetBundle depLab))
                {
                    depLab.ReferenceCount++;
                }
            }
        }

        /// <summary>
        /// 减少引用计数
        /// 当 AssetPool 或某个 Manager 释放资源时调用
        /// </summary>
        public void ReleaseReference(string assetName)
        {
            if (!m_BundleInfoMap.TryGetValue(assetName, out BundleInfo info))
            {
                return;
            }

            string bundleName = info.BundleName;
            if (m_LoadedAssetBundles.TryGetValue(bundleName, out LoadedAssetBundle lab))
            {
                lab.ReferenceCount--;
            }

            // 依赖包也要减少引用
            for (int i = 0; i < info.Dependencies.Count; i++)
            {
                string depName = info.Dependencies[i];
                if (m_LoadedAssetBundles.TryGetValue(depName, out LoadedAssetBundle depLab))
                {
                    depLab.ReferenceCount--;
                }
            }
        }

        /// <summary>
        /// 卸载 AssetBundle 资源
        /// 由 AssetPool 超时清理时调用，或手动卸载时调用
        /// </summary>
        public void UnLoadAsset(string assetName)
        {
            if (!m_BundleInfoMap.TryGetValue(assetName, out BundleInfo info))
            {
                return;
            }

            // ---- 卸载主资源包 ----
            string bundleName = info.BundleName;
            if (m_LoadedAssetBundles.TryGetValue(bundleName, out LoadedAssetBundle lab))
            {
                if (lab.Bundle != null)
                {
                    lab.Bundle.Unload(true);
                }
                m_LoadedAssetBundles.Remove(bundleName);
            }

            // ---- 卸载依赖包（仅当引用计数为 0 时）----
            for (int i = 0; i < info.Dependencies.Count; i++)
            {
                string depName = info.Dependencies[i];
                if (m_LoadedAssetBundles.TryGetValue(depName, out LoadedAssetBundle depLab))
                {
                    depLab.ReferenceCount--;
                    if (depLab.ReferenceCount <= 0)
                    {
                        if (depLab.Bundle != null)
                        {
                            depLab.Bundle.Unload(true);
                        }
                        m_LoadedAssetBundles.Remove(depName);
                    }
                }
            }
        }

        /// <summary>
        /// 检查资源是否已加载
        /// </summary>
        public bool IsAssetLoaded(string assetName)
        {
            if (m_BundleInfoMap.TryGetValue(assetName, out BundleInfo info))
            {
                return m_LoadedAssetBundles.ContainsKey(info.BundleName);
            }
            return false;
        }

        /// <summary>
        /// 获取资源的引用计数（调试用）
        /// </summary>
        public int GetReferenceCount(string assetName)
        {
            if (m_BundleInfoMap.TryGetValue(assetName, out BundleInfo info))
            {
                if (m_LoadedAssetBundles.TryGetValue(info.BundleName, out LoadedAssetBundle lab))
                {
                    return lab.ReferenceCount;
                }
            }
            return 0;
        }

    }

}
