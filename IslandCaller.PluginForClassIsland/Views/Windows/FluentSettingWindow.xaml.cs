using iNKORE.UI.WPF.Modern.Controls;
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
using System.Windows.Shapes;

namespace IslandCaller.Views.Windows
{
    /// <summary>
    /// FluentSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FluentSettingWindow : Window
    {
        public FluentSettingWindow()
        {
            InitializeComponent();
        }
        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            rootFrame.Navigate(args.SelectedItemContainer.Tag as Type);
            rootNav.Header = (args.SelectedItemContainer.Content as string) + "设置";
        }
        private void OnNavigationViewLoaded(object sender, RoutedEventArgs e)
        {
            rootNav.SelectedItem = rootNav.MenuItems[0];
        }
    }
}
