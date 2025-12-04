using System;
using System.Windows;
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
            TxtCategoria.Text = platoOriginal.Categoria;
            TxtSubcategoria.Text = platoOriginal.Subcategoria;
            TxtIngredientes.Text = platoOriginal.Ingredientes;
            TxtAlergenos.Text = platoOriginal.Alergenos;
            TxtPrecio.Text = platoOriginal.Precio.ToString("0.00");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) ||
                string.IsNullOrWhiteSpace(TxtPrecio.Text) ||
                !double.TryParse(TxtPrecio.Text, out double precio))
            {
                MessageBox.Show("Nombre y precio válidos son obligatorios.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Guardar cambios en el objeto Plato
            platoOriginal.Nombre = TxtNombre.Text;
            platoOriginal.Categoria = TxtCategoria.Text;
            platoOriginal.Subcategoria = TxtSubcategoria.Text;
            platoOriginal.Ingredientes = TxtIngredientes.Text;
            platoOriginal.Alergenos = TxtAlergenos.Text;
            platoOriginal.Precio = precio;

            // Cerrar ventana indicando éxito
            this.DialogResult = true;
            this.Close();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var resultado = MessageBox.Show($"¿Está seguro de que desea eliminar el plato '{platoOriginal.Nombre}'?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (resultado == MessageBoxResult.Yes)
            {
                // Marcar objeto como eliminado mediante un flag opcional o lo manejas en la lista principal
                PlatoEliminado = platoOriginal;
                this.DialogResult = true;
                this.Close();
            }
        }

        // Propiedad pública para saber si se eliminó
        public Plato PlatoEliminado { get; private set; }

        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Editar/Eliminar producto", "Observa la informacion del producto(puedes editarla), tamién puedes eliminarlo") { Owner = this };
            help.ShowDialog();
        }
    }
}
