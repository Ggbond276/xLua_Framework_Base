using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    class GameManager : MonoBehaviour
    {
        // 1. 点击开始之后 热更新加载完毕 GameManager脚本就会被挂载到Root并走初始化流程
        // 2. 然后就是ResourcesManager被挂载并初始化
        // 3. 然后就是LuaManager被挂载并初始化


        private static ResourcesManager _resources;
        public static ResourcesManager Resources
        {
            get { return _resources;  }
        }


        private static LuaManager _lua;
        public static LuaManager Lua
        {
            get { return _lua; }
        }


        private void Awake()
        {
            // 对于Unity脚本 我们可以直接将 挂载 = 初始化
            // 这个时候大家都做初始化工作 都不允许干活！
            _resources = this.gameObject.AddComponent<ResourcesManager>();
            _lua = this.gameObject.AddComponent<LuaManager>();

            // 为什么是lua先初始化呢 因为_resources在初始化的时候会给_lua吐数据
            // 所以我们需要_lua先准备好容器准备接收
            _lua.Init();
            _resources.Init();
        }
    }
}
