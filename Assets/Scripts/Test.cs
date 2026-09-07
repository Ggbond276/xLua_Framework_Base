using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        // 确保你的 FileList.txt 里有这个路径作为 Key
        string targetAsset = "Assets/BuildResources/UI/Prefab/Button.prefab";

        Debug.Log($"[Test] 准备发起异步加载: {targetAsset}");

        // 使用单例直接调用，并通过 Lambda 表达式完成回调实例化的全过程
        ResourcesManager.Instance.LoadAssets(targetAsset, (obj) =>
        {
            if (obj != null)
            {
                Debug.Log("[Test] 底层资源提取成功！开始实例化...");

                // 1. 将拿到的对象模板实例化到场景中
                GameObject go = Instantiate(obj) as GameObject;

                // 2. 将实例化出来的物体挂载到当前测试脚本所在的物体下
                go.transform.SetParent(this.transform);

                // 3. 重置一下本地坐标，确保它出现在屏幕中间
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);

                Debug.Log("[Test] 实例化完成，资源加载完美闭环！");
            }
            else
            {
                Debug.LogError("[Test] 加载失败，底层返回了 null！");
            }
        });
    }
}