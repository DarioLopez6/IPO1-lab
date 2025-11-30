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
            // Validación básica
            lblErrorNombre.Visibility = string.IsNullOrWhiteSpace(txtNombre.Text) ? Visibility.Visible : Visibility.Collapsed;
            if (lblErrorNombre.Visibility == Visibility.Visible)
                return;

            // Actualizar los datos del cliente
            ClienteEditado.Nombre = txtNombre.Text.Trim();
            ClienteEditado.Apellidos = txtApellidos.Text.Trim();
            ClienteEditado.Telefono = txtTelefono.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();
            ClienteEditado.eMail = txtCorreo.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();

            ClienteEditado.Direccion = new List<string>();
            if (!string.IsNullOrWhiteSpace(txtDireccionPrincipal.Text)) ClienteEditado.Direccion.Add(txtDireccionPrincipal.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtDireccionSecundaria.Text)) ClienteEditado.Direccion.Add(txtDireccionSecundaria.Text.Trim());

            ClienteEditado.Alergias = txtAlergias.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();
            ClienteEditado.Intolerancias = txtIntolerancias.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();

            // Actualizar forma de pago principal
            if (rbTarjeta.IsChecked == true) ClienteEditado.pago = FORMAPAGO.TARGETA;
            else if (rbEfectivo.IsChecked == true) ClienteEditado.pago = FORMAPAGO.EFECTIVO;
            else ClienteEditado.pago = FORMAPAGO.BIZUM;

            this.DialogResult = true;
            Close();
        }

        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Edición de cliente:", "Modifica los datos del cliente según sea necesario. Al menos el nombre es obligatorio.");
            help.Owner = this;
            help.ShowDialog();
        }


    }

}
