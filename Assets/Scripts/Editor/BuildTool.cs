using Assets.Scripts.Framework.Util;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    /// <summary>
    /// 对资源文件打AB包的核心方法
    /// </summary>
    /// <param name="target">目标打包平台 (Windows / Android / iOS)</param>
    public static void BuildAB(BuildTarget target)
    {
        List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();

        // 【新增】用来记录所有文件依赖关系的字符串列表，准备写入 FileList.txt
        List<string> bundleInfos = new List<string>();


        // 1. 创建存放AB包的输出根目录 (StreamingAssets)
        if (!Directory.Exists(PathUtil.StreamingAssetsPath))
        {
            Directory.CreateDirectory(PathUtil.StreamingAssetsPath);
            Debug.Log("已自动创建 StreamingAssets 文件夹");
        }

        // =========================================================================
        // 2. 对 BuildResources 目录下的所有文件进行深度递归扫描
        // [示例输入] 返回的绝对路径如: 
        // "C:/Developer/xLua_Framework_Base/Assets/BuildResources/UI/Prefabs/TestUI.prefab"
        // =========================================================================
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);


        for (int i = 0; i < files.Length; i++)
        {
            string filePath = files[i];

            // 过滤掉 Unity 编辑器自动生成的 .meta 杂质文件
            if (filePath.EndsWith(".meta"))
            {
                continue;
            }

            // =========================================================================
            // 3. 路径整形：将绝对盘符路径裁剪为以 Assets/ 开头的标准 Unity 相对路径
            // [示例输出] relativePath 变为: 
            // "Assets/BuildResources/UI/Prefabs/TestUI.prefab"
            // =========================================================================
            string relativePath = PathUtil.GetUnityPath(filePath);

            // 初始化当前文件的 AssetBundle 构建结构体
            AssetBundleBuild build = new AssetBundleBuild();

            // =========================================================================
            // 4. 提取无后缀的文件名
            // [示例输出] fileName 变为: "TestUI"
            // =========================================================================
            string fileName = Path.GetFileNameWithoutExtension(relativePath);
            string bundleName = fileName.ToLower() + ".ab";


            // =========================================================================
            // 5. 格式化 AB 包名：将文件名转为全小写，并拼接 .ab 后缀
            // [示例输出] build.assetBundleName 变为: "testui.ab"
            // =========================================================================
            build.assetBundleName = bundleName;

            // 将裁剪好的 Unity 相对路径放入资产数组中
            // [示例内容] build.assetNames = ["Assets/BuildResources/UI/Prefabs/TestUI.prefab"]
            build.assetNames = new string[] { relativePath };

            buildList.Add(build);
            Debug.Log("正在打包资源: " + relativePath);


            // =========================================================================
            // 【核心修复区】：开始查户口，建立依赖档案！
            // [上下文假设] 
            // relativePath 当前为: "Assets/BuildResources/UI/Prefabs/TestUI.prefab"
            // bundleName 当前为: "testui.ab"
            // =========================================================================

            // =========================================================================
            // 1. 获取当前文件依赖的所有小弟 (通过自定义方法查重并转换包名)
            // [示例输出] dependenceInfo 列表变为: 
            // ["background.ab", "button_150.ab"]
            // =========================================================================
            List<string> dependenceInfo = GetDependence(relativePath);

            // =========================================================================
            // 2. 拼接当前文件的基础信息：格式为 "相对路径|自己的包名"
            // [示例输出] bundleInfo 变为: 
            // "Assets/BuildResources/UI/Prefabs/TestUI.prefab|testui.ab"
            // =========================================================================
            string bundleInfo = relativePath + "|" + bundleName;

            // =========================================================================
            // 3. 如果有小弟，就把小弟的包名也拼在后面，用 '|' 隔开
            // [示例输出] 经过 string.Join 拼接后，bundleInfo 最终变为: 
            // "Assets/BuildResources/UI/Prefabs/TestUI.prefab|testui.ab|background.ab|button_150.ab"
            // =========================================================================
            if (dependenceInfo.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo);
            }

            // =========================================================================
            // 4. 将拼接好的一行完整记录，塞入全局的档案列表
            // [最终作用] 等整个 for 循环结束后，这个列表会被一口气写进 FileList.txt 中
            // =========================================================================
            bundleInfos.Add(bundleInfo);

            Debug.Log($"[打包映射] 原始文件: {relativePath} ==> 目标AB包: {bundleName}");
        }

        // ==========================================================
        // 【新增】：将档案列表真正写入到本地硬盘的 FileList.txt 中
        // ==========================================================
        string fileListPath = PathUtil.StreamingAssetsPath + "/FileList.txt";
        File.WriteAllLines(fileListPath, bundleInfos);
        Debug.Log(" 依赖档案 FileList.txt 生成成功！");


        BuildPipeline.BuildAssetBundles(
            PathUtil.StreamingAssetsPath,
            buildList.ToArray(),
            BuildAssetBundleOptions.ChunkBasedCompression,
            target
            );

        AssetDatabase.Refresh();
        Debug.Log(" AssetBundle 批量打包完成！");
    }

    /// <summary>
    /// 【新增方法】获取指定资源依赖的所有 AB 包名
    /// </summary>
    static List<string> GetDependence(string curFile)
    {
        List<string> dependence = new List<string>();

        // 查出所有依赖路径
        string[] files = AssetDatabase.GetDependencies(curFile);

        // 使用 Linq 过滤掉脚本文件(.cs) 以及 资源本身
        var validFiles = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();

        foreach (string file in validFiles)
        {
            // 将依赖的相对路径，也转换成对应的 AB 包名 (全小写 + .ab)
            string depFileName = Path.GetFileNameWithoutExtension(file);
            string depBundleName = depFileName.ToLower() + ".ab";
            dependence.Add(depBundleName);
        }

        return dependence;
    }

}
