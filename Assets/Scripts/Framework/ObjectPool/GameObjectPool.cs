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
        /// 从对象池中取出对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Object Spwan(string name)
        {
            Object obj = base.Spwan(name); // 取出对象池中的对象

            if (obj == null) // 如果对象池中的对象为空就返回空对象
                return null;

            GameObject go = obj as GameObject; // 强转取出的对象类型
            go.SetActive(true); // 显示出来
            return go; // 返回对象
        }

        /// <summary>
        /// 将对象存入对象池
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public override void UnSpwan(string name, Object obj)
        {
            GameObject go = obj as GameObject;
            go.SetActive(false); // 隐藏对象
            go.transform.SetParent(this.transform, false); // 挂载到对象池中
            base.UnSpwan(name, obj); // 将对象存入对象池
        }

        /// <summary>
        /// 释放对象池中的对象 检测超时时候会自动释放
        /// </summary>
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
