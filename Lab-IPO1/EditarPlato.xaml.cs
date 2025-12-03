using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lab_IPO1
{
    /// <summary>
    /// Lógica de interacción para VentanaPlato.xaml
    /// </summary>
    public partial class EditarPlato : Window
    {
        private List<Plato> _listaPlatos;

        // Constructor para AÑADIR un plato nuevo
        public EditarPlato(List<Plato> listaPlatos)
        {
            InitializeComponent();
            _listaPlatos = listaPlatos; // guardamos la referencia
        }

        // Constructor vacío opcional (si lo necesitas)
        public EditarPlato()
        {
            InitializeComponent();
        }

        // Cargar un plato existente para modificarlo
        public void CargarPlatoParaModificar(Plato p)
        {
            txtNombre.Text = p.Nombre;
            txtPrecio.Text = p.Precio.ToString();
            txtIngredientes.Text = p.Ingredientes;
            txtImagen.Text = p.Imagen?.ToString();
            txtCategoria.Text = p.Categoria;
            txtSubcategoria.Text = p.Subcategoria;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Si estamos modificando, PlatoCreado ya apunta al objeto
                if (PlatoCreado == null) PlatoCreado = new Plato();

                PlatoCreado.Nombre = txtNombre.Text;
                PlatoCreado.Categoria = txtCategoria.Text;
                PlatoCreado.Subcategoria = txtSubcategoria.Text;
                PlatoCreado.Ingredientes = txtIngredientes.Text;
                PlatoCreado.Alergenos = txtAlergenos.Text;
                PlatoCreado.Precio = int.Parse(txtPrecio.Text);
                PlatoCreado.Cantidad = int.Parse(txtCantidad.Text);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el plato: " + ex.Message);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();   // cierra la ventana
        }
        public Plato PlatoCreado { get; set; }
    }
}
