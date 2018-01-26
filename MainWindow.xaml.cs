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
            new ConfigurationWindow().Show();
        }
    }
}
