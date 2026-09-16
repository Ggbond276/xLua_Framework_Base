using Assets.Scripts.Framework.ObjectPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Framework.Manager
{
    public class PoolManager : MonoBehaviour
    {
        Transform m_PoolRoot;

        Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

        private void Awake()
        {
            
        }

        /// <summary>
        /// 创建层级结构 挂载节点
        /// </summary>
        public void Init()
        {
            GameObject pool = new GameObject("Pool");
            pool.transform.SetParent(this.transform, false);
            m_PoolRoot = pool.transform;
        }



        /// <summary>
        /// 创建对象池的泛型方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="poolName"></param>
        /// <param name="releaseTime"></param>
       private void CreatePool<T>(string poolName, float releaseTime) where T : PoolBase
        {
            if(!m_Pools.TryGetValue(poolName, out PoolBase pool))
            {
                GameObject go = new GameObject(poolName);
                go.transform.SetParent(m_PoolRoot, false);
                pool = go.AddComponent<T>();
                pool.Init(releaseTime);
                m_Pools.Add(poolName, pool);
            }
        }
        /// <summary>
        /// 创建GameObject对象池
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="releaseTime"></param>
        public void CreateGameObjectPool(string poolName, float releaseTime)
        {
            CreatePool<GameObjectPool>(poolName, releaseTime);
        }
        /// <summary>
        /// 创建资源对象池
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="releaseTime"></param>
        public void CreateAssetPool(string poolName, float releaseTime)
        {
            CreatePool<AssetPool>(poolName, releaseTime);
        }

        /// <summary>
        /// 取出对象(资源取出对象池)
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public Object Spawn(string poolName, string assetName)
        {
            if(m_Pools.TryGetValue(poolName, out PoolBase pool))
            {
                return pool.Spwan(assetName);
            }
            return null;
        }

        /// <summary>
        /// 存入对象(资源放入对象池)
        /// </summary>
        /// <param name="poolName"></param>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        public void UnSpawn(string poolName, string assetName, Object asset)
        {
            if(m_Pools.TryGetValue(poolName, out PoolBase pool))
            {
                pool.UnSpwan(assetName, asset);
            }
        }

    }
}
