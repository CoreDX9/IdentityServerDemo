# English [中文](README.md "中文")
# IdentityServerDemo
This is a .net core comprehensive learning and sample collection project that contains basic usage and comprehensive application examples for various functions. The goal is to show as quickly and easily as possible the use and practical effects of various functions.
<br> Current framework version: .Net Core 3.1
<br> VS 2019 version: 16.6.0 +
<br> Initial account: `admin`、`coredx`、`alice`、`bob`
<br> Initial password: `Pass123$`

## Executable Sample Program
1. DemoApp
   1. Client: C # oidc client example.
   2. CSharpScriptDemo: An example of running C# script using Roslyn.
   3. DomianSample: temporary test code.
   4. GrpcClient: Console gRPC client example.
   5. IdentityServer: The main project, the server of all clients. Includes most examples.
   6. IdentityServerGui: Graphical shell of IdentityServer. .net core 3.x Windows Desktop Application Development Example (WPF).
   7. JavascriptClient: js oidc client example.
   8. SudokuTset: Sudoku solver test code and usage examples.
   9. WebAPI mvc oidc client example.
   10. WebClient: Obsolete code archive.
   11. GraphicDemo: Examples of Windows window screenshots and screenshots.
   12. vJoyDemo: Example of virtual gamepad usage.
2. IdentityServerAdmin
   1. IdentityServer.Admin: IdentityServer4 Web management interface. Depends on IdentityServer.
   2. IdentityServer.Admin.Api: IdentityServer4 Web Api. Depends on IdentityServer.

## Instructions for Running
### Debugging
1. Download both projects and open IdentityServerDemo solution: <br> ``` https://github.com/CoreDX9/IdentityServerDemo.git ``` <br> ``` https://github.com/CoreDX9/Harmonic.git ```
2. Unload projects :<br>```Data/Domain.EntityFrameworkCore```<br>```Data/EntityHistoryMQReceive```<br>```Data/Repository```<br>```DemoApp/WebClient```<br>```Infrastructure/ResourceOwnerClient```
3. Fix project references for Infrastructure/Harmonic.
4. Wait for VS2019 to automatically restore Nuget and npm packages. If the automatic restore of the npm package fails, try running the npm install command in cmd. Make sure Node.js is installed.
5. Build project ```DemoPlugin/Plugin1```。
6. Set IdentityServer as the default startup project and set the startup mode to self-hosted.
7. Modify the database connection strings in IdentityServer/appsettings.json and appsettings.Development.json according to your actual situation. Currently only MS SqlServer is supported. If you need other database support, please modify the code yourself.
8. Run the IdentityServer project. Start experimenting and exploring various examples. All examples can be found in the navigation bar at the top of the web page. If the webpage is displaying abnormally, try resizing the browser window or adjusting the page zoom ratio.
9. Run other executable program projects that interest you.

### Release
1. Follow the previous instructions to ensure that commissioning can be started normally.
2. Right-click on the IdentityServer project and select Publish, select FolderProfileRelease Publish Configuration, and publish the project. If everything works, you can find the .publish folder in the solution folder.
3. Right-click on the Plugin1 project and select Publish to publish the project.
4. Right-click on the IdentityServerGui project and select Publish to publish the project. If all goes well, you can find IdentityServerGui.exe in the release location of IdentityServer.
5. Modify the database connection strings of IdentityServer's appsettings.json and appsettings.Production.json according to your actual situation.
6. Double-click IdentityServer.exe or IdentityServerGui.exe to run the application.

### Precautions
1. Try to use a higher version of the browser and try not to use IE.
2. Modify the settings of appsettings.json according to the situation.
3. Limited time and energy, unable to fully test the program, and the update of the framework or package version may lead to abnormal functions. If found, please feel free to raise issues. I will try to fix it.
4. The IdentityServer project contains a self-signed CA root certificate and website certificate. Self-signed certificates are used by default in publishing mode and issued to the localhost domain name to ensure that the Https protocol can be used normally in publishing mode.
5. After publishing the project using the configuration I provided, it can be run directly on a freshly installed Windows 7 Sp1 and later system without any dependencies, provided that it is set to use an in-memory database in the configuration, which is convenient for running the example quickly.
6. The database is redirected to the App_Data/Database folder of the IdentityServer project by default. At the same time, in order to make the programs in debug mode and release mode not interfere with each other, the database name includes the running environment suffix by default.
---
## Examples included
### IdentityServer
1. Customize project release configuration, and realize fully automated release by modifying csproj project files.
2. JQuery, Bootstrap4, Lay UI, Vue, Element-UI, Avue, Axios, Monaco-editor and many other common front-end frameworks and libraries.
3. Use of IdentityServer4 and Identity Core; SSO integration support.
4. Identity Core management; IdentityServer4 management.
5. Identity Core privacy data protection.
6. Identity Core two-factor authentication (two-step login). Use with Microsoft Authenticator or Google Authenticator app on your phone.
7. Writing and using gRPC service, cooperate with GrpcClient.
8. Web globalization support; webpage localization translation management.
9. Hybrid string globalization and localization support. Use both resx-based resources and database-based string localization. Use resx resources first, and use database data if the lookup fails.
10. Independent EF Core migration assembly with fully automatic migration and data initialization. Reference: [Create Migration](src/DemoApp/IdentityServer/EFCoreMigrationReadme.md "Create Migration").
11. EF Core automatic soft delete query filtering.
12. Package the asp.net core Web application as a Windows desktop application.
13. Use Roslyn to run C # scripts (similar to Try.Net); use ILSpy to decompile .net types. Use the monaco editor to display and edit the code.
14. Multi-version Web Open Api support, IdentityServer4 OAuth 2.0 integration.
15. Swagger multi-version Api browsing and debugging, OAuth 2.0 support.
16. Mvc endpoint list browsing.
17. Browse the list of services registered in DI.
18. File browsing.
19. Apply a health check.
20. Integrated Live Broadcasting Services. (Defective, only supports FFmpeg command line calls)
21. Razor Page usage example.
22. FluentValidation, Razor Page, comprehensive examples of multilingual integration.
23. SignalR live chat example; integrated SignalR, web controller interoperability example; integrated generic Hub example.
24. Assembly dynamic loading, calling, and unloading examples (new in .net core 3.0).
25. Sudoku solver webpage integration example; Vue-based data binding.
26. Example of using interfaceless browser, based on Puppeteer, Chromium79.
27. Video.js usage and extension examples; integrated with CCL Danmaku Engine and integrated with flv.js for HTML5 to directly support the playback of video in flv format.
28. Examples of use of neural network-based optical character integration.
29. Convenient and fast database description extension.
30. Razor view render to string example.
31. MiniProfiler example.
32. Blazor WebAssembly example.
33. Generate Linq Where expressions dynamically (data structures using JqGrid advanced queries).
34. Dynamic proxy extension based on built-in dependency injection framework.
#### and more...

### IdentityServerGui
1. .net core DI integration with WPF.
2. PropertyChanged.Fody integration.
3. Notification bar support.
#### and more...
