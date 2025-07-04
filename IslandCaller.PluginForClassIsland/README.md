<!--markdownlint-disable MD001 MD033 MD041 MD051-->

<div align="center">


# IslandCaller

IslandCaller 是一个基于 ClassIsland 提醒服务的轻量级点名器，用于在课堂上快速点名。
[![Stars](https://img.shields.io/github/stars/HUSX100/IslandCaller?label=Stars)](https://github.com/HUSX100/IslandCaller)
[![正式版 Release](https://img.shields.io/github/v/release/HUSX100/IslandCaller?style=flat-square&color=%233fb950&label=正式版)](https://github.com/HUSX100/IslandCaller/releases/latest)
[![下载量](https://img.shields.io/github/downloads/HUSX100/IslandCaller/total?style=social&label=下载量&logo=github)](https://github.com/HUSX100/IslandCaller/releases/latest)
[![GitHub Repo Languages](https://img.shields.io/github/languages/top/HUSX100/IslandCaller?style=flat-square)](https://github.com/HUSX100/IslandCaller/search?l=c%23)
</div>

## 特性

- 快速点名：输入班级名单后，可以通过桌面快捷方式快速点名。
- 简单易用：通过插件商店下载并进行基本配置即可使用。
- 集成 ClassIsland：依赖 ClassIsland 的提醒服务，通过注册 URL 协议实现快速点名。

## 功能

### 基本点名功能

- [x] 自定义名单
- [x] 快捷随机抽人

### 高级点名功能

- [x] 指定人数
- [ ] 指定性别
- [ ] 指定学号区间
- [x] 防名单重复
- [ ] 每节课防止重复抽取

### 点名方式

- [x] 桌面快捷方式
- [x] 悬浮窗
- [ ] 高级悬浮窗

### 联动功能

- [x] 与[智绘教Inkeys](https://github.com/Alan-CRL/Inkeys)联动(需升级至正式版20250503a及以上)


## 使用方法

1. **下载插件：** 从 ClassIsland 插件商店下载 `IslandCaller` 插件。

2. **创建快捷方式：**
   在插件设置中创建桌面快捷方式，或者自行创建一个指向 `classisland://plugins/IslandCaller/Run` 的快捷方式。

3. **输入并保存班级名单：**
   在设置中输入你的班级名单，每行一个学生姓名，并保存设置。

4. **点名：**
   - 点击桌面上的快捷方式，快捷点名。
   - 在插件设置界面开启悬浮窗后可使用悬浮窗快捷点名。

## 注意事项

- 确保 ClassIsland 本体的“注册 Url 协议”设置已开启，否则无法启动插件。

## 致谢

本项目使用了以下第三方库：

- [ClassIsland.PluginSdk](https://github.com/ClassIsland/ClassIsland/)

## 许可

本项目使用 GPL3 许可证进行开源，详细信息请查看 [LICENSE](LICENSE) 文件。

---

如果你在使用过程中遇到任何问题，欢迎提交 [Issue](https://github.com/HUSX100/IslandCaller/issues)。

---

感谢你使用 IslandCaller，希望它能为你的课堂管理带来便利！
