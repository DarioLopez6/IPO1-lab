using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        // SOLO LETRAS Y ESPACIOS
        private void SoloLetras_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsLetter(c) && c != ' ')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        // SOLO NÚMEROS, ESPACIOS Y COMAS
        private void SoloNumerosComas_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c) && c != ',' && c != ' ')
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        // VALIDACIÓN EN TIEMPO REAL
        private void ValidarCampos_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool nombreCorrecto = !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                  txtNombre.Text.All(c => char.IsLetter(c) || c == ' ');

            bool apellidosCorrectos = string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                                      txtApellidos.Text.All(c => char.IsLetter(c) || c == ' ');

            bool telefonoCorrecto = string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                                    txtTelefono.Text.All(c => char.IsDigit(c) || c == ',' || c == ' ');

            lblErrorNombre.Visibility = nombreCorrecto ? Visibility.Collapsed : Visibility.Visible;
            lblErrorApellidos.Visibility = apellidosCorrectos ? Visibility.Collapsed : Visibility.Visible;
            lblErrorTelefono.Visibility = telefonoCorrecto ? Visibility.Collapsed : Visibility.Visible;

            // El botón crear solo se habilita si TODO es válido
            btnCrear.IsEnabled = nombreCorrecto && apellidosCorrectos && telefonoCorrecto;
        }

    }
}
