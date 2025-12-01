using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Lab_IPO1
{
    public partial class EditarCliente : Window
    {
        public Cliente ClienteEditado { get; private set; }

    public EditarCliente(Cliente cliente)
        {
            InitializeComponent();

            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            ClienteEditado = cliente;

            // Cargar datos en los controles
            txtNombre.Text = cliente.Nombre;
            txtApellidos.Text = cliente.Apellidos;
            txtTelefono.Text = string.Join(", ", cliente.Telefono);
            txtCorreo.Text = string.Join(", ", cliente.eMail);
            txtDireccionPrincipal.Text = cliente.Direccion.Count > 0 ? cliente.Direccion[0] : "";
            txtDireccionSecundaria.Text = cliente.Direccion.Count > 1 ? cliente.Direccion[1] : "";
            txtAlergias.Text = string.Join(", ", cliente.Alergias);
            txtIntolerancias.Text = string.Join(", ", cliente.Intolerancias);

            switch (cliente.pago)
            {
                case FORMAPAGO.TARGETA: rbTarjeta.IsChecked = true; break;
                case FORMAPAGO.EFECTIVO: rbEfectivo.IsChecked = true; break;
                case FORMAPAGO.BIZUM: rbBizum.IsChecked = true; break;
            }

            // Forma de pago alternativa se deja sin seleccionar
            rbTarjeta1.IsChecked = false;
            rbEfectivo1.IsChecked = false;
            rbBizum1.IsChecked = false;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            bool hayError = false;

            // Nombre - solo letras
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || !txtNombre.Text.All(c => char.IsLetter(c) || c == ' '))
            {
                lblErrorNombre.Visibility = Visibility.Visible;
                hayError = true;
            }
            else lblErrorNombre.Visibility = Visibility.Collapsed;

            // Apellidos - solo letras
            if (!string.IsNullOrWhiteSpace(txtApellidos.Text) &&
                !txtApellidos.Text.All(c => char.IsLetter(c) || c == ' '))
            {
                lblErrorApellidos.Visibility = Visibility.Visible;
                hayError = true;
            }
            else lblErrorApellidos.Visibility = Visibility.Collapsed;

            // Teléfono - solo números y comas
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text) &&
                !txtTelefono.Text.All(c => char.IsDigit(c) || c == ',' || c == ' '))
            {
                lblErrorTelefono.Visibility = Visibility.Visible;
                hayError = true;
            }
            else lblErrorTelefono.Visibility = Visibility.Collapsed;

            if (hayError) return;
        }

        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Edición de cliente:", "Modifica los datos del cliente según sea necesario. Al menos el nombre es obligatorio.");
            help.Owner = this;
            help.ShowDialog();
        }


    }

}
