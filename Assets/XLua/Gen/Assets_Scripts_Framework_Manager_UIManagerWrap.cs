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
    public class AssetsScriptsFrameworkManagerUIManagerWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Assets.Scripts.Framework.Manager.UIManager);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 4, 4);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OpenUI", _m_OpenUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "CloseUI", _m_CloseUI);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Destroy", _m_Destroy);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "BackLayer", _g_get_BackLayer);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "MidLayer", _g_get_MidLayer);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "TipLayer", _g_get_TipLayer);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "LoadingLayer", _g_get_LoadingLayer);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "BackLayer", _s_set_BackLayer);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "MidLayer", _s_set_MidLayer);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "TipLayer", _s_set_TipLayer);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "LoadingLayer", _s_set_LoadingLayer);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new Assets.Scripts.Framework.Manager.UIManager();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Assets.Scripts.Framework.Manager.UIManager constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OpenUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _uiName = LuaAPI.lua_tostring(L, 2);
                    string _luaName = LuaAPI.lua_tostring(L, 3);
                    Assets.Scripts.Framework.Manager.UILayer _layer;translator.Get(L, 4, out _layer);
                    
                    gen_to_be_invoked.OpenUI( _uiName, _luaName, _layer );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_CloseUI(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _uiName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.CloseUI( _uiName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Destroy(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _uiName = LuaAPI.lua_tostring(L, 2);
                    
                    gen_to_be_invoked.Destroy( _uiName );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_BackLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.BackLayer);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_MidLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.MidLayer);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_TipLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.TipLayer);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_LoadingLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.LoadingLayer);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_BackLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.BackLayer = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_MidLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.MidLayer = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_TipLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.TipLayer = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_LoadingLayer(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Assets.Scripts.Framework.Manager.UIManager gen_to_be_invoked = (Assets.Scripts.Framework.Manager.UIManager)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.LoadingLayer = (UnityEngine.Transform)translator.GetObject(L, 2, typeof(UnityEngine.Transform));
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
