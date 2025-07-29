using ClassIsland.Core;
using ClassIsland.Core.Commands;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using IslandCaller.Services.NotificationProvidersNew;
using IslandCaller.Views.SettingsPages;
using IslandCaller.Models;

namespace IslandCaller.Views.Windows;

public partial class Hover : Window
{
    public static Hover? Instance;

    public Plugin Plugin { get; }
    public Hover(Plugin plugin)
    {
        Plugin = plugin;

        // 这两个属性绑定不生效,于是直接在初始化前设定
        this.Left = plugin.Settings.HoverPosition.X;
        this.Top = plugin.Settings.HoverPosition.Y;

        InitializeComponent();
        DataContext = this;

        // 根据设置控制窗口的显示
        if (!Plugin.Settings.IsHoverShow)
        {
            this.Hide();
        }
        else
        {
            this.Show();
        }

        // 监听设置变化
        Plugin.Settings.PropertyChanged += Settings_PropertyChanged;
    }

    private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.IsHoverShow))
        {
            if (Plugin.Settings.IsHoverShow)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }
    }

    void Hover_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    void Hover_OnMouseLeave(object sender, MouseEventArgs mouseEventArgs)
    {
        Plugin.Settings.HoverPosition = new Settings.HoverPositionData
        {
            X = this.Left,
            Y = this.Top
        };
    }

    void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        RandomCall(null, EventArgs.Empty);
    }

    private void RandomCall(object? sender, EventArgs e)
    {
        new IslandCallerNotificationProviderNew().RandomCall(1);
    }
}
