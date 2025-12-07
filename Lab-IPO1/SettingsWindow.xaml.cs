using System.Runtime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lab_IPO1
{
    public partial class SettingsWindow : Window
    {
        // Variables para almacenar temporalmente las selecciones
        public string selectedFont;
        public double selectedFontSize;
        public string selectedTheme;

        public SettingsWindow(FontFamily currentFont, double currentFontSize)
        {
            InitializeComponent();

            // Asignar valores actuales a las variables
            selectedFont = currentFont.Source;
            selectedFontSize = currentFontSize;

            // Inicializar ComboBox de fuentes
            foreach (ComboBoxItem item in FontComboBox.Items)
            {
                if (item.Content.ToString() == selectedFont)
                {
                    FontComboBox.SelectedItem = item;
                    break;
                }
            }

            // Inicializar ComboBox de tamaños
            foreach (ComboBoxItem item in FontSizeComboBox.Items)
            {
                if (double.TryParse(item.Content.ToString(), out double size) && size == selectedFontSize)
                {
                    FontSizeComboBox.SelectedItem = item;
                    break;
                }
            }

            // Eventos para actualizar variables cuando el usuario cambia selección
            FontComboBox.SelectionChanged += (s, e) =>
            {
                selectedFont = ((ComboBoxItem)FontComboBox.SelectedItem).Content.ToString();
            };

            FontSizeComboBox.SelectionChanged += (s, e) =>
            {
                selectedFontSize = double.Parse(((ComboBoxItem)FontSizeComboBox.SelectedItem).Content.ToString());
            };
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Aplicar cambios a la ventana principal
            if (Application.Current.MainWindow != null)
            {

                foreach (Window w in Application.Current.Windows)
                {
                    // Fuente
                    w.FontFamily = new FontFamily(selectedFont);

                    // Tamaño de fuente
                    w.FontSize = selectedFontSize;

                    // Tema
                    w.Background = selectedTheme == "Oscuro" ? Brushes.DarkSlateGray : Brushes.White;
                }

            }
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
