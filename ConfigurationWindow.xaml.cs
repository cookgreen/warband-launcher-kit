using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Launcher
{
    public partial class ConfigurationWindow
    {
        private Dictionary<string, string> _config;

        public void LoadRglConfig()
        {
            _config = new Dictionary<string, string>();
            var pairs = File.ReadAllLines(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mount&Blade Warband", "rgl_config.txt")).Select(t => t.Trim()).Where(t => t != "").Select(
                u =>
                {
                    var d = u.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                    return new KeyValuePair<string, string>(d[0].Trim(), d[1].Trim());
                });
            foreach (var p in pairs) _config[p.Key] = p.Value;
        }

        public void SaveRglConfig()
        {
            var f = new StreamWriter(Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mount&Blade Warband", "rgl_config.txt"));
            foreach (var p in _config)
            {
                f.WriteLine("{0} = {1}", p.Key, p.Value);
                f.WriteLine();
            }
            f.Close();
        }

        public void BindCheckBoxWithConfigDict(CheckBox checkBox, string configKey)
        {
            checkBox.Tag = configKey;
            checkBox.IsChecked = _config[configKey] == "1";
            checkBox.Checked += (s, e) => SyncCheckBoxWithConfigDict((CheckBox)s);
            checkBox.Unchecked += (s, e) => SyncCheckBoxWithConfigDict((CheckBox)s);
        }

        public void SyncCheckBoxWithConfigDict(CheckBox checkBox)
        {
            var t = checkBox.IsChecked ?? false;
            _config[(string)checkBox.Tag] = t ? "1" : "0";
        }

        public void BindCheckBoxWithConfigDictNot(CheckBox checkBox, string configKey)
        {
            checkBox.Tag = configKey;
            checkBox.IsChecked = _config[configKey] == "0";
            checkBox.Checked += (s, e) => SyncCheckBoxWithConfigDictNot((CheckBox)s);
            checkBox.Unchecked += (s, e) => SyncCheckBoxWithConfigDictNot((CheckBox)s);
        }

        public void SyncCheckBoxWithConfigDictNot(CheckBox checkBox)
        {
            var t = checkBox.IsChecked ?? true;
            _config[(string)checkBox.Tag] = t ? "0" : "1";
        }

        public ConfigurationWindow()
        {
            InitializeComponent();
            LoadRglConfig();
            MouseMove += (s, e) => { if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove(); };
            BindCheckBoxWithConfigDictNot(HideBlood, "enable_blood");
            BindCheckBoxWithConfigDict(EnableCheats, "cheat_mode");
            BindCheckBoxWithConfigDict(EnableSoundVariation, "cheat_mode");
            BindCheckBoxWithConfigDict(DisableMusic, "disable_music");
            BindCheckBoxWithConfigDict(DisableSound, "disable_sound");
            BindCheckBoxWithConfigDict(EditMode, "enable_edit_mode");
            BindCheckBoxWithConfigDict(SingleThread, "force_single_threading");
            BindCheckBoxWithConfigDict(PixelShaders, "use_pixel_shaders");
            BindCheckBoxWithConfigDict(Windowed, "start_windowed");
            BindCheckBoxWithConfigDict(ShowFramerate, "show_framerate");
            BindCheckBoxWithConfigDict(ForceVerticalSync, "force_vsync");
            BindCheckBoxWithConfigDict(LoadTexturesOnDemand, "use_ondemand_textures_");
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            SaveRglConfig();
            Close();
        }
    }
}
