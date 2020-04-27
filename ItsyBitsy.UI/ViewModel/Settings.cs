using ItsyBitsy.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ItsyBitsy.UI
{
    [Serializable]
    public sealed class Settings : ISettings
    {
        const string SettingsFile = "settings.json";
        private static readonly Lazy<Settings> lazy = new Lazy<Settings>(Initialize);
        private static Settings Initialize()
        {
            if (!File.Exists(SettingsFile))
                File.Create(SettingsFile);

            try
            {
                var settingsText = File.ReadAllText(SettingsFile);
                return (Settings)JsonSerializer.Deserialize(settingsText, typeof(Settings));
            }
            catch
            {
                return new Settings();
            }
        }

        private Settings() { }

        public void Save()
        {
            var settingJson = JsonSerializer.Serialize(this);
            File.WriteAllText(SettingsFile, settingJson);
        }

        public static Settings Instance { get { return lazy.Value; } }

        public bool FollowExtenalLinks { get; set; }
        public bool DownloadExternalContent { get; set; }
        //public bool RespectRobots { get; set; }
        public bool FollowRedirects { get; set; }
        public bool UseCookies { get; set; }
        //public bool IncludeImages { get; set; }
        //public bool IncludeCss { get; set; }
        //public bool IncludeJs { get; set; }
        //public bool IncludeJson { get; set; }
        //public bool IncludeOther { get; set; }
    }
}
