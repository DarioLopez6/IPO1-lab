using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab_IPO1
{
    public partial class LoginWindow : Window
    {
        private string usuario = "admin";
        private string password = "ipo1";

        private BitmapImage tick = new BitmapImage(new Uri("/imagenes/tick.png", UriKind.Relative));
        private BitmapImage cross = new BitmapImage(new Uri("/imagenes/cross.png", UriKind.Relative));
        private BitmapImage iconUser = new BitmapImage(new Uri("/imagenes/usuarioIcono.png", UriKind.Relative));
        private BitmapImage iconPass = new BitmapImage(new Uri("/imagenes/contrasenaIcono.png", UriKind.Relative));
        private BitmapImage eyeOpen = new BitmapImage(new Uri("/imagenes/ojoOn.png", UriKind.Relative));
        private BitmapImage eyeClosed = new BitmapImage(new Uri("/imagenes/ojoOff.png", UriKind.Relative));

        private bool passwordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();

            // Eventos de sincronización
            PwdBox.PasswordChanged += PwdBox_PasswordChanged;
            TxtPasswordVisible.TextChanged += TxtPasswordVisible_TextChanged;
        }

        // Usuario: enter valida y enfoca password
        private void TxtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidarUsuario();
                if (PwdBox.IsEnabled)
                    PwdBox.Focus();
            }
        }

        // PasswordBox: enter valida y enfoca BtnAcceder
        private void PwdBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidarContrasena();
                if ((passwordVisible ? TxtPasswordVisible.Text : PwdBox.Password) == password)
                    BtnAcceder.Focus();
            }
        }

        // TextBox visible: sincroniza y valida
        private void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passwordVisible)
                ValidarContrasena();
        }

        // PasswordBox: valida al cambiar
        private void PwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!passwordVisible)
                ValidarContrasena();
        }

        // Validación usuario
        private void ValidarUsuario()
        {
            if (string.IsNullOrEmpty(TxtUsuario.Text))
            {
                BorderUsuario.BorderBrush = Brushes.Gray;
                ImgUsuarioIcon.Source = iconUser;
                PwdBox.IsEnabled = false;
            }
            else if (TxtUsuario.Text == usuario)
            {
                BorderUsuario.BorderBrush = Brushes.Green;
                BorderUsuario.Background = Brushes.LightGreen;
                ImgUsuarioIcon.Source = tick;
                PwdBox.IsEnabled = true;
            }
            else
            {
                BorderUsuario.BorderBrush = Brushes.Red;
                BorderUsuario.Background = Brushes.LightCoral;
                ImgUsuarioIcon.Source = cross;
                PwdBox.IsEnabled = false;
            }
        }

        // Validación contraseña
        private void ValidarContrasena()
        {
            string pwd = passwordVisible ? TxtPasswordVisible.Text : PwdBox.Password;

            if (string.IsNullOrEmpty(pwd))
            {
                BorderContrasena.BorderBrush = Brushes.Gray;
                imgContrasena.Source = iconPass;
                BorderContrasena.Background = Brushes.White;
            }
            else if (pwd == password)
            {
                BorderContrasena.BorderBrush = Brushes.Green;
                BorderContrasena.Background = Brushes.LightGreen;
                imgContrasena.Source = tick;
            }
            else
            {
                BorderContrasena.BorderBrush = Brushes.Red;
                BorderContrasena.Background = Brushes.LightCoral;
                imgContrasena.Source = cross;
            }
        }

        // Mostrar/ocultar contraseña
        private void BtnShowPassword_Click(object sender, RoutedEventArgs e)
        {
            passwordVisible = !passwordVisible;

            if (passwordVisible)
            {
                TxtPasswordVisible.Text = PwdBox.Password;
                TxtPasswordVisible.Visibility = Visibility.Visible;
                PwdBox.Visibility = Visibility.Collapsed;
                ojo.Source = eyeOpen;

                TxtPasswordVisible.Focus();
                TxtPasswordVisible.CaretIndex = TxtPasswordVisible.Text.Length;
            }
            else
            {
                PwdBox.Password = TxtPasswordVisible.Text;
                TxtPasswordVisible.Visibility = Visibility.Collapsed;
                PwdBox.Visibility = Visibility.Visible;
                ojo.Source = eyeClosed;

                PwdBox.Focus();
                PwdBox.SelectAll();
            }
        }

        // Botón Acceder
        private void BtnAcceder_Click(object sender, RoutedEventArgs e)
        {
            ValidarUsuario();
            ValidarContrasena();

            string pwd = passwordVisible ? TxtPasswordVisible.Text : PwdBox.Password;

            if (BorderUsuario.BorderBrush == Brushes.Green && pwd == password)
            {
                MainWindow main = new MainWindow();
                main.Show();
                this.Close();
            }
        }

        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow();
            help.Owner = this;
            help.ShowDialog();
        }
        private void BtnRecuperar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("La contraseña para el usuario 'admin' es 'ipo1'.", "Recuperar Contraseña", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Btnautoria_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aplicación desarrollada por Rubén, Víctor y Darío.", "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
