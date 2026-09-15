using Assets.Scripts.Framework.Behaviour;
using Assets.Scripts.Framework.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Framework.Manager
{
    public class MySceneManager : MonoBehaviour
    {
        // 卸载的时候需要用到 难道没有更好的处理方法嘛
        // DEFENSE: 这里只能使用这样的方式不可以使用GetComponent 因为场景不是GameObject 无法使用GetComponent
        private string m_LogicName = "[SceneLogic]";


        // DEFENSE: 场景切换流畅 SetActive -> SceneManager.activeSceneChanged -> OnActiveSceneChanged
        private void Awake()
        {
            // 场景发生切换之后，我们需要做什么事情
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        /// <summary>
        /// 场景切换回调
        /// </summary>
        /// <param name="s1">被隐藏的场景</param>
        /// <param name="s2">被激活的场景</param>
        private void OnActiveSceneChanged(Scene s1, Scene s2)
        {
            if (!s1.isLoaded || !s2.isLoaded)
                return;

            SceneLogic logic1 = GetSceneLogic(s1);
            SceneLogic logic2 = GetSceneLogic(s2);

            logic1?.OnInAvtice();
            logic2?.OnActive();
        }

        /// <summary>
        /// 激活场景
        /// </summary>
        /// <param name="sceneName"></param>
        private void SetActive(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }


        /// <summary>
        /// 叠加加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="luaName"></param>
        public void LoadScene(string sceneName, string luaName)
        {
            // DEFENSE: Unity 的底层场景加载机制与普通资源（如 Prefab、材质、音频）存在根本区别，导致回调中传回的 obj 无法直接使用。
            GameManager.Resources.LoadScene(sceneName, (UnityEngine.Object obj) => {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
            });
        }


        /// <summary>
        /// 切换场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="luaName"></param>
        public void ChangeScene(string sceneName, string luaName)
        {
            GameManager.Resources.LoadScene(sceneName, (UnityEngine.Object obj) => {
                StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
            });
        }

        /// <summary>
        /// 场景异步加载方法
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="luaName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private IEnumerator StartLoadScene(string sceneName, string luaName, LoadSceneMode mode)
        {
            // 1.防重复加载校验
            if (IsLoadedScene(sceneName))
                yield break;

            // 2.异步加载并等待
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, mode);
            async.allowSceneActivation = true;
            yield return async;

            // 3.创建空物体[SceneLogic] 放到场景根目录下
            Scene scene = SceneManager.GetSceneByName(sceneName);
            GameObject go = new GameObject(m_LogicName);
            SceneManager.MoveGameObjectToScene(go, scene);


            AppLog.LogDone("MGR", $"SceneManager | 场景加载完成 | {sceneName}");

            // 4.建立和对应Lua脚本之间的联系
            SceneLogic logic = go.AddComponent<SceneLogic>();
            logic.Init(luaName);
            logic.OnEnter();
        }

        internal void Init()
        {
            AppLog.LogDone("MGR", "SceneManager | 初始化完成");
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private IEnumerator UnLoadScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);

            // 1.场景没有加载不需要卸载
            if(!scene.isLoaded)
            {
                AppLog.LogWarning("MGR", $"SceneManager | 卸载失败 | 场景未加载 | {sceneName}");
                yield break;
            }

            // 2.获取场景中的脚本
            SceneLogic logic = GetSceneLogic(scene);
            logic?.OnQuit();
            AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
            yield return async;
        }

        /// <summary>
        /// 获取挂载在场景上的SceneLogic脚本
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        private SceneLogic GetSceneLogic(Scene scene)
        {
            // 1.获取场景下的所有GameObject
            GameObject[] gameObjects = scene.GetRootGameObjects();
            foreach(GameObject go in gameObjects)
            {
                // 2.找到名为[SceneLogic]的脚本
                if (go.name.CompareTo(m_LogicName) == 0)
                {
                    // 3.获取Logic脚本并返回
                    SceneLogic logic = go.GetComponent<SceneLogic>();
                    return logic;
                }
            }
            return null;
        }

        /// <summary>
        /// 判断当前场景是否已经被加载了
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private bool IsLoadedScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded;
        }
    }
}
