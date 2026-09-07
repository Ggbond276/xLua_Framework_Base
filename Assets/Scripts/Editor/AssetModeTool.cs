using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetModeTool : Editor
{
    private const string PrefsKey = "IsEditorLoadMode";
    
    [MenuItem("Tools/开启 BuildingResources加载模式")]
    public static void ToggleMode()
    {
        bool isEditorMode = EditorPrefs.GetBool(PrefsKey, true);
        EditorPrefs.SetBool(PrefsKey, !isEditorMode);

        Debug.Log($"<color=cyan>资源加载模式已切换：{(EditorPrefs.GetBool(PrefsKey, true) ? "【BuildingResources加载模式】" : "【BundleStreaming加载模式】")}</color>");

    }

    [MenuItem("Tools/开启 BuildingResources加载模式", true)]
    public static bool ToggleModeValidate()
    {
        bool isEditorMode = EditorPrefs.GetBool(PrefsKey, true);
        Menu.SetChecked("Tools/开启 BuildingResources加载模式", isEditorMode);
        return true;
    }
}
