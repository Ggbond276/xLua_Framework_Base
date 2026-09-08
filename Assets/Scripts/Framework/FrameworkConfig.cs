using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework
{
    // 在资源右键菜单中暴露创建入口
    [CreateAssetMenu(fileName = "FrameworkConfig", menuName = "Framework/创建核心配置文件")]
    public class FrameworkConfig : ScriptableObject
    {
        [Header("云端服务器根地址")]
        public string ServerUrl = "[http://127.0.0.1:8080](http://127.0.0.1:8080)";

        // 未来还可以加：版本号、是否开启日志等配置
    }
}
