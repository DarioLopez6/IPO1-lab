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
}


