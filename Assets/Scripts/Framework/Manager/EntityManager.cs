using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    public class EntityManager : MonoBehaviour
    {
        Dictionary<string, GameObject> m_Entites = new Dictionary<string, GameObject>();
        Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();
        private Transform m_EntityParent;

        public void Init()
        {
            AppLog.LogDone("MGR", "EntityManager | 初始化完成");
        }
    }
}
