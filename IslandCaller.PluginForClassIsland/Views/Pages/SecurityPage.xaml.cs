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
using iNKORE.UI.WPF.Modern.Controls;
using IslandCaller.Models;
using IslandCaller.Views.Pages;
using IslandCaller.ViewModel.Pages;
using IslandCaller.Views.Windows;
using Page = iNKORE.UI.WPF.Modern.Controls.Page;
using Ring = iNKORE.UI.WPF.Modern.Controls.ProgressRing;

namespace IslandCaller.Views.Pages
{
    /// <summary>
    /// SecurityPage.xaml 的交互逻辑
    /// </summary>
    public partial class SecurityPage : Page
    {
        SecurityPageViewModel vm;
        public SecurityPage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                vm = (SecurityPageViewModel)this.DataContext;
                if (vm.EncryptionMode % 2 == 0)
                {
                    PIN_Button.Content = new TextBlock() { Text = "敬请期待", };
                    PIN_Button.Tag = "Create";
                }
                else
                {
                    PIN_Button.Content = new TextBlock() { Text = "删除", };
                    PIN_Button.Tag = "Delete";
                }
                if (vm.EncryptionMode == 0 || vm.EncryptionMode == 1 || vm.EncryptionMode == 4 || vm.EncryptionMode == 5 || vm.EncryptionMode == 8 || vm.EncryptionMode == 9 || vm.EncryptionMode == 12 || vm.EncryptionMode == 13)
                {
                    USB_Button.Content = new TextBlock() { Text = "敬请期待", };
                    USB_Button.Tag = "Create";
                }
                else
                {
                    USB_Button.Content = new TextBlock() { Text = "删除", };
                    USB_Button.Tag = "Delete";
                }
                if ((vm.EncryptionMode >= 0 && vm.EncryptionMode <= 3) || (vm.EncryptionMode >= 8 && vm.EncryptionMode <= 11))
                {
                    Hello_Button.Content = new TextBlock() { Text = "体验一下", };
                    Hello_Button.Tag = "Create";
                }
                else
                {
                    Hello_Button.Content = new TextBlock() { Text = "删除", };
                    Hello_Button.Tag = "Delete";
                }
                if (vm.EncryptionMode >= 0 && vm.EncryptionMode <= 7)
                {
                    TOTP_Button.Content = new TextBlock() { Text = "敬请期待", };
                    TOTP_Button.Tag = "Create";
                }
                else
                {
                    TOTP_Button.Content = new TextBlock() { Text = "删除", };
                    TOTP_Button.Tag = "Delete";
                }
            };
        }

        private void PIN_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Hello_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;

            if (btn != null && string.Equals(btn.Tag as string, "Create", StringComparison.Ordinal))
            {
                var ring = new Ring
                {
                    IsActive = true,
                };
                Hello_Button.IsEnabled = false;
                Hello_Button.Content = ring;
                if (await Core.CreateHelloPasskeyAsync())
                {
                    vm.EncryptionMode += 4;
                    Hello_Button.Content = new TextBlock()
                    {
                        Text = "删除",
                    };
                    Hello_Button.Tag = "Delete";
                    Hello_Button.IsEnabled = true;
                }
                else
                {
                    Hello_Button.Content = new TextBlock()
                    {
                        Text = "设置",
                    };
                    Hello_Button.Tag = "Create";
                    Hello_Button.IsEnabled = true;
                }
                return;
            }
            if (btn != null && string.Equals(btn.Tag as string, "Delete", StringComparison.Ordinal))
            {
                Settings.Instance.Security.SecretKey.Passkey = string.Empty;
                vm.EncryptionMode -= 4;
                Hello_Button.Content = new TextBlock()
                {
                    Text = "设置",
                };
                Hello_Button.Tag = "Create";
                Hello_Button.IsEnabled = true;
                return;
            }
        }
        private void USB_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TOTP_Button_Click(object sender, RoutedEventArgs e)
        {
            Button? btn = sender as Button;
            if (btn != null && string.Equals(btn.Tag as string, "Create", StringComparison.Ordinal))
            {
                var ring = new Ring
                {
                    IsActive = true,
                };
                TOTP_Button.Content = ring;
                TOTP_Button.IsEnabled = false;
                //Type createTOTP = typeof(TOTPCreate);
                //new FluentLogin(createTOTP).ShowDialog();
                vm.ReloadSettings();
                if(vm.EncryptionMode >= 9 && vm.EncryptionMode <= 15)
                {
                    TOTP_Button.Content = new TextBlock()
                    {
                        Text = "删除",
                    };
                    TOTP_Button.Tag = "Delete";
                    TOTP_Button.IsEnabled = true;
                    return;
                }
                else
                {
                    TOTP_Button.Content = new TextBlock()
                    {
                        Text = "设置",
                    };
                    TOTP_Button.Tag = "Create";
                    TOTP_Button.IsEnabled = true;
                    return;
                }
            }
            if (btn != null && string.Equals(btn.Tag as string, "Delete", StringComparison.Ordinal))
            {
                Settings.Instance.Security.SecretKey.TOTPKey = string.Empty;
                vm.EncryptionMode -= 8;
                TOTP_Button.Content = new TextBlock()
                {
                    Text = "设置",
                };
                TOTP_Button.Tag = "Create";
                TOTP_Button.IsEnabled = true;
                return;
            }
        }
    }
}
