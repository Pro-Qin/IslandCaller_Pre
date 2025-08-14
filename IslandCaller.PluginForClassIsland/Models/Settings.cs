using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using static WindowHelper;

namespace IslandCaller.Models
{
    public class Settings
    {
        private static SettingsModel _instance = new SettingsModel();
        public static event Action<SettingsModel>? SettingsChanged;

        public static SettingsModel Instance
        {
            get => _instance;
            set
            {
                _instance = value;
                SettingsChanged?.Invoke(value);
            }
        }

        public void Load()
        {
            RegistryKey IsC_RootKey = Registry.CurrentUser.OpenSubKey(@"Software\IslandCaller", writable: true);
            RegistryKey IsC_GeneralKey;
            RegistryKey IsC_ProfileKey;
            RegistryKey IsC_HoverKey;
            RegistryKey IsC_SecurityKey;
            RegistryKey IsC_HoverKey_Position;
            RegistryKey IsC_SecurityKey_SecretKey;

            if (IsC_RootKey == null)
            {
                IsC_RootKey = Registry.CurrentUser.CreateSubKey(@"Software\IslandCaller", writable: true);
                IsC_GeneralKey = IsC_RootKey?.CreateSubKey("General", writable: true);
                IsC_ProfileKey = IsC_RootKey?.CreateSubKey("Profile", writable: true);
                IsC_HoverKey = IsC_RootKey?.CreateSubKey("Hover", writable: true);
                IsC_SecurityKey = IsC_RootKey?.CreateSubKey("Security", writable: true);
                IsC_HoverKey_Position = IsC_HoverKey?.CreateSubKey("Position", writable: true);
                IsC_SecurityKey_SecretKey = IsC_SecurityKey?.CreateSubKey("SecretKey", writable: true);

                IsC_GeneralKey?.SetValue("Version", Instance.General.Version);
                IsC_HoverKey?.SetValue("IsEnable", Instance.Hover.IsEnable);
                IsC_HoverKey_Position?.SetValue("X", Instance.Hover.Position.X);
                IsC_HoverKey_Position?.SetValue("Y", Instance.Hover.Position.Y);
                IsC_SecurityKey?.SetValue("EncryptionMode", Instance.Security.EncryptionMode);
                IsC_SecurityKey_SecretKey?.SetValue("AESKey", Instance.Security.SecretKey.AESKey);
                IsC_SecurityKey_SecretKey?.SetValue("TOTPKey", Instance.Security.SecretKey.TOTPKey);
                IsC_SecurityKey_SecretKey?.SetValue("Passkey", Instance.Security.SecretKey.Passkey);
                IsC_SecurityKey_SecretKey?.SetValue("Password", Instance.Security.SecretKey.Password);
                IsC_SecurityKey_SecretKey?.SetValue("USBKey", Instance.Security.SecretKey.USBKey);

                MessageBox.Show("Welcome!");
            }
            else
            {
                IsC_GeneralKey = IsC_RootKey?.OpenSubKey("General", writable: true);
                IsC_ProfileKey = IsC_RootKey?.OpenSubKey("Profile", writable: true);
                IsC_HoverKey = IsC_RootKey?.OpenSubKey("Hover", writable: true);
                IsC_SecurityKey = IsC_RootKey?.OpenSubKey("Security", writable: true);
                IsC_HoverKey_Position = IsC_HoverKey?.OpenSubKey("Position", writable: true);
                IsC_SecurityKey_SecretKey = IsC_SecurityKey?.OpenSubKey("SecretKey", writable: true);

                Instance.Hover.IsEnable = bool.TryParse(IsC_HoverKey?.GetValue("IsEnable")?.ToString(), out var b) ? b : false;
                Instance.Hover.Position.X = Convert.ToDouble(IsC_HoverKey_Position?.GetValue("X") ?? 200.0);
                Instance.Hover.Position.Y = Convert.ToDouble(IsC_HoverKey_Position?.GetValue("Y") ?? 200.0);
                Instance.Security.EncryptionMode = Convert.ToInt32(IsC_SecurityKey?.GetValue("EncryptionMode") ?? 0);
                Instance.Security.SecretKey.AESKey = IsC_SecurityKey_SecretKey?.GetValue("AESKey") as string ?? string.Empty;
                Instance.Security.SecretKey.TOTPKey = IsC_SecurityKey_SecretKey?.GetValue("TOTPKey") as string ?? string.Empty;
                Instance.Security.SecretKey.Passkey = IsC_SecurityKey_SecretKey?.GetValue("Passkey") as string ?? string.Empty;
                Instance.Security.SecretKey.Password = IsC_SecurityKey_SecretKey?.GetValue("Password") as string ?? string.Empty;
                Instance.Security.SecretKey.USBKey = IsC_SecurityKey_SecretKey?.GetValue("USBKey") as string ?? string.Empty;
            }

            SettingsBinder.Bind(Instance, Save);
        }

        public void Save()
        {
            RegistryKey IsC_RootKey = Registry.CurrentUser.OpenSubKey(@"Software\IslandCaller", writable: true);
            RegistryKey IsC_HoverKey = IsC_RootKey?.OpenSubKey("Hover", writable: true);
            RegistryKey IsC_HoverKey_Position = IsC_HoverKey?.OpenSubKey("Position", writable: true);
            RegistryKey IsC_SecurityKey = IsC_RootKey?.OpenSubKey("Security", writable: true);
            RegistryKey IsC_SecurityKey_SecretKey = IsC_SecurityKey?.OpenSubKey("SecretKey", writable: true);

            IsC_HoverKey?.SetValue("IsEnable", Instance.Hover.IsEnable);
            IsC_HoverKey_Position?.SetValue("X", Instance.Hover.Position.X);
            IsC_HoverKey_Position?.SetValue("Y", Instance.Hover.Position.Y);
            IsC_SecurityKey?.SetValue("EncryptionMode", Instance.Security.EncryptionMode);
            IsC_SecurityKey_SecretKey?.SetValue("AESKey", Instance.Security.SecretKey.AESKey);
            IsC_SecurityKey_SecretKey?.SetValue("TOTPKey", Instance.Security.SecretKey.TOTPKey);
            IsC_SecurityKey_SecretKey?.SetValue("Passkey", Instance.Security.SecretKey.Passkey);
            IsC_SecurityKey_SecretKey?.SetValue("Password", Instance.Security.SecretKey.Password);
            IsC_SecurityKey_SecretKey?.SetValue("USBKey", Instance.Security.SecretKey.USBKey);
        }
    }
    public static class SettingsBinder
    {
        public static void Bind(SettingsModel model, Action onChange)
        {
            // General
            model.General.PropertyChanged += (_, _) => onChange();

            // Hover
            model.Hover.PropertyChanged += (_, _) => onChange();
            model.Hover.Position.PropertyChanged += (_, _) => onChange();

            // Security
            model.Security.PropertyChanged += (_, _) => onChange();
            model.Security.SecretKey.PropertyChanged += (_, _) => onChange();
        }
    }

}