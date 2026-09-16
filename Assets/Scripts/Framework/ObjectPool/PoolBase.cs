using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Framework.ObjectPool
{
    public class PoolBase : MonoBehaviour
    {
        
        protected float m_ReleaseTime;

        protected long m_LastReleaseTime = 0;



        protected List<PoolObject> m_Objects;

        private void Start()
        {
            m_LastReleaseTime = System.DateTime.Now.Ticks;
        }

        public void Init(float time)
        {
            m_ReleaseTime = time;
            m_Objects = new List<PoolObject>();
        }

        // TODO: 对象池使用List数据结构会导致查找过慢的性能问题 这里以后要思考对对象池做数据结构优化
        /// <summary>
        /// 判断一个对象池中有没有对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool isInPool(string name)
        {
            foreach(var obj in m_Objects)
            {
                if(obj.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 从对象池中取出对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Object Spwan(string name)
        {
            foreach(var po in m_Objects) // 遍历对象池进行姓名匹配取出对象
            {
                if(po.Name == name)
                {
                    m_Objects.Remove(po);
                    return po.Object;
                }
            }
            return null;
        }

        /// <summary>
        /// 将对象打包存入对象池
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public virtual void UnSpwan(string name, UnityEngine.Object obj)
        {
            PoolObject po = new PoolObject(name, obj);
            m_Objects.Add(po);
        }

        public virtual void Release()
        {

        }


        private void Update()
        {
            if(System.DateTime.Now.Ticks - m_LastReleaseTime >= m_ReleaseTime * 10000000)
            {
                m_LastReleaseTime = DateTime.Now.Ticks;
                Release();
            }
        }
    }
}
