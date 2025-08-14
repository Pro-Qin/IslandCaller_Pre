using ClassIsland.Core.Abstractions.Services;
using ClassIsland.Core.Abstractions.Services.NotificationProviders;
using ClassIsland.Core.Attributes;
using ClassIsland.Core.Models.Notification;
using ClassIsland.Shared.Enums;
using ClassIsland.Shared.Interfaces;
using IslandCaller.Controls.NotificationProviders;
using IslandCaller.Views.Windows;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using IslandCaller.Views.Windows;

namespace IslandCaller.Services.NotificationProvidersNew;

[NotificationProviderInfo(
    "9B570BF1-9A32-40C0-9D5D-4FFA69E03A37",
    "IslandCallerServices",
    PackIconKind.AccountCheck,
    "用于为IslandCaller提供通知接口")]
public class IslandCallerNotificationProviderNew : NotificationProviderBase
{
    public async void RandomCall(int stunum)
    {
        IntPtr ptr1 = CoreDll.GetRandomStudentName(stunum);
        string output = Marshal.PtrToStringBSTR(ptr1);
        Marshal.FreeBSTR(ptr1); // 释放分配的 BSTR 内存
        int maskduration = stunum * 2 + 1; // 计算持续时间
        ShowNotification(new NotificationRequest()
        {
            MaskContent = new NotificationContent()
            {
                Content = new IslandCallerNotificationControl(output),
                Duration = new TimeSpan(0, 0, maskduration),
                IsSpeechEnabled = true,
                SpeechContent = output,
            }
        });
        var fluentShower = new FluentShower(output, stunum);
        fluentShower.Show();
        await Task.Delay(maskduration * 1000); // 等待指定的持续时间
        fluentShower.Close();
    }
}
