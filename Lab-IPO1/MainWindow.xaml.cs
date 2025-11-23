using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab_IPO1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    List<Pedido> pendientesDePago = new List<Pedido>();
    List<Pedido> enElaboracion = new List<Pedido>();
    List<Pedido> listosParaEntregar = new List<Pedido>();
    List<Pedido> historial = new List<Pedido>();

    public MainWindow()
    {
        InitializeComponent();
    }
    public void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded) return; // evita errores al inicializar

        // Productos
        ImgProductos.Source = new BitmapImage(new Uri(
            TabProductos.IsSelected ? "/imagenes/menuProductosOn.png" : "/imagenes/menuProductosOff.png",
            UriKind.Relative));

        // Pedidos
        ImgPedidos.Source = new BitmapImage(new Uri(
            TabPedidos.IsSelected ? "/imagenes/menuPedidosOn.png" : "/imagenes/menuPedidosOff.png",
            UriKind.Relative));

        // Clientes
        ImgClientes.Source = new BitmapImage(new Uri(
            TabClientes.IsSelected ? "/imagenes/menuClientesOn.png" : "/imagenes/menuClientesOff.png",
            UriKind.Relative));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        LoginWindow main = new LoginWindow();
        main.Show();
        this.Close();
    }
    private void BtnAyuda_Click(object sender, RoutedEventArgs e)
    {
        HelpWindow help = new HelpWindow();
        help.Owner = this;
        help.ShowDialog();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        HelpWindow help = new HelpWindow();
        help.Owner = this;
        help.ShowDialog();
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        btnenLocal.IsChecked = false;
        btntelfono.IsChecked = false;
        btnbizum.IsChecked = false;
        btnefectivo.IsChecked = false;
        btntarjeta.IsChecked = false;
        txtcliente.Text = "";
        txtdomicilio.Text = "";
        txthora.Text = "";
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        Button_Click_2(sender, e);
    }
    private void Btnautoria_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Aplicación desarrollada por Rubén, Víctor y Darío.", "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Button_Click_4(object sender, RoutedEventArgs e)
    {
        bool local = false;
        if (btnenLocal.IsChecked == true) local = true;
        string hora = txthora.Text;
        string domicilio = txtdomicilio.Text;
        string cliente = txtcliente.Text;
        List<Producto> productos = new List<Producto>();
        double total = double.Parse(txttotal.Text.Substring(0, txttotal.Text.Length-1));
        int pago = 0;    // Forma de pago: 1=Tarjeta, 2=Efectivo, 3=Bizum
        if (btntarjeta.IsChecked == true) pago = 1;
        else if (btnefectivo.IsChecked == true) pago = 2;
        else if (btnbizum.IsChecked == true) pago = 3;
        int estado = 1;  // Estado inicial: pendiente de pago = 1
        string puntos = "";
        if (total > 20)
        {
            puntos = "+3";
        }

        if ((btnenLocal.IsChecked == btntelfono.IsChecked) ||
            (btntarjeta.IsChecked == btnefectivo.IsChecked &&
            btntarjeta.IsChecked == btnbizum.IsChecked) ||
            (total == 0) || hora == "")
        {
            MessageBox.Show("Faltan datos obligatorios o hay datos incorrectos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        else
        {

            pendientesDePago.Add(new Pedido(local, hora, domicilio, cliente, productos, total, pago, estado, puntos));

            Button_Click_2(sender, e);

            txtlogo.Text = local + " " + hora + " " + domicilio + " " + cliente + " " + productos + " " + total + " " + pago + " " + estado + " " + puntos;
        }

    }

}


