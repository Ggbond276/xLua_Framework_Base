#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class AssetsScriptsFrameworkManagerGameManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Assets.Scripts.Framework.Manager.GameManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 2, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Inject", _m_Inject);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnFrameworkReady", _m_OnFrameworkReady);
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 9, 0);
			
			
            
			Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Resources", _g_get_Resources);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Lua", _g_get_Lua);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "UI", _g_get_UI);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Entity", _g_get_Entity);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Scene", _g_get_Scene);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Sound", _g_get_Sound);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Event", _g_get_Event);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Pool", _g_get_Pool);
            Utils.RegisterFunc(L, Utils.CLS_GETTER_IDX, "Net", _g_get_Net);
            
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new Assets.Scripts.Framework.Manager.GameManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Assets.Scripts.Framework.Manager.GameManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Inject(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Assets.Scripts.Framework.Manager.GameManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.GameManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    Assets.Scripts.Framework.Manager.ResourcesManager _resources = (Assets.Scripts.Framework.Manager.ResourcesManager)translator.GetObject(L, 2, typeof(Assets.Scripts.Framework.Manager.ResourcesManager));
                    Assets.Scripts.Framework.Manager.LuaManager _lua = (Assets.Scripts.Framework.Manager.LuaManager)translator.GetObject(L, 3, typeof(Assets.Scripts.Framework.Manager.LuaManager));
                    Assets.Scripts.Framework.Manager.UIManager _ui = (Assets.Scripts.Framework.Manager.UIManager)translator.GetObject(L, 4, typeof(Assets.Scripts.Framework.Manager.UIManager));
                    Assets.Scripts.Framework.Manager.EntityManager _entity = (Assets.Scripts.Framework.Manager.EntityManager)translator.GetObject(L, 5, typeof(Assets.Scripts.Framework.Manager.EntityManager));
                    Assets.Scripts.Framework.Manager.MySceneManager _scene = (Assets.Scripts.Framework.Manager.MySceneManager)translator.GetObject(L, 6, typeof(Assets.Scripts.Framework.Manager.MySceneManager));
                    Assets.Scripts.Framework.Manager.SoundManager _sound = (Assets.Scripts.Framework.Manager.SoundManager)translator.GetObject(L, 7, typeof(Assets.Scripts.Framework.Manager.SoundManager));
                    Assets.Scripts.Framework.Manager.EventManager _myEvent = (Assets.Scripts.Framework.Manager.EventManager)translator.GetObject(L, 8, typeof(Assets.Scripts.Framework.Manager.EventManager));
                    Assets.Scripts.Framework.Manager.PoolManager _pool = (Assets.Scripts.Framework.Manager.PoolManager)translator.GetObject(L, 9, typeof(Assets.Scripts.Framework.Manager.PoolManager));
                    Assets.Scripts.Framework.Manager.NetManager _net = (Assets.Scripts.Framework.Manager.NetManager)translator.GetObject(L, 10, typeof(Assets.Scripts.Framework.Manager.NetManager));
                    
                    gen_to_be_invoked.Inject( _resources, _lua, _ui, _entity, _scene, _sound, _myEvent, _pool, _net );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnFrameworkReady(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Assets.Scripts.Framework.Manager.GameManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.GameManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.OnFrameworkReady(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Resources(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Resources);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Lua(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Lua);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_UI(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.UI);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Entity(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Entity);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Scene(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Scene);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Sound(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Sound);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Event(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Event);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Pool(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Pool);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_Net(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			    translator.Push(L, Assets.Scripts.Framework.Manager.GameManager.Net);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}
