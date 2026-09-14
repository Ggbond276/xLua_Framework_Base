using Assets.Scripts.Framework.Behaviour;
using Assets.Scripts.Framework.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XLua;

namespace Assets.Scripts.Framework.Manager
{
    // 1.UI层级枚举
    [LuaCallCSharp]
    public enum UILayer
    {
        Back = 0,
        Mid = 1,
        Tip = 2,
        Loading = 3
    }

    [LuaCallCSharp]
    public class UIManager : MonoBehaviour
    {

        // 2. 4个层级TransForm
        public Transform BackLayer;
        public Transform MidLayer;
        public Transform TipLayer;
        public Transform LoadingLayer;

        // 存放UI的容器 UI以GameObject的形式存在

        // 我要打开一个UI 并告诉这个UI它对应的是哪个Lua


        // 这里分为第一次打开UI和第二次打开UI
        // 第一次打开UI内存缓存中肯定是没有的
        // 所以需要去硬盘中拿到UIPrefab 然后进行实例化工作
        // 第二次打开UI的时候内存缓存中会有UI
        // 所以直接去内存缓存中拿到UI显示出来即可

        // 所以OpenUI这个方法分为两种情况进行处理
        // 这里要弄清楚缓存的概念 我们需要一个缓存的容器 来对UI进行管理
        // 那么这里如何造出肉体呢 其实很简单
        // 我们要记住 我们之前写的ResourcesManager就是用来异步进行资源加载的
        // 使用了之后 资源就可以被加载到内存缓存之中了

        // 切记资源加载之后资源只是存在于内存缓存之中 还是GameObject 并不是实例化能看见的东西
        // 所以一定要使用实例化方法才能把内存缓存中的GameObject加载Unity场景中


        // 然后这里记住一点 所有的GameObject都是没有挂载LuaBeaviour脚本的、
        // 都是由UIManager动态加载的时候动态进行挂载的
        // 然后UIManager会将LuaName这个钥匙交给LuaLogic

        // DEFENSE: 内存泄漏问题
        // NOTE: 这里为了追求秒开体验选择使用空间换时间，玩家关闭UI的时候，代码根本没有调用Destory来摧毁物体
        // 而是单纯的SetActive(false), 这在单机小游戏中是没有问题的，但是如果进行大型游戏开发，玩家在两小时中
        // 打开了50个不同的界面，这50个界面全部都会堆积在内存永远不释放，最终游戏的内存会被撑爆，游戏直接闪退
        private Dictionary<string, GameObject> m_UIDict = new Dictionary<string, GameObject>();



        /// <summary>
        /// 生命周期开始，创建Canvas + 四个层级面板
        /// </summary>
        private void Awake()
        {
            CreateCanvas();
        }

        /// <summary>
        /// 生命周期结束，销毁容器
        /// </summary>
        private void OnDestroy()
        {
            if (m_UIDict != null)
            {
                m_UIDict.Clear();
                m_UIDict = null;
            }
        }


        /// <summary>
        /// 创建Canvas + 四个层级面板
        /// </summary>
        private void CreateCanvas()
        {
            // 1.创建一个空物体 给他赋予名字 并挂载到指定节点
            GameObject canvasObj = new GameObject("UICanvas");
            canvasObj.transform.SetParent(this.transform);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0; // 同一 Canvas 下靠 sibling排序,跨 Canvas 才用 sortingOrder

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // 2. 创建 4 个层级
            BackLayer = CreateLayer("BackLayer", canvasObj.transform);
            MidLayer = CreateLayer("MidLayer", canvasObj.transform);
            TipLayer = CreateLayer("TipLayer", canvasObj.transform);
            LoadingLayer = CreateLayer("LoadingLayer", canvasObj.transform);
        }

        /// <summary>
        /// 创建层级面板
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        private Transform CreateLayer(string name, Transform parent)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent);
            RectTransform rect = go.AddComponent<RectTransform>();
            // 铺满父级
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return go.transform;
        }

        /// <summary>
        /// 根据枚举值返回对应的层级Transform
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        private Transform GetLayer(UILayer layer)
        {
            switch (layer)
            {
                case UILayer.Back: return BackLayer;
                case UILayer.Mid: return MidLayer;
                case UILayer.Tip: return TipLayer;
                case UILayer.Loading: return LoadingLayer;
                default: return BackLayer;
            }
        }



        // ================================================================
        // 【上行】Lua → UIManager (Lua 主动调用的方法)
        // ================================================================


        /// <summary>
        /// 打开UI面板
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="luaName"></param>
        public void OpenUI(string uiName, string luaName, UILayer layer)
        {
            // 检查传入是否为空
            if(string.IsNullOrEmpty(uiName))
            {
                AppLog.LogSys("UIManager", "OpenUI 失败:uiName 为空");
                return;
            }


            // 看是否命中缓存
            if (m_UIDict.TryGetValue(uiName, out GameObject ui))
            {
                // 缓存命中 → 直接显示
                // OnEnable 会自动触发 → 翻译官会通知 Lua:OnOpen
                ui.SetActive(true);
                ui.GetComponent<UILogic>()?.OnOpen();
                return;
            }

            // 如果缓存没有命中就异步进行资源加载
            GameManager.Resources.LoadUI(uiName, (UnityEngine.Object obj) => {
                if (obj == null) return;

                // 1.实例化并挂载
                GameObject go = Instantiate(obj) as GameObject;
                go.name = uiName;
                go.transform.SetParent(GetLayer(layer), false);
                go.transform.localScale = Vector3.one;

                // 2.加入缓存
                m_UIDict.Add(uiName, go);

                // 3.挂翻译官
                UILogic uiLogic = go.AddComponent<UILogic>();
                // 4.初始化，实现UILogic和lua脚本的双向绑定
                uiLogic.Init(luaName);
                uiLogic.OnOpen();
                
            });

        }
        /// <summary>
        /// 关闭UI面板
        /// </summary>
        /// <param name="uiName"></param>
        public void CloseUI(string uiName)
        {
            if(m_UIDict.TryGetValue(uiName, out GameObject ui))
            {
                ui.GetComponent<UILogic>()?.OnClose();
                ui.SetActive(false);
            } else
            {
                AppLog.LogService("UIManager" ,$"CloseUI 失败:缓存中找不到 {uiName}");
            }
        }
        /// <summary>
        /// 销毁UI面板
        /// </summary>
        /// <param name="uiName"></param>
        public void Destroy(string uiName)
        {
            if(m_UIDict.TryGetValue(uiName, out GameObject ui))
            {
                Destroy(ui);
                m_UIDict.Remove(uiName);
            }
        }



        
    }
}
