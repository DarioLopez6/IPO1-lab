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
            /*PwdBox.PasswordChanged += PwdBox_PasswordChanged;
            TxtPasswordVisible.TextChanged += TxtPasswordVisible_TextChanged;*/
        }

        // Usuario: Enter valida y enfoca password
        private void TxtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidarUsuario();
                if (PwdBox.IsEnabled)
                    PwdBox.Focus();
            }
        }

        // Contraseña: Enter ejecuta Acceder
        private void TxtContrasena_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnAcceder.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
        private void TxtPasswordVisible_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidarContrasena();

                if (TxtPasswordVisible.Text == password)
                    BtnAcceder.Focus();
            }
        }


        private void PwdBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ValidarContrasena();

                string pwd = passwordVisible ? TxtPasswordVisible.Text : PwdBox.Password;

                if (pwd == password)
                    BtnAcceder.Focus();
            }
        }


        private void TxtPasswordVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (passwordVisible)
                ValidarContrasena();
        }

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
                SetPasswordBoxEnabled(false);
                if(txterrorUsuario.Visibility == Visibility.Visible)
                {
                    BorderUsuario.BorderBrush = Brushes.Red;
                    BorderUsuario.Background = Brushes.LightCoral;
                    ImgUsuarioIcon.Source = cross;
                    SetPasswordBoxEnabled(false);
                }
                return;
            }

            if (TxtUsuario.Text == usuario)
            {
                BorderUsuario.BorderBrush = Brushes.Green;
                BorderUsuario.Background = Brushes.LightGreen;
                ImgUsuarioIcon.Source = tick;
                SetPasswordBoxEnabled(true);
                return;
            }

            // Usuario incorrecto
            BorderUsuario.BorderBrush = Brushes.Red;
            BorderUsuario.Background = Brushes.LightCoral;
            ImgUsuarioIcon.Source = cross;
            SetPasswordBoxEnabled(false);
        }

        // Helper para habilitar/deshabilitar PwdBox y TxtPasswordVisible
        private void SetPasswordBoxEnabled(bool enabled)
        {
            PwdBox.IsEnabled = enabled;
            TxtPasswordVisible.IsEnabled = enabled;

            if (enabled)
            {
                BorderContrasena.Background = Brushes.White;
                PwdBox.Foreground = Brushes.Black;
                TxtPasswordVisible.Foreground = Brushes.Black;
                PwdBox.Cursor = Cursors.IBeam;
                TxtPasswordVisible.Cursor = Cursors.IBeam;
            }
            else
            {
                BorderContrasena.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                PwdBox.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                TxtPasswordVisible.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                PwdBox.Cursor = Cursors.No;
                TxtPasswordVisible.Cursor = Cursors.No;
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
            else
            {
                // Usuario correcto → error solo en contraseña
                if (BorderUsuario.BorderBrush == Brushes.Green)
                {
                    txterrorContrasena.Visibility = Visibility.Visible;
                    txterrorUsuario.Visibility = Visibility.Hidden;

                    // ❗ Cambiar a error visual
                    BorderContrasena.BorderBrush = Brushes.Red;
                    BorderContrasena.Background = Brushes.LightCoral;
                    imgContrasena.Source = cross;
                }
                else
                {
                    // Usuario incorrecto → error usuario
                    txterrorUsuario.Visibility = Visibility.Visible;
                    txterrorContrasena.Visibility = Visibility.Hidden;

                    BorderContrasena.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    PwdBox.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                    TxtPasswordVisible.Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                    PwdBox.Cursor = Cursors.No;
                    TxtPasswordVisible.Cursor = Cursors.No;
                }
            }
            if (txterrorUsuario.Visibility == Visibility.Visible)
            {
                BorderUsuario.BorderBrush = Brushes.Red;
                BorderUsuario.Background = Brushes.LightCoral;
                ImgUsuarioIcon.Source = cross;
                SetPasswordBoxEnabled(false);
            }
        }


        private void BtnAyuda_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow("• Inicio de Sesión:", "Introduce tu usuario y contraseña proporcionados por el administrador del TPV.");
            help.Owner = this;
            help.ShowDialog();
        }

        private void BtnRecuperar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("La contraseña para el usuario 'admin' es 'ipo1'.", "Recuperar Contraseña", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Btnautoria_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aplicación desarrollada por Rubén, Víctor y Darío. \nFecha: 1/12/2025.\nPrimer prototipo(V0.6).", "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
        }




        private void BtnPRUEBA_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Owner = this;
            settings.ShowDialog();
        }








    }

}
