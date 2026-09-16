using Assets.Scripts.Framework.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Framework.ObjectPool
{
    public class AssetPool : PoolBase
    {

        /// <summary>
        /// 取出对象池中的货物使用
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override UnityEngine.Object Spwan(string name)
        {
            return base.Spwan(name);
        }

        /// <summary>
        /// 将对象打包放入对象池
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public override void UnSpwan(string name, UnityEngine.Object obj)
        {
            base.UnSpwan(name, obj);
        }


        public override void Release()
        {
            base.Release();
            foreach(PoolObject item in m_Objects) // 循环对象池中的所有对象
            {
                if(DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000) // 如果对象池很久都没有被使用过了
                {
                    // Log
                    GameManager.Resources.UnLoadBundle(item.Name);
                    m_Objects.Remove(item);
                    Release();
                    return;
                }
            }
        }


    }
}
