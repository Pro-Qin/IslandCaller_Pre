using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using ClassIsland.Shared.Helpers;
using IslandCaller.Models;
using IslandCaller.Services.NotificationProviders;
using IslandCaller.Views.SettingsPages;
using IslandCaller.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Runtime.InteropServices;

namespace IslandCaller;

[PluginEntrance]
public class Plugin : PluginBase
{
    public Settings Settings { get; set; } = new();
    public static string PlugincfgFolder;

    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        PlugincfgFolder = PluginConfigFolder;
        services.AddHostedService<IslandCallerNotificationProvider>();
        services.AddSettingsPage<IslandCallerSettingsPage>();
        Settings = ConfigureFileHelper.LoadConfig<Settings>(Path.Combine(PluginConfigFolder, "Settings.json"));
        Settings.PropertyChanged += (sender, args) =>
        {
            ConfigureFileHelper.SaveConfig<Settings>(Path.Combine(PluginConfigFolder, "Settings.json"), Settings);
        };

        AppBase.Current.AppStarted += (_, _) =>
        {
            // Declare the external function outside the local function scope
            CoreDll.DllInit(
            Path.Combine(PlugincfgFolder, "default.txt"),
            Settings.IsAntiRepeatEnabled
        );
            if (Settings.IsHoverShow)
            {
                Hover.Instance ??= new Hover(this);
                Hover.Instance.Show();
            }
        };
    }
}

public static class CoreDll
{
    // 导入初始化函数
    [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern int DllInit(
        [MarshalAs(UnmanagedType.LPWStr)] string filename,
        bool isAntiRepeat);

    // 导入获取随机学生名称的函数
    [DllImport(".\\Plugins\\Plugin.IslandCaller\\Core.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern IntPtr GetRandomStudentName(int number);
}
