using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Lab_IPO1
{
    public partial class CrearCliente : Window
    {
        public Cliente NuevoCliente { get; private set; }

        public CrearCliente()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            lblErrorNombre.Visibility = Visibility.Collapsed;

            // Validación: nombre obligatorio
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                lblErrorNombre.Visibility = Visibility.Visible;
                return;
            }

            // Convertir listas
            List<string> telefonos = txtTelefono.Text.Split(',')
                .Select(s => s.Trim()).Where(s => s != "").ToList();

            List<string> correos = txtCorreo.Text.Split(',')
                .Select(s => s.Trim()).Where(s => s != "").ToList();

            List<string> direcciones = new List<string>();
            if (!string.IsNullOrWhiteSpace(txtDireccionPrincipal.Text))
                direcciones.Add(txtDireccionPrincipal.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtDireccionSecundaria.Text))
                direcciones.Add(txtDireccionSecundaria.Text.Trim());

            List<string> alergias = txtAlergias.Text.Split(',')
                .Select(s => s.Trim()).Where(s => s != "").ToList();

            List<string> intolerancias = txtIntolerancias.Text.Split(',')
                .Select(s => s.Trim()).Where(s => s != "").ToList();

            // Forma de pago principal
            FORMAPAGO formaPagoPrincipal;
            if (rbTarjeta.IsChecked == true) formaPagoPrincipal = FORMAPAGO.TARGETA;
            else if (rbEfectivo.IsChecked == true) formaPagoPrincipal = FORMAPAGO.EFECTIVO;
            else formaPagoPrincipal = FORMAPAGO.BIZUM; // añade Bizum al enum si no existe

            // Forma de pago alternativa
            FORMAPAGO formaPagoAlternativa;
            if (rbTarjeta1.IsChecked == true) formaPagoAlternativa = FORMAPAGO.TARGETA;
            else if (rbEfectivo1.IsChecked == true) formaPagoAlternativa = FORMAPAGO.EFECTIVO;
            else formaPagoAlternativa = FORMAPAGO.BIZUM;

            // Generar ID
            int nuevoID = new Random().Next(100000, 999999);

            // Crear cliente (puedes guardar la alternativa en otra propiedad si quieres)
            NuevoCliente = new Cliente(
                nuevoID,
                txtNombre.Text.Trim(),
                txtApellidos.Text.Trim(),
                direcciones,
                telefonos,
                correos,
                alergias,
                intolerancias,
                formaPagoPrincipal,
                0,
                0
            );

            this.DialogResult = true;
            Close();

        }



        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Creación de cliente:", "Introduce los datos del nuevo cliente(como minimno el nombre)");
            help.Owner = this;
            help.ShowDialog();
        }
    }
}
