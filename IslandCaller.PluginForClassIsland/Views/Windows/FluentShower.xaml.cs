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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IslandCaller.Views.Windows
{
    /// <summary>
    /// FluentShower.xaml 的交互逻辑
    /// </summary>
    public partial class FluentShower : Window
    {
        public FluentShower(string name, int num)
        {
            var control = new Controls.FluentShower.FluentShowerControl(name)
            {
                Height = 95,
                Margin = new Thickness(10, 0, 15, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            InitializeComponent();
            this.Width = 510 + (num - 1) * 248;
            ShowerGrid.Children.Add(control);
        }
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowHelper.EnableFluentWindow(this, 0x7FFF628B); // 设置模糊 + 圆角
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            source.CompositionTarget.BackgroundColor = Colors.Transparent;
        }
    }
}
