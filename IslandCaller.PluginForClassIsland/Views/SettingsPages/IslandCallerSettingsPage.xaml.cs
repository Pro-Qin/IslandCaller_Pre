using ClassIsland.Core.Abstractions.Controls;
using System.Windows;
using ClassIsland.Core.Attributes;
using MaterialDesignThemes.Wpf;
using System.IO;
using IWshRuntimeLibrary;
using ClassIsland.Core.Controls.CommonDialog;
using System.Diagnostics;
using IslandCaller.Models;
namespace IslandCaller.Views.SettingsPages;

/// <summary>
/// IslandCallerSettingsPage.xaml 的交互逻辑
/// </summary>
[SettingsPageInfo("IslandCaller.IslandCallerSettingsPage", "IslandCaller 设置页面", PackIconKind.AccountCheckOutline, PackIconKind.AccountCheck)]
public partial class IslandCallerSettingsPage : SettingsPageBase
{
    public Plugin Plugin { get; }

    public IslandCallerSettingsPage(Plugin plugin)
    {
        Plugin = plugin;
        InitializeComponent();
        DataContext = this;
    }

    private void ButtonCreateLnk_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // 定义快捷方式的路径
            string shortcutPath = System.IO.Path.Combine(desktopPath, "IslandCaller.lnk");

            // 创建 WshShell 对象
            WshShell shell = new WshShell();
            // 创建快捷方式
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            // 设置快捷方式的目标
            shortcut.TargetPath = "classisland://plugins/IslandCaller/Run";
            // 设置快捷方式的描述
            shortcut.Description = "IslandCaller 插件快捷方式";
            // 设置快捷方式的图标（可选）
            string iconName = @"Plugins\Plugin.IslandCaller\Icon.ico";
            string currentDirectory = Directory.GetCurrentDirectory();
            // 构建文件路径
            string iconPath = System.IO.Path.Combine(currentDirectory, iconName);
            shortcut.IconLocation = iconPath;
            // 保存快捷方式
            shortcut.Save();
            CommonDialog.ShowInfo("成功创建了快捷方式。");
        }
        catch (System.Exception ex)
        {
            CommonDialog.ShowError($"无法创建快捷方式：{ex.Message}");
        }
    }

    private void ButtonEditNamelist_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            string fileName = @"Config\Plugins\Plugin.IslandCaller\default.txt";
            string currentDirectory = Directory.GetCurrentDirectory();
            // 构建文件路径
            string filePath = System.IO.Path.Combine(currentDirectory, fileName);

            // 检查文件是否存在
            if (!System.IO.File.Exists(filePath))
            {
                // 文件不存在，则创建文件
                using (FileStream fs = System.IO.File.Create(filePath))
                {
                    CommonDialog.ShowInfo("名单不存在，已创建新名单！");
                }
            }

            if (System.IO.File.Exists(filePath))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("notepad.exe " + filePath)
                    {
                        UseShellExecute = false, // 必须为 false 才能启用事件
                    },
                    EnableRaisingEvents = true
                };
                process.Exited += (sender, e) =>
                {
                    // 在这里写进程关闭后的代码
                    CoreDll.DllInit(
                        System.IO.Path.Combine(Plugin.PlugincfgFolder, "default.txt"),true);
                };

                process.Start();
                CommonDialog.ShowInfo("成功打开名单。关闭前记得保存哦！");
            }
        }
        catch (System.Exception ex)
        {
            CommonDialog.ShowError($"无法打开名单：{ex.Message}");
        }
    }

}
