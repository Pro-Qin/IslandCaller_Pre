using IslandCaller.Services.NotificationProvidersNew;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
namespace IslandCaller.Views.Windows
{
    /// <summary>
    /// HoverFluent.xaml 的交互逻辑
    /// </summary>
    public partial class HoverFluent : System.Windows.Window
    {
        public HoverFluent()
        {
            InitializeComponent();
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.EnableFluentWindow(this); // 设置模糊 + 圆角
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            source.CompositionTarget.BackgroundColor = Colors.Transparent;
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove(); // 系统窗口拖动
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MainButton.IsEnabled = false;
            img.Opacity = 0.5;
            new IslandCallerNotificationProviderNew().RandomCall(1);
            await Task.Delay(3000);
            MainButton.IsEnabled = true;
            img.Opacity = 1.0;
        }
    }
}
