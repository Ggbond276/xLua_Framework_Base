using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathUtil
{
    public static readonly string DataPath = Application.dataPath;

    public static readonly string StreamingAssetsPath = Application.streamingAssetsPath;

    public static readonly string BuildResourcesPath = DataPath + "/BuildResources";

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
}
