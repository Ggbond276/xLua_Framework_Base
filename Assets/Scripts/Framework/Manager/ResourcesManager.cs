using Assets.Scripts.Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


namespace Assets.Scripts.Framework.Manager
{

    public class ResourcesManager : MonoBehaviour
    {

        // 自己调用初始化方法
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

        internal class BundleInfo
        {
            public string AssetsName;
            public string bundleName;
            public List<string> Dependences;
        }
        /// <summary>
        /// key：ab包的地址信息  value：1.ab包的名称，2.ab包的依赖包的名称列表
        /// </summary>
        private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
        /// <summary>
        /// key：ab包的名称 value：ab包的真正的内存资源
        /// </summary>
        private Dictionary<string, AssetBundle> m_LoadedBundle = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 这个方法的作用就是将FileList这个文件列表中的信息全部变成对象存入内存中
        /// 解析版本文件的方法 Assets/BuildResources/UI/login.prefab|login.ab|common_ui.ab|shader.ab
        /// 文件路径："Assets/BuildResources/UI/login.prefab"
        /// 主包名："login.ab"
        /// 依赖包名："common_ui.ab"
        /// 依赖包名："shader.ab"
        /// </summary>
        private void ParseVersionFile()
        {
            // 1. 拼接 FileList.txt 的绝对物理路径
            string url = Path.Combine(PathUtil.BundleResourcesPath, "FileList.txt");
            // 2. 调用 C# 底层 IO 接口，将文本一口气全部读进内存，按行变成数组
            string[] data = File.ReadAllLines(url);
            // 3. 遍历每一行字符串，开始“切洋葱”
            for (int i = 0; i < data.Length; i++)
            {
                // infos[] 里面包含的是拆解出来的所有文件信息
                string[] infos = data[i].Split('|');
                // 这里就是将信息进行组装存储
                BundleInfo bundleInfo = new BundleInfo();
                bundleInfo.AssetsName = infos[0];
                bundleInfo.bundleName = infos[1];
                bundleInfo.Dependences = new List<string>();
                for (int j = 2; j < infos.Length; j++)
                {
                    bundleInfo.Dependences.Add(infos[j]);
                }

                m_BundleInfos.Add(infos[0], bundleInfo);


                // 如果我们找到了Lua的脚本文件就需要弄一份放到LuaManager中
                if (infos[0].IndexOf("LuaScripts") > 0)
                {
                    GameManager.Lua.AddLuaName(infos[0]);
                }
            }

        }

        /// <summary>
        /// 异步加载Bundle的方法
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> action = null)
        {
            BundleInfo info = m_BundleInfos[assetName];
            string bundleName = info.bundleName;
            List<string> dependences = info.Dependences;

            // 加载依赖包
            for (int i = 0; i < dependences.Count; i++)
            {
                if (m_LoadedBundle.ContainsKey(dependences[i]))
                    continue;

                // 从硬盘中将资源弄到硬盘仓库中 由于从硬盘里面读资源是非常消耗时间的事情 所以要使用协程挂起
                string depBundleName = dependences[i];
                string depPath = Path.Combine(PathUtil.BundleResourcesPath, depBundleName);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(depPath);
                yield return request;

                m_LoadedBundle.Add(dependences[i], request.assetBundle);
            }

            // 加载主资源包
            if (!m_LoadedBundle.ContainsKey(bundleName))
            {
                string path = Path.Combine(PathUtil.BundleResourcesPath, bundleName);
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
                yield return request;

                m_LoadedBundle.Add(bundleName, request.assetBundle);
            }

            // 从内存里面读取资源
            AssetBundle mainBundle = m_LoadedBundle[bundleName];
            AssetBundleRequest bundleRequest = mainBundle.LoadAssetAsync(assetName);
            yield return bundleRequest;

            action?.Invoke(bundleRequest.asset);

        }

        /// <summary>
        /// 加载Bundle的方法
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        public void LoadAssets(string assetName, Action<UnityEngine.Object> action = null)
        {


#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.GetBool("IsEditorLoadMode", true))
            {
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);
                if (obj == null)
                {
                    AppLog.LogError("IO", $"资源加载失败 | {assetName}");
                }
                action?.Invoke(obj);
                return;
            }
#endif
            // 这里是正常流程从Streaming进行数据读取
            StartCoroutine(LoadBundleAsync(assetName, action));
        }


        // IMPORTANT: LoadLua是异步方法
        /// <summary>
        /// 加载Lua资源
        /// </summary>
        /// <param name="luaName"></param>
        /// <param name="action"></param>
        public void LoadLua(string luaName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetLuaPath(luaName), action);
        }

        // IMPORTANT: LoadUI是异步方法
        /// <summary>
        ///  加载UI预制体资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        public void LoadUI(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetUIPath(assetName), action);
        }

        // IMPORTANT: LoadMusic是异步方法
        /// <summary>
        /// 加载音乐资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetMusicPath(assetName), action);
        }

        // IMPORTANT: LoadSound是异步方法
        /// <summary>
        /// 加载音效资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        public void LoadSound(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetSoundPath(assetName), action);
        }

        // IMPORTANT: LoadEffect是异步方法
        /// <summary>
        /// 加载特效资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="action"></param>
        public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetEffectPath(assetName), action);
        }

        public void LoadScene(string sceneName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetScenePath(sceneName), action);
        }

        // 加载预制体
        public void LoadModel(string entityName, Action<UnityEngine.Object> action = null)
        {
            LoadAssets(PathUtil.GetModelPath(entityName), action);
        }

    }

}