using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows Bundle")]
    public static void BundleWindowsBuild()
    {
        BuildAB(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/Build Android Bundle")]
    public static void BundleAndroidBuild()
    {
        BuildAB(BuildTarget.Android);
    }
    [MenuItem("Tools/Build iphone Bundle")]
    public static void BundleIphoneBuild()
    {
        BuildAB(BuildTarget.iOS);
    }
    public static void BuildAB(BuildTarget target)
    {
        List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();

        if (!Directory.Exists(PathUtil.StreamingAssetsPath))
        {
            Directory.CreateDirectory(PathUtil.StreamingAssetsPath);
            Debug.Log("已自动创建 StreamingAssets 文件夹");
        }

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];

            if (filePath.EndsWith(".meta"))
            {
                continue;
            }

            string relativePath = PathUtil.GetUnityPath(filePath);

            // 打包AssetBundleBuild
            AssetBundleBuild build = new AssetBundleBuild();
            // 提取文件名
            string fileName = Path.GetFileNameWithoutExtension(relativePath);
            // 文件名转小写加 .ab后缀
            build.assetBundleName = fileName.ToLower() + ".ab";
            // 把接取好的相对路径塞进资产数组中
            build.assetNames = new string[] { relativePath };

            buildList.Add(build);
            Debug.Log("正在打包资源: " + relativePath);

        }


        BuildPipeline.BuildAssetBundles(
            PathUtil.StreamingAssetsPath,
            buildList.ToArray(),
            BuildAssetBundleOptions.ChunkBasedCompression,
            target
            );

        AssetDatabase.Refresh();
        Debug.Log(" AssetBundle 批量打包完成！");
    }

}
