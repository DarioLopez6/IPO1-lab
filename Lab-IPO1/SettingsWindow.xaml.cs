using System.Windows;

namespace Lab_IPO1
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void BtnLight_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.SetLightTheme();
        }

        private void BtnDark_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.SetDarkTheme();
        }

    }
}
