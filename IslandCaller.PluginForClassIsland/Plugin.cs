using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using IslandCaller.Models;
using IslandCaller.Services.IslandCallerHostService;
using IslandCaller.Services.NotificationProvidersNew;
using IslandCaller.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace IslandCaller;

[PluginEntrance]
public class Plugin : PluginBase
{
    public static string PlugincfgFolder;
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        PlugincfgFolder = PluginConfigFolder;
        services.AddHostedService<IslandCallerHostService>();
        services.AddNotificationProvider<IslandCallerNotificationProviderNew>();

        AppBase.Current.AppStarted += (_, _) =>
        {
            new Models.Settings().Load();
            CoreDll.DllInit(
            Path.Combine(PlugincfgFolder, "default.txt"),
            true);
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Plugins\\Plugin.IslandCaller", "Wpf.Ui.dll");
            if (File.Exists(dllPath))
            {
                Assembly.LoadFrom(dllPath);
            }
            else
            {
                MessageBox.Show("未找到 Wpf.Ui.dll at " + dllPath);
            }
            
            if (Settings.Instance.Hover.IsEnable)
            {
                new HoverFluent().Show();
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
