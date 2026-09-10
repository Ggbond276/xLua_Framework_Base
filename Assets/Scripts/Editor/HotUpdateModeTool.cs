using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class HotUpdateModeTool : Editor
{
    private const string PrefsKey = "IsEnableHotUpdate";

    [MenuItem("Tools/切换 热更新管线开关")]
    public static void ToggleMode()
    {
        bool isEnable = EditorPrefs.GetBool(PrefsKey, true);
        EditorPrefs.SetBool(PrefsKey, !isEnable);

        bool currentState = EditorPrefs.GetBool(PrefsKey, true);

        // 核心诉求：精确指明数据的读取去向
        string targetPath = currentState ? "沙盒区 (PersistentDataPath)" : "只读光盘区 (StreamingAssets)";

        // 极简工业风 Log (中文版)
        Debug.Log($"<color=yellow>[工具] 热更管线: {(currentState ? "已开启" : "已关闭")} | 资源读取路径: {targetPath}</color>");
    }

    [MenuItem("Tools/Framework/切换 热更新管线开关", true)]
    public static bool ToggleModeValidate()
    {
        bool isEnable = EditorPrefs.GetBool(PrefsKey, true);
        Menu.SetChecked("Tools/Framework/切换 热更新管线开关", isEnable);
        return true;
    }
}

