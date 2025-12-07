using System.Windows;
using System.Windows.Media;

namespace Lab_IPO1
{
    public static class ThemeManager
    {
        public static void SetLightTheme()
        {
            Application.Current.Resources["WindowBackground"] = Brushes.White;
        }

        public static void SetDarkTheme()
        {
            Application.Current.Resources["WindowBackground"] = Brushes.Black;
        }
    }
}
