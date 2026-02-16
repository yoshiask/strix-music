using System;
using System.IO;
using OwlCore.ComponentModel;
using OwlCore.Storage;
using StrixMusic.AppModels;

namespace StrixMusic.Settings
{
    public class OpenSubsonicCoreSettings : CoreSettingsBase, IInstanceId
    {
        public OpenSubsonicCoreSettings(IModifiableFolder folder) : base(folder, AppSettingsSerializer.Singleton)
        {
        }

        /// <inheritdoc />
        public override bool IsSettingValidForCoreCreation(string propertyName, object? value) => propertyName switch
        {
            nameof(ServerUrl) => Uri.IsWellFormedUriString(value?.ToString(), UriKind.Absolute),
            nameof(InstanceId) or nameof(Username) or nameof(Password)
                => !string.IsNullOrWhiteSpace((string?)value ?? string.Empty),
            _ => true,
        };

        /// <inheritdoc />
        public override object GetSettingByName(string settingName) => settingName switch
        {
            nameof(InstanceId) => InstanceId,
            nameof(ServerUrl) => ServerUrl,
            nameof(Username) => Username,
            nameof(Password) => Password,
            _ => throw new ArgumentOutOfRangeException(nameof(settingName), settingName, @"Unknown setting name specified.")
        };
        
        public string InstanceId
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }
        
        public string ServerUrl
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }
        
        public string Username
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }
        
        public string Password
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }
    }
}
