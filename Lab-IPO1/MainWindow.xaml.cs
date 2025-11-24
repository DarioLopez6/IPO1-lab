using System.Collections.ObjectModel;
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
    ObservableCollection<Producto> productosActuales = new ObservableCollection<Producto>();
    private List<Pedido> pedidos = new List<Pedido>();
    private List<Producto> productList = new List<Producto>();
    public MainWindow()
    {
        InitializeComponent();
        listaProductos.ItemsSource = productosActuales;
        productosActuales.Add(new Producto
        {
            Nombre = "Pizza Margarita",
            Cantidad = 1,
            Precio = 12.00
        });
        ActualizarTotal();
        CargarEjemplosPedidos();
        CargarPedidos(); //carga los pedidos de prueba en las 4 listas
    }
    private void AgregarPedidoAFase(Pedido p)
    {
        var card = CrearCardPedido(p);

        switch (p.Estado)
        {
            case 1: listaPendientesPago.Children.Add(card); break;
            case 2: listaEnElaboracion.Children.Add(card); break;
            case 3: listaListos.Children.Add(card); break;
            case 4: listaHistorial.Children.Add(card); break;
        }

        ActualizarContadores();
    }

    private void MoverPedidoSiguiente(Pedido p, Border card)
    {
        // quitar del contenedor actual
        switch (p.Estado)
        {
            case 1: listaPendientesPago.Children.Remove(card); break;
            case 2: listaEnElaboracion.Children.Remove(card); break;
            case 3: listaListos.Children.Remove(card); break;
            case 4: listaHistorial.Children.Remove(card); break;
        }

        // cambiar estado (simple incremento hasta 4)
        p.Estado = Math.Min(4, p.Estado + 1);

        // añadir al nuevo contenedor
        AgregarPedidoAFase(p);
    }

    private void EliminarPedido(Pedido p, Border card)
    {
        // quitar del contenedor visible
        switch (p.Estado)
        {
            case 1: listaPendientesPago.Children.Remove(card); break;
            case 2: listaEnElaboracion.Children.Remove(card); break;
            case 3: listaListos.Children.Remove(card); break;
            case 4: listaHistorial.Children.Remove(card); break;
        }

        // quitar de la lista maestra si procede
        pedidos.Remove(p);

        ActualizarContadores();
    }

    private void ActualizarContadores()
    {
        txtNumPendientes.Text = listaPendientesPago.Children.Count.ToString();
        txtNumEnElaboracion.Text = listaEnElaboracion.Children.Count.ToString();
        txtNumListos.Text = listaListos.Children.Count.ToString();
        txtNumHistorial.Text = listaHistorial.Children.Count.ToString();
    }

    private Border CrearCardPedido(Pedido p)
    {
        // Border principal
        var card = new Border
        {
            Background = Brushes.White,
            CornerRadius = new CornerRadius(8),
            BorderBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221)),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8),
            Margin = new Thickness(0, 0, 0, 8),
            Cursor = System.Windows.Input.Cursors.Hand
        };

        var root = new StackPanel();
        card.Child = root;

        // Header (siempre visible)
        var header = new Grid();
        header.ColumnDefinitions.Add(new ColumnDefinition());
        header.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var title = new TextBlock
        {
            Text = $"ID: {p.Id} - {p.Cliente}",
            FontWeight = FontWeights.Bold
        };

        var rightPanel = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
        rightPanel.Children.Add(new TextBlock { Text = p.Local ? "Local" : "Teléfono", Margin = new Thickness(0, 0, 10, 0) });
        rightPanel.Children.Add(new TextBlock { Text = $"{p.Total:0.00}€", FontWeight = FontWeights.Bold });
        var flecha = new TextBlock { Text = "▼", Margin = new Thickness(5, 0, 0, 0) };
        rightPanel.Children.Add(flecha);

        header.Children.Add(title);
        header.Children.Add(rightPanel);
        Grid.SetColumn(rightPanel, 1);

        root.Children.Add(header);

        // Detalle (oculto inicialmente)
        var detalle = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Visibility = Visibility.Collapsed };

        detalle.Children.Add(new TextBlock { Text = $"Fecha/Hora: {p.Hora}" });
        detalle.Children.Add(new TextBlock { Text = $"Dirección: {(string.IsNullOrEmpty(p.Domicilio) ? "—" : p.Domicilio)}" });
        detalle.Children.Add(new TextBlock { Text = $"Forma pago: {(p.Pago == 1 ? "Tarjeta" : p.Pago == 2 ? "Efectivo" : "Bizum")}" });

        // Productos (compacto)
        var productosText = string.Join(", ", p.Productos.Select(x => $"{x.Nombre} x{x.Cantidad}"));
        detalle.Children.Add(new TextBlock { Text = $"Productos: {productosText}" });

        // Botones de acción (ejemplo: mover estado, eliminar)
        var acciones = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0) };

        var btnMover = new Button { Content = "Siguiente fase", Padding = new Thickness(6, 2, 6, 2), Margin = new Thickness(0, 0, 8, 0) };
        btnMover.Click += (s, e) =>
        {
            MoverPedidoSiguiente(p, card);
        };

        var btnEliminar = new Button { Content = "Eliminar", Padding = new Thickness(6, 2, 6, 2) };
        btnEliminar.Click += (s, e) =>
        {
            EliminarPedido(p, card);
        };

        acciones.Children.Add(btnMover);
        acciones.Children.Add(btnEliminar);
        detalle.Children.Add(acciones);

        root.Children.Add(detalle);

        // Click en la tarjeta expande/colapsa
        card.MouseLeftButtonUp += (s, e) =>
        {
            if (detalle.Visibility == Visibility.Visible)
            {
                detalle.Visibility = Visibility.Collapsed;
                flecha.Text = "▼";
            }
            else
            {
                detalle.Visibility = Visibility.Visible;
                flecha.Text = "▲";
            }
        };

        // Guardar referencia al Pedido (opcional)
        card.Tag = p;

        return card;
    }

    private void CargarEjemplosPedidos()
    {
        // ejemplo corto
        var p1 = new Pedido(false,"hora", "domicilio", "cliente", productList, 10.0, 10, 1, "No tiene puntos");
        pedidos.Add(p1);
        AgregarPedidoAFase(p1);

        // ejemplo extendido
        var p2 = new Pedido(false, "hora", "domicilio", "cliente", productList, 10.0, 10, 1, "No tiene puntos");
        pedidos.Add(p2);
        AgregarPedidoAFase(p2);

        // ejemplo en elaboracion
        var p3 = new Pedido(false, "hora", "domicilio", "cliente", productList, 10.0, 10, 1, "No tiene puntos");
        pedidos.Add(p3);
        AgregarPedidoAFase(p3);

        // Actualiza contadores (si no se llaman en AgregarPedidoAFase)
        ActualizarContadores();
    }
    private void ActualizarTotal()
    {
        double total = productosActuales.Sum(p => p.Precio * p.Cantidad);

        txttotal.Text = total.ToString("0.00") + "€";
    }

    private void EliminarProducto_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Producto prod)
        {
            productosActuales.Remove(prod);
            ActualizarTotal(); // si tienes una función para recalcular
        }
    }


    private void CargarPedidos()
    {
        // Pedidos de prueba para la lista de pendientes de pago
        pendientesDePago.Add(new Pedido(false, "13:00", "Calle Falsa 123", "Juan Pérez", new List<Producto>(), 25.50, 2, 1, "+3"));
        pendientesDePago.Add(new Pedido(true, "14:30", "", "María López", new List<Producto>(), 15.00, 1, 1, ""));

        // Pedidos de prueba para la lista de en elaboración
        enElaboracion.Add(new Pedido(false, "12:45", "Avenida Siempre Viva 456", "Carlos García", new List<Producto>(), 30.00, 3, 2, "+3"));

        // Pedidos de prueba para la lista de listos para entregar
        listosParaEntregar.Add(new Pedido(true, "11:15", "", "Ana Martínez", new List<Producto>(), 20.00, 2, 3, ""));

        // Pedidos de prueba para la lista de historial
        historial.Add(new Pedido(false, "10:00", "Plaza Mayor 789", "Luis Fernández", new List<Producto>(), 18.75, 1, 4, ""));
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
            Pedido pedido = new Pedido(local, hora, domicilio, cliente, productos, total, pago, estado, puntos);
            pendientesDePago.Add(pedido);
            AgregarPedidoAFase(pedido);
            Button_Click_2(sender, e);
            ActualizarTotal();
            txtlogo.Text = local + " " + hora + " " + domicilio + " " + cliente + " " + productos + " " + total + " " + pago + " " + estado + " " + puntos;
        }

    }

}


