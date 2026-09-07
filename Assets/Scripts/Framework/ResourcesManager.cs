using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    // 增加单例，方便全局访问
    public static ResourcesManager Instance;

    private void Awake()
    {
        // 1. 初始化单例
        Instance = this;
        // 2. 改在 Awake 中解析，确保数据最先就绪
        this.ParseVersionFile();
    }

    internal class BundleInfo
    {
        public string AssetsName;
        public string bundleName;
        public List<string> Dependences;
    }

    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
    private Dictionary<string, AssetBundle> m_LoadedBundle = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 解析版本文件的方法
    /// </summary>
    private void ParseVersionFile()
    {
        // 1. 拼接 FileList.txt 的绝对物理路径
        string url = Path.Combine(PathUtil.BundleResourcesPath, "FileList.txt");
        // 2. 调用 C# 底层 IO 接口，将文本一口气全部读进内存，按行变成数组
        string[] data = File.ReadAllLines(url);
        // 3. 遍历每一行字符串，开始“切洋葱”
        for(int i = 0; i < data.Length; i ++)
        {
            string[] infos = data[i].Split('|');
            BundleInfo bundleInfo = new BundleInfo();
            bundleInfo.AssetsName = infos[0];
            bundleInfo.bundleName = infos[1];
            bundleInfo.Dependences = new List<string>();
            
            for(int j = 2; j < infos.Length; j++)
            {
                bundleInfo.Dependences.Add(infos[j]);
            }

            m_BundleInfos.Add(infos[0], bundleInfo);
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
        for(int i = 0; i < dependences.Count; i++)
        {
            if (m_LoadedBundle.ContainsKey(dependences[i]))
                continue;

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

        if(UnityEditor.EditorPrefs.GetBool("IsEditorLoadMode", true))
        {
            Debug.Log($"<color=yellow>[BuildingResources加载模式] 正在从本地目录直读: {assetName}</color>");

            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetName);

            if (obj == null)
            {
                Debug.LogError($"[BuildingResources加载模式] 找不到资源，请检查路径是否正确: {assetName}");
            }

            action?.Invoke(obj);

            return;

        }
#endif 
        StartCoroutine(LoadBundleAsync(assetName, action));
    }

    /// <summary>
    /// 加载Lua资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public void LoadLua(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAssets(PathUtil.GetLuaPath(assetName), action);
    }

    /// <summary>
    ///  加载UI预制体资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public void LoadUI(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAssets(PathUtil.GetUIPath(assetName), action);
    }

    /// <summary>
    /// 加载音乐资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAssets(PathUtil.GetMusicPath(assetName), action);
    }

    /// <summary>
    /// 加载音效资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public void LoadSound(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAssets(PathUtil.GetSoundPath(assetName), action);
    }

    /// <summary>
    /// 加载特效资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="action"></param>
    public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAssets(PathUtil.GetEffectPath(assetName), action);
    }

}
