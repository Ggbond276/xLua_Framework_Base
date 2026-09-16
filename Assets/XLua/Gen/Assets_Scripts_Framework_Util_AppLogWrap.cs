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
    public class AssetsScriptsFrameworkUtilAppLogWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Assets.Scripts.Framework.Util.AppLog);
			Utils.BeginObjectRegister(type, L, translator, 0, 0, 0, 0);
			
			
			
			
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 9, 0, 0);
			Utils.RegisterFunc(L, Utils.CLS_IDX, "LogSys", _m_LogSys_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogService", _m_LogService_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogIO", _m_LogIO_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogNet", _m_LogNet_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogDone", _m_LogDone_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogWarning", _m_LogWarning_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogError", _m_LogError_xlua_st_);
            Utils.RegisterFunc(L, Utils.CLS_IDX, "LogHighlight", _m_LogHighlight_xlua_st_);
            
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            return LuaAPI.luaL_error(L, "Assets.Scripts.Framework.Util.AppLog does not have a constructor!");
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogSys_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogSys( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogService_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogService( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogIO_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogIO( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogNet_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogNet( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogDone_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogDone( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogWarning_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogWarning( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogError_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogError( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LogHighlight_xlua_st_(RealStatePtr L)
        {
		    try {
            
            
            
                
                {
                    string _module = LuaAPI.lua_tostring(L, 1);
                    string _msg = LuaAPI.lua_tostring(L, 2);
                    
                    Assets.Scripts.Framework.Util.AppLog.LogHighlight( _module, _msg );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}
