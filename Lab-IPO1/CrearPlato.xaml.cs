using Lab_IPO1;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lab_IPO1
{
    public partial class CrearPlato : Window
    {
        public Plato PlatoCreado { get; private set; }
        private string rutaImagen = null;

        public CrearPlato()
        {
            InitializeComponent();
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            ComboCategoria.ItemsSource = new string[]
            {
                "Entrante","Primero","Segundo","Postre","Bebida","Otro"
            };

            ComboSubcategoria.ItemsSource = new string[]
            {
                "Ensalada","Carne","Arroces y Pastas","Pescado","Otro"
            };
        }

        // ===================== ELEGIR IMAGEN =======================
        private void BtnElegirImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Imágenes|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                rutaImagen = dlg.FileName;
                ImgPlato.Source = new BitmapImage(new Uri(rutaImagen));
                ErrorImagen.Visibility = Visibility.Collapsed; // Ocultar error si ya seleccionó
            }
        }

        // ===================== CREAR PLATO =======================
        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            bool hayError = false;

            // Ocultar todos los errores al iniciar
            ErrorNombre.Visibility = Visibility.Collapsed;
            ErrorPrecio.Visibility = Visibility.Collapsed;
            ErrorCategoria.Visibility = Visibility.Collapsed;
            ErrorSubcategoria.Visibility = Visibility.Collapsed;
            ErrorImagen.Visibility = Visibility.Collapsed;

            // Validar Nombre
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                ErrorNombre.Visibility = Visibility.Visible;
                hayError = true;
            }

            // Validar Precio
            if (!double.TryParse(TxtPrecio.Text, out double precio) || precio < 0)
            {
                ErrorPrecio.Visibility = Visibility.Visible;
                hayError = true;
            }

            // Validar Categoría
            if (ComboCategoria.SelectedItem == null)
            {
                ErrorCategoria.Visibility = Visibility.Visible;
                hayError = true;
            }

            // Validar Subcategoría
            if (ComboSubcategoria.SelectedItem == null)
            {
                ErrorSubcategoria.Visibility = Visibility.Visible;
                hayError = true;
            }

            // Validar Imagen
            if (rutaImagen == null)
            {
                ErrorImagen.Visibility = Visibility.Visible;
                hayError = true;
            }

            if (hayError) return; // Detener si hay errores

            // Crear el Plato
            PlatoCreado = new Plato(
                ComboCategoria.SelectedItem.ToString(),     // Categoria
                ComboSubcategoria.SelectedItem.ToString(),  // Subcategoria
                TxtNombre.Text,                             // Nombre
                TxtIngredientes.Text,                       // Ingredientes
                precio,                                     // Precio
                TxtAlergenos.Text,                           // Alergenos
                new Uri(rutaImagen, UriKind.Absolute),       // Imagen
                1                                            // Cantidad
            );


            DialogResult = true;
            Close();
        }

        // ===================== CANCELAR =======================
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ===================== AYUDA =======================
        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Crear plato", "Añade foto y datos del producto para crearlo") { Owner = this };
            help.ShowDialog();
        }
    }
}
