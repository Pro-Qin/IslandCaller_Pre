using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace IslandCaller.Models
{
    public class SettingsModel
    {
        public GeneralSetting General { get; set; } = new GeneralSetting();
        public ProfileSetting Profile { get; set; } = new ProfileSetting();
        public HoverSetting Hover { get; set; } = new HoverSetting();
        public SecuritySetting Security { get; set; } = new SecuritySetting();
    }

    public class GeneralSetting : INotifyPropertyChanged
    {
        private string _version = "1.0.7.0";
        public string Version
        {
            get => _version;
            set { if (_version != value) { _version = value; OnPropertyChanged(nameof(Version)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class ProfileSetting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class HoverSetting : INotifyPropertyChanged
    {
        private bool _isEnable = true;
        public bool IsEnable
        {
            get => _isEnable;
            set { if (_isEnable != value) { _isEnable = value; OnPropertyChanged(nameof(IsEnable)); } }
        }

        public PositionSetting Position { get; set; } = new PositionSetting();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class PositionSetting : INotifyPropertyChanged
    {
        private double _x = 200.0;
        public double X
        {
            get => _x;
            set { if (_x != value) { _x = value; OnPropertyChanged(nameof(X)); } }
        }

        private double _y = 200.0;
        public double Y
        {
            get => _y;
            set { if (_y != value) { _y = value; OnPropertyChanged(nameof(Y)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SecuritySetting : INotifyPropertyChanged
    {
        private int _encryptionMode = 0;
        public int EncryptionMode
        {
            get => _encryptionMode;
            set { if (_encryptionMode != value) { _encryptionMode = value; OnPropertyChanged(nameof(EncryptionMode)); } }
        }

        public SecretKeySetting SecretKey { get; set; } = new SecretKeySetting();
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class SecretKeySetting : INotifyPropertyChanged
    {
        private string _aesKey = string.Empty;
        public string AESKey
        {
            get => _aesKey;
            set { if (_aesKey != value) { _aesKey = value; OnPropertyChanged(nameof(AESKey)); } }
        }

        private string _totpKey = string.Empty;
        public string TOTPKey
        {
            get => _totpKey;
            set { if (_totpKey != value) { _totpKey = value; OnPropertyChanged(nameof(TOTPKey)); } }
        }

        private string _passkey = string.Empty;
        public string Passkey
        {
            get => _passkey;
            set { if (_passkey != value) { _passkey = value; OnPropertyChanged(nameof(Passkey)); } }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { if (_password != value) { _password = value; OnPropertyChanged(nameof(Password)); } }
        }

        private string _usbKey = string.Empty;
        public string USBKey
        {
            get => _usbKey;
            set { if (_usbKey != value) { _usbKey = value; OnPropertyChanged(nameof(USBKey)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}