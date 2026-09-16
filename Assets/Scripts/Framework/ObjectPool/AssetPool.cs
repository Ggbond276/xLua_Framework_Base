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

        public override UnityEngine.Object Spwan(string name)
        {
            return base.Spwan(name);
        }

        public override void UnSpwan(string name, UnityEngine.Object obj)
        {
            base.UnSpwan(name, obj);
        }


        public override void Release()
        {
            base.Release();
            foreach(PoolObject item in m_Objects)
            {
                if(DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
                {
                    // Log
                    GameManager.Resources.UnLoadBundle(name);
                    m_Objects.Remove(item);
                    Release();
                    return;
                }
            }
        }
    }
}
