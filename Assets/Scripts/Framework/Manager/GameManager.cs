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

        private static ResourcesManager _resources;
        public static ResourcesManager Resources
        {
            get { return _resources;  }
        }

        private void Awake()
        {
            // 对于Unity脚本 我们可以直接将 挂载 = 初始化
            _resources = this.gameObject.AddComponent<ResourcesManager>();
        }
    }
}
