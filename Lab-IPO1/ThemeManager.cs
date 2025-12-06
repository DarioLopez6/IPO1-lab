using System.Windows;
using System.Windows.Media;

namespace Lab_IPO1
{
    public static class ThemeManager
    {
        private static ResourceDictionary _res = Application.Current.Resources;

        public static void SetPrimaryColor(Color color)
        {
            _res["PrimaryColor"] = color;
            _res["PrimaryBrush"] = new SolidColorBrush(color);
        }

        public static void SetSecondaryColor(Color color)
        {
            _res["SecondaryColor"] = color;
            _res["SecondaryBrush"] = new SolidColorBrush(color);
        }

        public static void SetBackgroundColor(Color color)
        {
            _res["BackgroundColor"] = color;
            _res["BackgroundBrush"] = new SolidColorBrush(color);
        }

        public static void SetTextColor(Color color)
        {
            _res["TextColor"] = color;
            _res["TextBrush"] = new SolidColorBrush(color);
        }

        public static void SetFontSize(double size)
        {
            _res["GlobalFontSize"] = size;
        }

        public static void SetFontFamily(string family)
        {
            _res["GlobalFontFamily"] = new FontFamily(family);
        }

        public static void SetFontWeight(FontWeight weight)
        {
            _res["GlobalFontWeight"] = weight;
        }

        public static void SetFontStyle(FontStyle style)
        {
            _res["GlobalFontStyle"] = style;
        }

        public static void SetTextDecoration(TextDecorationCollection deco)
        {
            _res["GlobalTextDecoration"] = deco;
        }

        public static void ResetDefaults()
        {
            // Aquí puedes resetear los colores y fuentes a valores por defecto
            SetPrimaryColor((Color)ColorConverter.ConvertFromString("#FFFF6B35"));
            SetSecondaryColor((Color)ColorConverter.ConvertFromString("#F0F0F0"));
            SetBackgroundColor(Colors.White);
            SetTextColor(Colors.Black);
            SetFontFamily("Segoe UI");
            SetFontSize(14);
            SetFontWeight(FontWeights.Normal);
            SetFontStyle(FontStyles.Normal);
            SetTextDecoration(new TextDecorationCollection());
        }
    }
}
