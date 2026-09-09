using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    internal class LuaManager : MonoBehaviour
    {
        // lua这边的容器需要先准备好 ResourcesManager才能准备给我们吐数据
        private List<string> luaNames;

        public void Init()
        {
            luaNames = new List<string>();
        }

        public void AddLuaName(string luaName)
        {
            luaNames.Add(luaName);
        }
    }
}
