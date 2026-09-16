using Assets.Scripts.Framework.ObjectPool;
using Assets.Scripts.Framework.Util;
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
            pool.transform.SetParent(this.transform, false); // 现在就有一个Pool节点挂载在System上
            m_PoolRoot = pool.transform;

            CreateGameObjectPool("UIPool", 10); // 创建一个UI 10秒没有使用就销毁
            CreateGameObjectPool("EntityPool", 5); // 创建Entity对象池 5秒没有使用就销毁
            
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
                GameObject go = new GameObject(poolName); // 创建一个叫做poolName空物体
                go.transform.SetParent(m_PoolRoot, false); // 挂载到Pool节点上
                pool = go.AddComponent<T>(); // 给当前节点挂载脚本 什么类型的对象池就挂载什么脚本
                pool.Init(releaseTime); // 初始化对象池，设置销毁时间
                m_Pools.Add(poolName, pool); // 对象池管理器加入一个对象池
                AppLog.LogDone("POOL", $"对象池 | 创建完成 | {poolName}");
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
