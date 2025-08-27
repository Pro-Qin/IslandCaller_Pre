using ClassIsland.Core.Abstractions.Controls;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Enums.SettingsWindow;
using IslandCaller.Views.Windows;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IslandCaller.Views.Pages
{
    /// <summary>
    /// ClassIslandSettingPage.xaml 的交互逻辑
    /// </summary>
    [SettingsPageInfo(
    "entrance.IslandCallerSetting",   // 设置页面 id
    "IslandCaller 设置页面",  // 设置页面名称
    PackIconKind.AccountCheckOutline,   // 未选中时设置页面图标
    PackIconKind.AccountCheck,  // 选中时设置页面图标
    SettingsPageCategory.External  // 设置页面类别
)]
    public partial class ClassIslandSettingPage : SettingsPageBase
    { 
        public ClassIslandSettingPage()
        {
            InitializeComponent();
            new FluentSettingWindow().Show();
        }
    }
}
