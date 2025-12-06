using System;
using System.Windows;
using System.Windows.Media;

namespace Lab_IPO1
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void BtnPrimaryColor_Click(object sender, RoutedEventArgs e)
        {
            if (TryParseColor(txtPrimaryColor.Text, out Color color))
                ThemeManager.SetPrimaryColor(color);
        }

        private void BtnSecondaryColor_Click(object sender, RoutedEventArgs e)
        {
            if (TryParseColor(txtSecondaryColor.Text, out Color color))
                ThemeManager.SetSecondaryColor(color);
        }

        private void BtnTextColor_Click(object sender, RoutedEventArgs e)
        {
            if (TryParseColor(txtTextColor.Text, out Color color))
                ThemeManager.SetTextColor(color);
        }

        private void BtnFontSize_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(txtFontSize.Text, out double size))
                ThemeManager.SetFontSize(size);
        }

        private void CbFontFamily_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbFontFamily.SelectedItem is System.Windows.Controls.ComboBoxItem item)
                ThemeManager.SetFontFamily(item.Content.ToString());
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.ResetDefaults();

            // Actualizar campos visuales
            txtPrimaryColor.Text = "#FFFF6B35";
            txtSecondaryColor.Text = "#F0F0F0";
            txtTextColor.Text = "Black";
            txtFontSize.Text = "14";
            cbFontFamily.SelectedIndex = 0;
        }

        private bool TryParseColor(string input, out Color color)
        {
            try
            {
                color = (Color)ColorConverter.ConvertFromString(input);
                return true;
            }
            catch
            {
                MessageBox.Show("Color inválido. Usa formato #RRGGBB o nombre de color.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                color = Colors.Transparent;
                return false;
            }
        }
    }
}
