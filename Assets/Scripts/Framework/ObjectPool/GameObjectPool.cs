using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Framework.ObjectPool
{
    public class GameObjectPool : PoolBase 
    {
        /// <summary>
        /// 为什么是到父类里面取对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Object Spwan(string name)
        {
            Object obj = base.Spwan(name);
            if (obj == null)
                return null;

            GameObject go = obj as GameObject;
            go.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 回收对象的方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public override void UnSpwan(string name, Object obj)
        {
            GameObject go = obj as GameObject;
            go.SetActive(false);
            go.transform.SetParent(this.transform, false);
            base.UnSpwan(name, obj);
        }



        public override void Release()
        {
            base.Release();
            foreach(PoolObject item in m_Objects)
            {
                if(System.DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
                {
                    // Log一下
                    Destroy(item.Object);
                    m_Objects.Remove(item);
                    Release();
                    return;
                }
            }
        }
    }
}
