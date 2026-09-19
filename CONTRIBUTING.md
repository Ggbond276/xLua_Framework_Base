# 贡献指南

感谢你考虑为 xLua Framework Base 做出贡献！

## 提交规范

本项目使用 [Conventional Commits](https://www.conventionalcommits.org/) 规范：

```
<type>(<scope>): <subject>

<body>

<footer>
```

**类型（type）**：

| 类型 | 说明 |
| --- | --- |
| `feat` | 新功能 |
| `fix` | Bug 修复 |
| `refactor` | 重构（既不新增功能也不修 Bug） |
| `docs` | 文档变更 |
| `style` | 代码风格（不影响逻辑） |
| `test` | 测试相关 |
| `chore` | 构建/工具变更 |

**示例**：

```
feat(Net): NetManager 接入 GameManager 并生成 XLua Wrap

- GameManager.Inject 增加 NetManager 参数
- XLua 自动生成 GameManagerWrap 注册 Net 属性，供 Lua 访问
```

## 提交流程

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feat/amazing-feature`)
3. 提交你的修改 (`git commit -m 'feat: Add some amazing feature'`)
4. 推送到分支 (`git push origin feat/amazing-feature`)
5. 发起 Pull Request

## 代码规范

### C#

- 4 空格缩进
- 公共字段使用 PascalCase
- 私有字段使用 `m_CamelCase`
- 方法名 PascalCase
- 类、方法、字段必须带 XML 文档注释（公共 API）

### Lua

- 2 空格缩进
- 模块名 PascalCase（`UIMainLogic`）
- 局部变量 camelCase
- 公开方法 PascalCase（`OnOpen`）

### Commit

- 标题不超过 50 字符
- 正文详细解释"为什么"而非"是什么"
- 一个 commit 只做一件事

## Pull Request 检查清单

- [ ] 代码已在 Unity Editor 中通过编译
- [ ] 已测试热更 / 非热更两条启动路径
- [ ] 涉及 Lua 业务时同步更新对应的 C# Wrapper（XLua 重新生成）
- [ ] 重大修改更新了 README
- [ ] Commit message 符合规范

## 报告 Bug

请使用 [GitHub Issues](https://github.com/Ggbond276/xLua_Framework_Base/issues)，并包含：

1. Unity 版本
2. 复现步骤
3. 期望行为 vs 实际行为
4. 控制台日志（带颜色更佳）
5. 截图或视频（如适用）

## 联系我们

- GitHub: [@Ggbond276](https://github.com/Ggbond276)
