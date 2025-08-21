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
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = (int)NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE);
            exStyle |= NativeMethods.WS_EX_NOACTIVATE;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_EXSTYLE, (IntPtr)exStyle);
        }
        public HoverFluent()
        {
            InitializeComponent();
            this.Left = Models.Settings.Instance.Hover.Position.X;
            this.Top = Models.Settings.Instance.Hover.Position.Y;
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.EnableFluentWindow(this, 0x00FFFFFF); // 设置模糊 + 圆角
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            source.CompositionTarget.BackgroundColor = Colors.Transparent;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
                Models.Settings.Instance.Hover.Position.X = this.Left;
                Models.Settings.Instance.Hover.Position.Y = this.Top;
            }
        }

        private void MainButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
                if (Models.Settings.Instance.Hover.Position.X == this.Left || Models.Settings.Instance.Hover.Position.Y == this.Top)
                {
                    MainButton_Click(sender,e);
                }
                else
                {
                    Models.Settings.Instance.Hover.Position.X = this.Left;
                    Models.Settings.Instance.Hover.Position.Y = this.Top;
                }
            }
        }

        private async void MainButton_Click(object sender, RoutedEventArgs e)
        {
            MainButton.IsEnabled = false;
            img.Opacity = 0.5;
            new IslandCallerNotificationProviderNew().RandomCall(1);
            await Task.Delay(3000);
            MainButton.IsEnabled = true;
            img.Opacity = 1.0;
        }
        private void SecondaryButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
                if (Models.Settings.Instance.Hover.Position.X == this.Left || Models.Settings.Instance.Hover.Position.Y == this.Top)
                {
                    SecondaryButton_Click(sender, e);
                }
                else
                {
                    Models.Settings.Instance.Hover.Position.X = this.Left;
                    Models.Settings.Instance.Hover.Position.Y = this.Top;
                }
            }
        }
        private void SecondaryButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    
internal static class NativeMethods
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x08000000;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }

}
