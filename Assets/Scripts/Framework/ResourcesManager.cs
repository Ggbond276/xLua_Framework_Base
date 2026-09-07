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

    // 解析版本文件的方法
    private void ParseVersionFile()
    {
        // 1. 拼接 FileList.txt 的绝对物理路径
        string url = Path.Combine(Application.streamingAssetsPath, "FileList.txt");
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

    IEnumerator LoadBundleAsync(string assetName, Action<UnityEngine.Object> action)
    {
        BundleInfo info = m_BundleInfos[assetName];
        string bundleName = info.bundleName;
        List<string> dependences = info.Dependences;

        for(int i = 0; i < dependences.Count; i++)
        {
            if (m_LoadedBundle.ContainsKey(dependences[i]))
                continue;

            string depBundleName = dependences[i];
            string depPath = Path.Combine(Application.streamingAssetsPath, depBundleName);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(depPath);
            yield return request;

            m_LoadedBundle.Add(dependences[i], request.assetBundle);
        }

        if (!m_LoadedBundle.ContainsKey(bundleName))
        {
            string path = Path.Combine(Application.streamingAssetsPath, bundleName);
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            yield return request;

            m_LoadedBundle.Add(bundleName, request.assetBundle);
        }


        AssetBundle mainBundle = m_LoadedBundle[bundleName];

        AssetBundleRequest bundleRequest = mainBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;

        action?.Invoke(bundleRequest.asset);

    }


    public void LoadAssets(string assetName, Action<UnityEngine.Object> action)
    {
        StartCoroutine(LoadBundleAsync(assetName, action));
    }
   
}
