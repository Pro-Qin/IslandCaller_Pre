using ClassIsland.Core;
using ClassIsland.Core.Abstractions;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Extensions.Registry;
using IslandCaller.Models;
using IslandCaller.PluginForClassIsland.Models;
using IslandCaller.Services.IslandCallerHostService;
using IslandCaller.Services.NotificationProvidersNew;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
namespace IslandCaller;

[PluginEntrance]
public class Plugin : PluginBase
{
    public static string PlugincfgFolder;
    public override void Initialize(HostBuilderContext context, IServiceCollection services)
    {
        Log.WriteLog("Plugin.cs", "Debug", "IslandCaller Plugin Initializing");
        PlugincfgFolder = PluginConfigFolder;
        services.AddHostedService<IslandCallerHostService>();
        services.AddNotificationProvider<IslandCallerNotificationProviderNew>();

        AppBase.Current.AppStarted += (_, _) =>
        {
            new Models.Settings().Load();
            if (Settings.Instance.Profile.ProfileList.TryGetValue(Settings.Instance.Profile.DefaultProfile, out string value))
            {
                if (Core.RandomImport(value) == 0) Log.WriteLog("Plugin.cs", "Success", "Core Initialized");
                else Log.WriteLog("Plugin.cs", "Error", "Core Initialize Failed");
            }
            else
            {
                Log.WriteLog("Plugin.cs", "Error", "Profile not found");
            }
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins\\Plugin.IslandCaller", "iNKORE.UI.WPF.Modern.Controls");
            if (File.Exists(dllPath))
            {
                Assembly.LoadFrom(dllPath);
            }

            if (Settings.Instance.Hover.IsEnable)
            {
                Status.Instance.fluenthover.Show();
            }
            
        };
    }

}
