using iNKORE.UI.WPF.Modern.Controls;
using IslandCaller.Services.NotificationProvidersNew;
using System.Net.NetworkInformation;
using System.Windows;
using Ring = iNKORE.UI.WPF.Modern.Controls.ProgressRing;

namespace IslandCaller.Views.Windows
{
    /// <summary>
    /// FluentSettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FluentCallerGUI : Window
    {
        public FluentCallerGUI()
        {
            var ring = new Ring
            {
                IsActive = true,
            };
            InitializeComponent();
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Random_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new IslandCallerNotificationProviderNew().RandomCall((int)NumSlider.Value);
        }
    }
}
