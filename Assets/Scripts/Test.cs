using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        // 现在的终极清爽写法：只传名字，结合 Lambda 闭包秒杀一切！
        ResourcesManager.Instance.LoadUI("Button", (obj) =>
        {
            if (obj != null)
            {
                GameObject go = Instantiate(obj) as GameObject;
                go.transform.SetParent(this.transform);
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);
            }
        });
    }
}