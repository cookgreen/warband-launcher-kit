using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Launcher
{
    public partial class MainWindow
    {
        private readonly bool WSE = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            var loader = WSE ? new WseLoader() : (ILoader)new WarbandLoader();
            loader.LoadGame();
            Close();
        }

        private void MainWindowOnMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void ConfigureButtonClick(object sender, RoutedEventArgs e)
        {
            string warbandRglTxtFullPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents", "Mount&Blade Warband", "rgl_config.txt");
            if (File.Exists(warbandRglTxtFullPath))
            {
                new ConfigurationWindow().Show();
            }
            else
            {
                MessageBox.Show("Could not find config file!");
            }
        }
    }
}
