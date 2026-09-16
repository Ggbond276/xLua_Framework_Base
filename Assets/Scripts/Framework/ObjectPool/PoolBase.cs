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

        public virtual Object Spwan(string name)
        {
            foreach(var po in m_Objects)
            {
                if(po.Name == name)
                {
                    m_Objects.Remove(po);
                    return po.Object;
                }
            }
            return null;
        }

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
