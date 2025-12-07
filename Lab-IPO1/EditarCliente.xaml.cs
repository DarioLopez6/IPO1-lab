using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Lab_IPO1
{
    public partial class EditarCliente : Window
    {
        private string rutaFotoCliente; // Nueva propiedad para la foto
        public Cliente ClienteEditado { get; private set; }

        public EditarCliente(Cliente cliente, FontFamily fontName, double fontSize)
        {
            InitializeComponent();
            this.FontFamily = fontName;
            this.FontSize = fontSize;

            if (cliente == null) throw new ArgumentNullException(nameof(cliente));

            ClienteEditado = cliente;
            rutaFotoCliente = cliente.Imagen; // Cargar ruta actual del cliente

            // Cargar datos en los controles
            txtNombre.Text = cliente.Nombre;
            txtApellidos.Text = cliente.Apellidos;
            txtTelefono.Text = string.Join(", ", cliente.Telefono);
            txtCorreo.Text = string.Join(", ", cliente.eMail);
            txtDireccionPrincipal.Text = cliente.Direccion.Count > 0 ? cliente.Direccion[0] : "";
            txtDireccionSecundaria.Text = cliente.Direccion.Count > 1 ? cliente.Direccion[1] : "";
            txtAlergias.Text = string.Join(", ", cliente.Alergias);
            txtIntolerancias.Text = string.Join(", ", cliente.Intolerancias);
            txtPuntosActuales.Text = cliente.puntosAcumulados.ToString();


            // Cargar imagen
            if (!string.IsNullOrEmpty(rutaFotoCliente))
            {
                imgFotoCliente.Source = new BitmapImage(new Uri(rutaFotoCliente, UriKind.RelativeOrAbsolute));
            }

            // Forma de pago principal
            switch (cliente.pago)
            {
                case FORMAPAGO.TARGETA: rbTarjeta.IsChecked = true; break;
                case FORMAPAGO.EFECTIVO: rbEfectivo.IsChecked = true; break;
                case FORMAPAGO.BIZUM: rbBizum.IsChecked = true; break;
            }
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

            // Guardar cambios
            ClienteEditado.Nombre = txtNombre.Text.Trim();
            ClienteEditado.Apellidos = txtApellidos.Text.Trim();
            ClienteEditado.Telefono = txtTelefono.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();
            ClienteEditado.eMail = txtCorreo.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();
            ClienteEditado.Direccion = new System.Collections.Generic.List<string>();
            if (!string.IsNullOrWhiteSpace(txtDireccionPrincipal.Text)) ClienteEditado.Direccion.Add(txtDireccionPrincipal.Text.Trim());
            if (!string.IsNullOrWhiteSpace(txtDireccionSecundaria.Text)) ClienteEditado.Direccion.Add(txtDireccionSecundaria.Text.Trim());
            ClienteEditado.Alergias = txtAlergias.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();
            ClienteEditado.Intolerancias = txtIntolerancias.Text.Split(',').Select(s => s.Trim()).Where(s => s != "").ToList();

            if (int.TryParse(txtPuntosActuales.Text, out int puntos))
                ClienteEditado.puntosAcumulados = puntos;


            // Forma de pago
            if (rbTarjeta.IsChecked == true) ClienteEditado.pago = FORMAPAGO.TARGETA;
            else if (rbEfectivo.IsChecked == true) ClienteEditado.pago = FORMAPAGO.EFECTIVO;
            else ClienteEditado.pago = FORMAPAGO.BIZUM;

            // Foto
            ClienteEditado.Imagen = rutaFotoCliente;

            this.DialogResult = true;
            Close();
        }

        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Edición de cliente:", "Modifica los datos del cliente según sea necesario. Al menos el nombre es obligatorio.", this.FontFamily, this.FontSize);
            help.Owner = this;
            help.ShowDialog();
        }

        private void BtnSeleccionarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Archivos de imagen (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";
            dlg.Title = "Seleccionar foto del cliente";

            bool? resultado = dlg.ShowDialog();
            if (resultado == true)
            {
                rutaFotoCliente = dlg.FileName;
                imgFotoCliente.Source = new BitmapImage(new Uri(rutaFotoCliente));
            }
        }
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

    }
}
