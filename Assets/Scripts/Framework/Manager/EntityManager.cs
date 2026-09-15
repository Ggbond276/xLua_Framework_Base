using Assets.Scripts.Framework.Behaviour;
using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Framework.Manager
{
    [XLua.LuaCallCSharp]
    public class EntityManager : MonoBehaviour
    {

        /// <summary>
        /// 键是什么 值是什么 我都不知道
        /// </summary>
        Dictionary<string, GameObject> m_Entites = new Dictionary<string, GameObject>();
        /// <summary>
        /// 键是什么 值是什么 我都不知道
        /// </summary>
        Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();
        private Transform m_Entity;

        public void Init()
        {
            Transform runtime = this.transform.parent.Find("Runtime"); // 找到Runtime节点
            if(runtime == null) {
                AppLog.LogError("EntityManager", "runtime is not exist");
                return;
            }
            GameObject entityGo = new GameObject("Entity");
            entityGo.transform.SetParent(runtime);
            m_Entity = entityGo.transform;
            AppLog.LogDone("MGR", "EntityManager | 初始化完成");
        }

        /// <summary>
        /// 设置分组- 这个由外部传入可以实现自定义
        /// 比如现在有一个List NCP Monster Character 那现在我们就会有三个分组
        /// </summary>
        /// <param name="groups"></param>
        public void SetEntityGroup(List<string> groups)
        {
            for(int i = 0; i < groups.Count; i++)
            {
                GameObject group = new GameObject("Group-" + groups[i]);  // 创建名教 Group-NPC的节点
                group.transform.SetParent(m_Entity, false); // 将节点放在m_EntityParent节点下
                m_Groups[groups[i]] = group.transform; // 将节点放入内存方便进行获取
            }
        }

        // 对于分组我们应该直接根据分组创建父节点 然后存入到

        /// <summary>
        /// 获取分组的根节点
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Transform GetGroup(string group)
        {
            if (!m_Groups.ContainsKey(group)) // 看Group中有没有分组
                AppLog.LogError("EntityManager", "group is not exist"); // 没有就直接输出不存在
            return m_Groups[group]; // 有就返回根节点
        }

        /// <summary>
        /// 显示Entity
        /// </summary>
        /// <param name="entityName">实体的名字叫做什么</param>
        /// <param name="group">实体是哪个组的</param>
        /// <param name="luaName">实体挂载的lua脚本的名字</param>
        public GameObject ShowEntity(string entityName, string group, string luaName)
        {
            
            GameObject entity = null; // 创建空物体
            if(m_Entites.TryGetValue(entityName, out entity)) // 判断实体是否存在
            {
                EntityLogic logic = entity.GetComponent<EntityLogic>(); // 实体存在就获取脚本
                logic.OnShow(); // 实体显示 做对应操作 TODO: 为什么这里不需要Active就能显示啊 OnShow() 不是给Lua的回调吗
                return entity;
            }


            GameManager.Resources.LoadModel(entityName, (UnityEngine.Object obj) =>  // 根据entityName的名字 将AB包数据加载道内存中去
            {
                entity = Instantiate(obj) as GameObject; // 将内存中的entity实例化到场景中
                Transform parent = GetGroup(group); // 根据自己的group分组名字，获取到父节点
                entity.transform.SetParent(parent, false); // 将实体挂载到父节点下
                EntityLogic entityLogic = entity.AddComponent<EntityLogic>(); // 获取实体脚本
                entityLogic.Init(luaName); // 实现脚本与Lua的绑定
                entityLogic.OnShow(); // 通知Lua实体显示了 做对应操作
            });

            return entity;

        } 
    }
}
