# xLua Framework Base

基于 Unity 2022 LTS 与 Tencent xLua 的客户端底层框架——提供热更新、AB 资源管理、双对象池、TCP 网络与 UI / 场景 / 实体生命周期管理。

![Unity](https://img.shields.io/badge/Unity-2022.3.46f1c1-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-9.0-blue?logo=csharp)
![.NET](https://img.shields.io/badge/.NET-Standard%202.1-purple?logo=dotnet)
![xLua](https://img.shields.io/badge/xLua-v2.1.15-green)
![Lua](https://img.shields.io/badge/Lua-5.3-blue?logo=lua)
![TextMeshPro](https://img.shields.io/badge/TextMeshPro-3.0.6-orange)
![Visual Scripting](https://img.shields.io/badge/Visual_Scripting-1.9.4-yellow)
![Timeline](https://img.shields.io/badge/Timeline-1.7.6-red)
![UGUI](https://img.shields.io/badge/UGUI-1.0.0-lightgrey)
![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Android%20%7C%20iOS%20%7C%20WebGL-blue)

---

## 项目简介

`xLua_Framework_Base` 是一个面向 Unity 商业项目的**客户端底层框架**。它把工程中"与具体业务无关"的能力——启动流程、资源加载、网络通信、对象池、Lua 虚拟机、UI / 场景 / 实体生命周期——沉淀到 C# 层，业务侧使用 Lua 开发。

Lua 脚本既可随包发布，也可作为热更资源增量下发。

**项目定位**：Unity 客户端工程脚手架 / 原型框架。本仓库只包含客户端，不含服务端、协议生成器与数据库。

---

## 技术栈

| 技术 | 版本 |
|---|---|
| Unity | 2022.3.46f1c1 LTS |
| C# LangVersion | 9.0 |
| .NET API 兼容级别 | .NET Standard 2.1 |
| xLua | v2.1.15 |
| Lua | 5.3 |
| TextMeshPro | 3.0.6 |
| Visual Scripting | 1.9.4 |
| Timeline | 1.7.6 |
| UGUI | 1.0.0 |

---

## 核心功能

- **冷启动 / 热更新分流**——环境探针自动判定首装释放还是云端增量对比
- **AssetBundle 资源管理**——自动依赖分析、运行时引用计数、超时卸载
- **双对象池**——`GameObjectPool`（UI / Entity 实例）+ `AssetPool`（AB 资源）
- **TCP 网络客户端**——IPv4 / IPv6 自适应，按长度字段分包
- **Lua 虚拟机**——每个 LuaBehaviour 持有独立沙盒表，自动回收
- **UI / 场景 / 实体生命周期**——`UILogic` / `SceneLogic` / `EntityLogic` 三个子类统一封装
- **7 色企业级日志**——SYS / SVC / IO / NET / WARN / ERR / HIGHLIGHT
- **编辑器工具**——AB 构建、资源加载模式切换、热更开关一键切换

---

## 核心架构

```
                Unity 引擎
                    |
                    v
                GameApp                 <-- 冷启动入口
                    |
        +-----------+-----------+
        |                       |
        v                       v
    HotUpdate            FrameworkBootstrap   <-- 4 阶段启动
        |                       |
        +-----------+-----------+
                    v
              GameManager          <-- 注入所有 Manager
                    |
   +-----+---------+---------+-----+-----+
   |     |         |         |     |     |
   v     v         v         v     v     v
 Pool  Lua  Resources  UI  Scene  Sound  Event  Net
   |     |         |         |     |     |     |
   +-----+---------+---------+-----+-----+-----+
                         |
                         v
                  LuaBehaviour                <-- UI / Scene / Entity
                         |
                         v
                  Lua 业务脚本               <-- .bytes（Main / Scene / UI / Entity / Message）
                         |
                         v
                   NetClient (TCP)
```

---

## 项目结构

```
xLua_Framework_Base/
├── Assets/
│   ├── BuildResources/             业务资源
│   │   ├── Audio/                  音乐与音效
│   │   ├── Effect/                 特效
│   │   ├── LuaScripts/             Lua 字节码（.bytes）
│   │   │   ├── Main.bytes          入口
│   │   │   ├── Scene/              场景逻辑（Loading / MainCity）
│   │   │   ├── UI/                 UI 逻辑（Main / Shop / Sound）
│   │   │   ├── Entity/             实体逻辑（PlayerInputController）
│   │   │   └── Message/            消息协议（base_msg / msg_mgr）
│   │   ├── Model/Prefabs/
│   │   ├── Scene/                  Unity 场景
│   │   └── UI/Prefabs/             UI 预制
│   ├── Plugins/                    原生插件（xlua.bundle 等）
│   ├── Scripts/
│   │   ├── Framework/              C# 框架核心
│   │   │   ├── GameApp.cs
│   │   │   ├── FrameworkBootstrap.cs
│   │   │   ├── FrameworkConfig.cs
│   │   │   ├── HotUpdate.cs
│   │   │   ├── Behaviour/          LuaBehaviour / UILogic / SceneLogic / EntityLogic
│   │   │   ├── Manager/            10 个管理器
│   │   │   ├── Network/            TCP 客户端
│   │   │   ├── ObjectPool/         PoolBase / PoolObject / GameObjectPool / AssetPool
│   │   │   └── Util/               AppLog / PathUtil / FileUtil
│   │   ├── Editor/                 构建工具与模式切换
│   │   └── PlayerMove.cs           示例
│   └── XLua/                       xLua 引擎源码
├── Packages/manifest.json          UPM 包依赖
├── ProjectSettings/                Unity 工程配置
├── LICENSE
├── CONTRIBUTING.md
├── SECURITY.md
└── README.md
```

---

## 运行方式

### 1. 准备 Unity

- 安装 **Unity 2022.3.46f1c1 LTS**（与 `ProjectSettings/ProjectVersion.txt` 一致）
- 用 Unity Hub 打开本目录，等待依赖恢复
- 确认 `Project Settings → Player → Other Settings → Api Compatibility Level = .NET Standard 2.1`

### 2. 创建框架配置

1. 在 Project 面板右键 → `Create → Framework → 创建核心配置文件`
2. 生成 `Assets/Resources/FrameworkConfig.asset`
3. 在 Inspector 中填入云端服务器地址（如 `http://127.0.0.1:8080`）

### 3. 选择加载模式（编辑器内）

- `Tools → 开启 BuildingResources加载模式`：开发期直读 `BuildResources/`
- `Tools/Framework/切换 热更新管线开关`：是否走热更流程

### 4. 构建 AB 包（可选）

`Tools → Build Windows / Android / iPhone Bundle`：扫描 `Assets/BuildResources/` → 解析依赖 → 生成 `FileList.txt` → 调用 `BuildPipeline.BuildAssetBundles` 打包到 `StreamingAssets/`。

### 5. 运行

启动场景中挂 `GameApp` 预制，运行即可。运行时流程：

```
GameApp
   ↓
HotUpdate（可选）
   ↓
FrameworkBootstrap（4 阶段）
   ↓
GameManager.Inject()
   ↓
LuaManager.LoadMain()
   ↓
Main.bytes（Lua 入口）
   ↓
进入游戏
```

---

## 项目状态

**已完成**

- 冷启动、热更新状态机、AB 管理、双对象池、Lua 集成、UI / Scene / Entity 生命周期、全局事件、音效、TCP 网络、编辑器工具

**未提供**

- 服务端工程（仓库不含）
- 数据库持久化（仓库不含）
- Protobuf / 协议自动生成（当前使用自研 TCP 协议：`4B msgId + 4B msgLen + UTF-8 body`）
- 单元测试（`Packages/manifest.json` 未启用 `com.unity.test-framework`）

---

## 协议

MIT License，详见 `LICENSE`。
