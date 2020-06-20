# 中文 [English](README_EN.md "English")
# IdentityServerDemo
这是一个 .net core 综合性学习和示例收集项目，包含各种功能的基础使用和综合应用示例。目标是尽可能方便快速地展示各种功能的使用方法和实际效果。
<br> 当前框架版本： .Net Core 3.1
<br> VS 2019 版本： 16.6.0 以上
<br> 初始账号： `admin`、`coredx`、`alice`、`bob`
<br> 初始密码： `Pass123$`

## 解决 Github 克隆速度慢的方法
将 ```www.github.com/仓库地址``` 改为 ```www.github.com.cnpmjs.org/仓库地址```

## 可运行的示例程序
1. DemoApp
   1.  Client： C# oidc 客户端示例。
   2.  CSharpScriptDemo：使用 Roslyn 运行 C# 脚本的示例。
   3.  DomianSample：临时性测试代码。
   4.  GrpcClient：控制台 gRPC 客户端示例。
   5.  IdentityServer：主要项目，所有客户端的服务端。包括大多数示例。
   6.  IdentityServerGui：IdentityServer 的图形化外壳。.net core 3.x Windows 桌面应用开发示例（WPF）。
   7.  JavascriptClient：js oidc 客户端示例。
   8.  SudokuTset：数独求解器测试代码和使用示例。
   9.  WebAPI mvc oidc 客户端示例。
   10. WebClient：过时代码存档。
   11. GraphicDemo：Windows 窗口和屏幕截图示例。相关博客：[Windows GDI 窗口与 Direct3D 屏幕截图](https://www.cnblogs.com/coredx/p/12422559.html)
   12. vJoyDemo：虚拟游戏手柄使用演示。
2. IdentityServerAdmin
   1.  IdentityServer.Admin：IdentityServer4 Web 管理界面。依赖 IdentityServer。
   2.  IdentityServer.Admin.Api：IdentityServer4 Web Api。依赖 IdentityServer。

## 使用说明
### 调试方式
1. 下载这两个项目并打开 IdentityServerDemo 解决方案： <br>``` https://github.com/CoreDX9/IdentityServerDemo.git ```<br>``` https://github.com/CoreDX9/Harmonic.git ```
2. 卸载这些项目：<br>```Data/Domain.EntityFrameworkCore```<br>```Data/EntityHistoryMQReceive```<br>```Data/Repository```<br>```DemoApp/WebClient```<br>```Infrastructure/ResourceOwnerClient```
3. 修复 Infrastructure/Harmonic 的项目引用。
4. 等待 VS2019 自动还原 Nuget 和 npm 包。如果 npm 包自动还原失败，可尝试在 cmd 中运行 npm install 命令。请确保已安装 Node.js。如果出现 gyp 错误，可尝试（以管理员身份）运行命令```npm install --global --production windows-build-tools```进行修复。
5. 编译生成 ```DemoPlugin/Plugin1```。
6. 把 IdentityServer 设置为默认启动项目并设置启动方式为自托管方式。
7. 根据你的实际情况修改 IdentityServer/appsettings.json 和 appsettings.Development.json 中的数据库连接字符串。目前仅支持 MS SqlServer，需要其他数据库支持的请自行修改代码。
8. 运行 IdentityServer 项目。开始试用并探索各种示例吧。所有示例均可在网页顶部的导航栏找到。如果网页显示效果异常，请尝试调整浏览器窗口大小或调整页面缩放比例。
9. 运行你感兴趣的其他可执行程序项目。

### 发布方式
1. 根据前面的说明确保能正常启动调试。
2. 在 IdentityServer 项目上单击右键并选择发布，选择 FolderProfileRelease 发布配置，发布项目。如果一切正常，可以在解决方案文件夹找到 .publish 文件夹。
3. 在 Plugin1 项目上单击右键并选择发布，发布项目。
4. 在 IdentityServerGui 项目上单击右键并选择发布，发布项目。如果一切正常，可以在 IdentityServer 的发布位置找到 IdentityServerGui.exe。
5. 根据你的实际情况修改 IdentityServer 的 appsettings.json 和 appsettings.Production.json 的数据库连接字符串。
6. 双击 IdentityServer.exe 或者 IdentityServerGui.exe 运行程序。

### 注意事项
1. 尽量使用较高版本的浏览器，尽量不要使用 IE。
2. 根据情况修改 appsettings.json 的各项设定。
3. 时间精力有限，无法全面测试程序，可能会发生更新框架或包版本导致功能异常的情况，如有发现，欢迎提 issue 交流。我会尽力修复。
4. IdentityServer 项目包含一个自签名的 CA 根证书和网站证书。自签名证书默认会在发布模式下使用，颁发给 localhost 域名，确保在发布模式下可以正常使用 Https 协议。
5. 使用我提供的配置发布项目后可以在全新安装的 Windows 7 Sp1 及更高版本的系统中直接运行，没有任何依赖，前提是在配置中设置为使用内存数据库，方便快速运行示例。
6. 数据库默认会重定向到 IdentityServer 项目的 App_Data/Database 文件夹。同时为了能让调试模式和发布模式的程序运行互不干扰，数据库名称默认包含运行环境后缀。
---
## 包含的示例
### IdentityServer
1. 自定义项目发布配置，通过修改 csproj 项目文件实现全自动化发布。
2. JQuery、Bootstrap4、Lay UI、Vue、Element-UI、Avue、Axios、Monaco-editor 等众多常用前端框架和库的使用。
3. IdentityServer4 与 Identity Core 的使用；SSO 集成支持。
4. Identity Core 管理；IdentityServer4 管理。相关博客：[Asp.Net Core IdentityServer4 管理面板集成](https://www.cnblogs.com/coredx/p/12318135.html)
5. Identity Core 隐私数据保护。相关博客：[Asp.Net Core Identity 隐私数据保护](https://www.cnblogs.com/coredx/p/12210232.html)
6. Identity Core 双因素验证（两步登录）。在手机上配合 Microsoft Authenticator 或 Google Authenticator 应用使用。
7. gRPC 服务的编写和使用，配合 GrpcClient。
8. Web 全球化支持；网页本地化翻译管理。
9. 混合型字符串全球化与本地化支持。同时使用基于 resx 资源和基于数据库的字符串本地化功能。优先使用 resx 资源，查找失败后使用数据库数据。相关博客：[Asp.Net Core 混合全球化与本地化支持](https://www.cnblogs.com/coredx/p/12271537.html)
10. 独立的 EF Core 迁移程序集与全自动迁移和数据初始化。参考：[创建迁移](src/DemoApp/IdentityServer/EFCoreMigrationReadme.md "创建迁移")。
11. EF Core 全自动软删除查询过滤。
12. 把 asp.net core Web 应用包装为 Windows 桌面应用。相关博客：[.Net Core 3 骚操作 之 用 Windows 桌面应用开发 Asp.Net Core 网站](https://www.cnblogs.com/coredx/p/12031514.html)
13. 使用 Roslyn 运行 C# 脚本（类似 Try.Net 那种）；使用 ILSpy 反编译 .net 类型。使用 monaco editor 展示和编辑代码。相关博客：[C# 编译器 和 反编译器，你要哪个（歪头）？ 我全都要（捏拳）！](https://www.cnblogs.com/coredx/p/12045104.html)
14. 多版本 Web Open Api 支持，IdentityServer4 OAuth 2.0 集成。
15. Swagger 多版本 Api 浏览和调试，OAuth 2.0 支持。
16. Mvc 终结点列表浏览。
17. DI 中注册的服务列表浏览。
18. 文件浏览。
19. 应用健康检查。
20. 集成直播服务。（存在缺陷，仅支持 FFmpeg 命令行调用）
21. Razor Page 使用示例。
22. FluentValidation、Razor Page、多语言集成综合示例。
23. SignalR 实时聊天示例；集成SignalR、Web 控制器互操作示例；集成泛型 Hub 示例。
24. 程序集动态载入、调用、卸载示例（.net core 3.0 新增功能）。
25. 数独求解器网页集成示例；基于 Vue 的数据绑定。相关博客：[C# 数独求解算法。](https://www.cnblogs.com/coredx/p/12173702.html)
26. 无界面浏览器使用示例，基于 Puppeteer，Chromium79.
27. video.js 使用和扩展示例；集成 CCL 弹幕引擎，集成 flv.js 实现 HTML5 直接支持 flv 格式视频的播放。
28. 基于神经网络的光学字符识别使用示例。
29. 方便快捷的数据库说明扩展。相关博客：[EntityFramework Core 2.x/3.x （ef core） 在迁移中自动生成数据库表和列说明](https://www.cnblogs.com/coredx/p/10026783.html)
30. Razor 视图渲染为字符串服务示例。
31. MiniProfiler 示例。
32. Blazor WebAssembly 示例。相关博客：[浏览器中的 .Net Core —— Blazor WebAssembly 初体验](https://www.cnblogs.com/coredx/p/12342936.html)
33. 动态生成 Linq Where 表达式（使用 JqGrid 高级查询的数据结构）。相关博客：[动态构造任意复杂的 Linq Where 表达式](https://www.cnblogs.com/coredx/p/12423929.html)
34. 基于内置依赖注入框架的动态代理扩展。
35. 基于 Blazor WebAssembly 的 gRPC Web Client 使用示例。
#### 还有更多……

### IdentityServerGui
1. .net core DI 与 WPF 集成。
2. PropertyChanged.Fody 集成。
3. 通知栏支持。
#### 还有更多……
