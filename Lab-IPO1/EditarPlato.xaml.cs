using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace Lab_IPO1
{
    public partial class EditarPlato : Window
    {
        private Plato platoOriginal;

        public EditarPlato(Plato plato)
        {
            InitializeComponent();
            platoOriginal = plato;
            CargarDatosPlato();

            // Limitar TxtPrecio a números y punto decimal
            TxtPrecio.PreviewTextInput += TxtPrecio_PreviewTextInput;
            DataObject.AddPastingHandler(TxtPrecio, OnPaste); // Evitar pegar texto no numérico
        }

        private void CargarDatosPlato()
        {
            if (platoOriginal == null) return;

            // Imagen
            if (platoOriginal.Imagen != null)
                ImgPlato.Source = new BitmapImage(platoOriginal.Imagen);
            else
                ImgPlato.Source = new BitmapImage(new Uri("/imagenes/logo.png", UriKind.Relative));

            // Información
            TxtNombre.Text = platoOriginal.Nombre;
            TxtIngredientes.Text = platoOriginal.Ingredientes;
            TxtAlergenos.Text = platoOriginal.Alergenos;
            TxtPrecio.Text = platoOriginal.Precio.ToString("0.00");

            // Categoría
            CmbCategoria.SelectedItem = CmbCategoria.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == platoOriginal.Categoria);

            // Subcategoría
            CmbSubcategoria.SelectedItem = CmbSubcategoria.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(i => i.Content.ToString() == platoOriginal.Subcategoria);
        }


        // Validar y guardar
        private void BtnEditar_Click(object sender, RoutedEventArgs e)
            {
                bool tieneErrores = false;

                // Limpiar mensajes
                ErrNombre.Visibility = Visibility.Collapsed;
                ErrCategoria.Visibility = Visibility.Collapsed;
                ErrSubcategoria.Visibility = Visibility.Collapsed;
                ErrPrecio.Visibility = Visibility.Collapsed;

                // Nombre obligatorio
                if (string.IsNullOrWhiteSpace(TxtNombre.Text))
                {
                    ErrNombre.Visibility = Visibility.Visible;
                    tieneErrores = true;
                }

                // Categoría obligatorio
                if (CmbCategoria.SelectedItem == null)
                {
                    ErrCategoria.Visibility = Visibility.Visible;
                    tieneErrores = true;
                }

                // Subcategoría obligatorio
                if (CmbSubcategoria.SelectedItem == null)
                {
                    ErrSubcategoria.Visibility = Visibility.Visible;
                    tieneErrores = true;
                }

                // Precio obligatorio y válido
                string precioTexto = TxtPrecio.Text; // Reemplazar punto por coma
                if (!double.TryParse(precioTexto, NumberStyles.AllowDecimalPoint, new CultureInfo("es-ES"), out double precio) || precio < 0)
                {
                    ErrPrecio.Visibility = Visibility.Visible;
                    tieneErrores = true;
                }

                if (tieneErrores)
                    return;

                // Guardar cambios
                platoOriginal.Nombre = TxtNombre.Text;
                platoOriginal.Categoria = (CmbCategoria.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Otro";
                platoOriginal.Subcategoria = (CmbSubcategoria.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Otro";
                platoOriginal.Ingredientes = TxtIngredientes.Text;
                platoOriginal.Alergenos = TxtAlergenos.Text;
                platoOriginal.Precio = Math.Round(precio, 2); // Redondear a 2 decimales

                this.DialogResult = true;
                this.Close();
            }


    private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show(
                $"¿Está seguro de que desea eliminar el plato '{platoOriginal.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                PlatoEliminado = platoOriginal;
                this.DialogResult = true;
                this.Close();
            }
        }

        // Propiedad pública para saber si se eliminó
        public Plato PlatoEliminado { get; private set; }

        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow(
                "• Editar/Eliminar producto",
                "Observa la información del producto (puedes editarla), también puedes eliminarlo")
            {
                Owner = this
            };
            help.ShowDialog();
        }

        // Solo permitir números y un punto decimal en TxtPrecio
        // Solo permitir números con hasta 2 decimales en TxtPrecio
        private void TxtPrecio_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string textoActual = TxtPrecio.Text;
            int selectionStart = TxtPrecio.SelectionStart;
            int selectionLength = TxtPrecio.SelectionLength;

            // Simular el texto resultante después de la inserción
            string nuevoTexto = textoActual.Remove(selectionStart, selectionLength);
            nuevoTexto = nuevoTexto.Insert(selectionStart, e.Text);

            // Validar formato: número positivo con hasta 2 decimales
            Regex regex = new Regex(@"^\d*\,?\d{0,2}$");
            e.Handled = !regex.IsMatch(nuevoTexto);
        }

        // Evitar pegar texto que no sea número con hasta 2 decimales
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!Regex.IsMatch(text, @"^\d*\,?\d{0,2}$"))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


    
    }
}
