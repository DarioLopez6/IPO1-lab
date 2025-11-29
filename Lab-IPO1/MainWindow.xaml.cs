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
using System.Xml;
using System.Linq;

namespace Lab_IPO1;

public partial class MainWindow : Window
{
    List<Pedido> pendientesDePago = new List<Pedido>();
    List<Pedido> enElaboracion = new List<Pedido>();
    List<Pedido> listosParaEntregar = new List<Pedido>();
    List<Pedido> historial = new List<Pedido>();
    ObservableCollection<Plato> productosActuales = new ObservableCollection<Plato>();
    private List<Pedido> pedidos = new List<Pedido>();
    private List<Plato> productList = new List<Plato>();
    private List<Plato> listadoPlatos = new List<Plato>();

public MainWindow()
    {
        InitializeComponent();
        listadoPlatos = CargarContenidoXML();
        PlatosListView.ItemsSource = listadoPlatos;

        CargarPedidos();
        ActualizarTotal();
        CargarEjemplosPedidos();
        CargarPedidos(); // carga los pedidos de prueba en las 4 listas
        
        listadoPlatos = new List<Plato>
{
new Plato { Nombre = "Pizza Margarita", Precio = 8, Imagen = new Uri("/imagenes/pizza.png", UriKind.Relative), Cantidad = 0 },
new Plato { Nombre = "Hamburguesa con Queso", Precio = 6, Imagen = new Uri("/imagenes/hamburguesa.png", UriKind.Relative), Cantidad = 0 },
new Plato { Nombre = "Ensalada César", Precio = 5, Imagen = new Uri("/imagenes/ensalada.png", UriKind.Relative), Cantidad = 0 },
new Plato { Nombre = "Pasta Boloñesa", Precio = 7, Imagen = new Uri("/imagenes/pasta.png", UriKind.Relative), Cantidad = 0 },
new Plato { Nombre = "Taco Mexicano", Precio = 4, Imagen = new Uri("/imagenes/taco.png", UriKind.Relative), Cantidad = 0 }
};
        PlatosListView.ItemsSource = listadoPlatos;
        DataContext = listadoPlatos;

    }
    private void txtBuscador_TextChanged(object sender, TextChangedEventArgs e)
    {
        FiltrarYBuscarPedidos();
    }

    private void cmbFiltroPedido_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FiltrarYBuscarPedidos();
    }

    private void cmbFiltroPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FiltrarYBuscarPedidos();
    }


    private List<Plato> CargarContenidoXML()
    {
        List<Plato> listado = new List<Plato>();
        XmlDocument doc = new XmlDocument();
        doc.Load("Datos/platos.xml");

        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            if (node.Attributes == null) continue;

            Plato nuevoPlato = new Plato
            {
                Categoria = node.Attributes["Categoria"]?.Value ?? "",
                Subcategoria = node.Attributes["Subcategoria"]?.Value ?? "",
                Nombre = node.Attributes["Nombre"]?.Value ?? "",
                Ingredientes = node.Attributes["Ingredientes"]?.Value ?? "",
                Precio = int.Parse(node.Attributes["Precio"]?.Value ?? "0"),
                Alergenos = node.Attributes["Alergenos"]?.Value ?? "",
                Cantidad = int.Parse(node.Attributes["Cantidad"]?.Value ?? "0")
            };

            string img = node.Attributes["Imagen"]?.Value;
            if (!string.IsNullOrEmpty(img))
                nuevoPlato.Imagen = new Uri(img, UriKind.Relative);

            listado.Add(nuevoPlato);
        }

        return listado;
    }

    private void PlatosListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PlatosListView.SelectedItem is Plato prodSeleccionado)
        {
            var existente = productosActuales.FirstOrDefault(p => p.Nombre == prodSeleccionado.Nombre);
            if (existente != null)
            {
                existente.Cantidad++;
            }
            else
            {
                productosActuales.Add(new Plato
                {
                    Nombre = prodSeleccionado.Nombre,
                    Precio = prodSeleccionado.Precio,
                    Imagen = prodSeleccionado.Imagen,
                    Cantidad = 1
                });
            }
            ActualizarTotal();
            PlatosListView.SelectedItem = null;
        }
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
        switch (p.Estado)
        {
            case 1: listaPendientesPago.Children.Remove(card); break;
            case 2: listaEnElaboracion.Children.Remove(card); break;
            case 3: listaListos.Children.Remove(card); break;
            case 4: listaHistorial.Children.Remove(card); break;
        }

        p.Estado = Math.Min(4, p.Estado + 1);
        AgregarPedidoAFase(p);
    }

    private void EliminarPedido(Pedido p, Border card)
    {
        switch (p.Estado)
        {
            case 1: listaPendientesPago.Children.Remove(card); break;
            case 2: listaEnElaboracion.Children.Remove(card); break;
            case 3: listaListos.Children.Remove(card); break;
            case 4: listaHistorial.Children.Remove(card); break;
        }

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

    private void ActualizarTotal()
    {
        double total = productosActuales.Sum(p => p.Precio * p.Cantidad);
        txttotal.Text = total.ToString("0.00") + "€";
    }

    private Border CrearCardPedido(Pedido p)
    {
        var card = new Border
        {
            Background = Brushes.White,
            CornerRadius = new CornerRadius(8),
            BorderBrush = new SolidColorBrush(Color.FromRgb(221, 221, 221)),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(8),
            Margin = new Thickness(0, 0, 0, 8),
            Cursor = Cursors.Hand
        };

        var root = new StackPanel();
        card.Child = root;

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

        var detalle = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Visibility = Visibility.Collapsed };
        detalle.Children.Add(new TextBlock { Text = $"Fecha/Hora: {p.Hora}" });
        detalle.Children.Add(new TextBlock { Text = $"Dirección: {(string.IsNullOrEmpty(p.Domicilio) ? "—" : p.Domicilio)}" });
        detalle.Children.Add(new TextBlock { Text = $"Forma pago: {(p.Pago == 1 ? "Tarjeta" : p.Pago == 2 ? "Efectivo" : "Bizum")}" });

        var productosText = string.Join(", ", p.Platos.Select(x => $"{x.Nombre} x{x.Cantidad}"));
        detalle.Children.Add(new TextBlock { Text = $"Productos: {productosText}" });

        var acciones = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0) };
        var btnMover = new Button { Content = "Siguiente fase", Padding = new Thickness(6, 2, 6, 2), Margin = new Thickness(0, 0, 8, 0) };
        btnMover.Click += (s, e) => MoverPedidoSiguiente(p, card);

        var btnEliminar = new Button { Content = "Eliminar", Padding = new Thickness(6, 2, 6, 2) };
        btnEliminar.Click += (s, e) => EliminarPedido(p, card);

        acciones.Children.Add(btnMover);
        acciones.Children.Add(btnEliminar);
        detalle.Children.Add(acciones);
        root.Children.Add(detalle);

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

        card.Tag = p;
        return card;
    }

    private void CargarEjemplosPedidos()
    {
        var p1 = new Pedido(false, "hora", "domicilio", "cliente", productList, 10.0, 10, 1, "No tiene puntos");
        pedidos.Add(p1); AgregarPedidoAFase(p1);

        var p2 = new Pedido(false, "hora", "domicilio", "cliente", productList, 10.0, 10, 1, "No tiene puntos");
        pedidos.Add(p2); AgregarPedidoAFase(p2);

        var p3 = new Pedido(false, "hora", "domicilio", "cliente", productList, 10.0, 10, 1, "No tiene puntos");
        pedidos.Add(p3); AgregarPedidoAFase(p3);

        ActualizarContadores();
    }

    private void EliminarProducto_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Plato prod)
        {
            productosActuales.Remove(prod);
            ActualizarTotal();
        }
    }

    private void CargarPedidos()
    {
        pendientesDePago.Add(new Pedido(false, "13:00", "Calle Falsa 123", "Juan Pérez", new List<Plato>(), 25.50, 2, 1, "+3"));
        pendientesDePago.Add(new Pedido(true, "14:30", "", "María López", new List<Plato>(), 15.00, 1, 1, ""));

        enElaboracion.Add(new Pedido(false, "12:45", "Avenida Siempre Viva 456", "Carlos García", new List<Plato>(), 30.00, 3, 2, "+3"));

        listosParaEntregar.Add(new Pedido(true, "11:15", "", "Ana Martínez", new List<Plato>(), 20.00, 2, 3, ""));

        historial.Add(new Pedido(false, "10:00", "Plaza Mayor 789", "Luis Fernández", new List<Plato>(), 18.75, 1, 4, ""));
    }

    private void FiltrarYBuscarPedidos()
    {
        string busqueda = txtBuscador.Text.Trim().ToLower();
        string filtroPedido = "Todos";
        if (cmbFiltroPedido.SelectedItem is ComboBoxItem cpi && cpi.Content != null)
            filtroPedido = cpi.Content.ToString();
        string filtroPago = "Todos";
        if (cmbFiltroPago != null && cmbFiltroPago.SelectedItem is ComboBoxItem cpf && cpf.Content != null)
        {
            filtroPago = cpf.Content.ToString();
        }

        var filtrados = pedidos.Where(p =>
        {
            bool coincidePedido = filtroPedido == "Todos" || (filtroPedido == "Local" && p.Local) || (filtroPedido == "Teléfono" && !p.Local);

            string pagoReal = p.Pago switch
            {
                1 => "Tarjeta",
                2 => "Efectivo",
                _ => "Bizum"
            };

            bool coincidePago = filtroPago == "Todos" || pagoReal.Equals(filtroPago, System.StringComparison.OrdinalIgnoreCase);

            string textoCompleto =
                $"{p.Id} {p.Cliente} {p.Hora} {p.Domicilio} {(p.Local ? "Local" : "Teléfono")} {pagoReal} {p.Total} " +
                string.Join(" ", p.Platos.Select(prod => $"{prod.Nombre} {prod.Cantidad}"));

            bool coincideBusqueda = string.IsNullOrWhiteSpace(busqueda) || textoCompleto.ToLower().Contains(busqueda);

            return coincidePedido && coincidePago && coincideBusqueda;
        }).ToList();

        PintarPedidosFiltrados(filtrados);
    }

    private void PintarPedidosFiltrados(List<Pedido> lista)
    {
        if (listaPendientesPago == null || listaEnElaboracion == null ||
            listaListos == null || listaHistorial == null)
            return;

        listaPendientesPago.Children.Clear();
        listaEnElaboracion.Children.Clear();
        listaListos.Children.Clear();
        listaHistorial.Children.Clear();

        foreach (var p in lista)
        {
            var card = CrearCardPedido(p);

            switch (p.Estado)
            {
                case 1: listaPendientesPago.Children.Add(card); break;
                case 2: listaEnElaboracion.Children.Add(card); break;
                case 3: listaListos.Children.Add(card); break;
                case 4: listaHistorial.Children.Add(card); break;
            }
        }

        ActualizarContadores();
    }

    // Eventos de UI y botones
    public void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded) return;

        ImgProductos.Source = new BitmapImage(new Uri(TabProductos.IsSelected ? "/imagenes/menuProductosOn.png" : "/imagenes/menuProductosOff.png", UriKind.Relative));
        ImgPedidos.Source = new BitmapImage(new Uri(TabPedidos.IsSelected ? "/imagenes/menuPedidosOn.png" : "/imagenes/menuPedidosOff.png", UriKind.Relative));
        ImgClientes.Source = new BitmapImage(new Uri(TabClientes.IsSelected ? "/imagenes/menuClientesOn.png" : "/imagenes/menuClientesOff.png", UriKind.Relative));
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        LoginWindow main = new LoginWindow();
        main.Show();
        this.Close();
    }

    private void BtnAyuda_Click(object sender, RoutedEventArgs e)
    {
        HelpWindow help = new HelpWindow { Owner = this };
        help.ShowDialog();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        HelpWindow help = new HelpWindow { Owner = this };
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
        txterrorTipoPedido.Visibility = Visibility.Hidden;
        txterrorHora.Visibility = Visibility.Hidden;
        txterrorDomicilio.Visibility = Visibility.Hidden;
        txterrorCliente.Visibility = Visibility.Hidden;
        txterrorPago.Visibility = Visibility.Hidden;
        txtpedidoVacio.Visibility = Visibility.Hidden;
        txterrorPedido.Visibility = Visibility.Hidden;
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
        bool local = btnenLocal.IsChecked == true;
        string hora = txthora.Text;
        string domicilio = txtdomicilio.Text;
        string cliente = txtcliente.Text;
        List<Plato> platos = new List<Plato>();
        double total = double.Parse(txttotal.Text.Substring(0, txttotal.Text.Length - 1));
        int pago = btntarjeta.IsChecked == true ? 1 : btnefectivo.IsChecked == true ? 2 : btnbizum.IsChecked == true ? 3 : 0;
        int estado = 1;
        string puntos = total > 20 ? "+3" : "";

        if ((btnenLocal.IsChecked == btntelfono.IsChecked) ||
            (btntarjeta.IsChecked == btnefectivo.IsChecked && btntarjeta.IsChecked == btnbizum.IsChecked) ||
            total == 0 || string.IsNullOrEmpty(hora) || string.IsNullOrWhiteSpace(cliente) ||
            (btntelfono.IsChecked == true && string.IsNullOrEmpty(domicilio)))
        {
            if (!btnenLocal.IsChecked.Value && !btntelfono.IsChecked.Value) txterrorTipoPedido.Visibility = Visibility.Visible; else txterrorTipoPedido.Visibility = Visibility.Hidden;
            if (string.IsNullOrWhiteSpace(hora)) txterrorHora.Visibility = Visibility.Visible; else txterrorHora.Visibility = Visibility.Hidden;
            if (btntelfono.IsChecked == true && string.IsNullOrEmpty(domicilio)) txterrorDomicilio.Visibility = Visibility.Visible; else txterrorDomicilio.Visibility = Visibility.Hidden;
            if (string.IsNullOrWhiteSpace(cliente)) txterrorCliente.Visibility = Visibility.Visible; else txterrorCliente.Visibility = Visibility.Hidden;
            if (!btntarjeta.IsChecked.Value && !btnefectivo.IsChecked.Value && !btnbizum.IsChecked.Value) txterrorPago.Visibility = Visibility.Visible; else txterrorPago.Visibility = Visibility.Hidden;
            if (total == 0) txtpedidoVacio.Visibility = Visibility.Visible; else txtpedidoVacio.Visibility = Visibility.Hidden;
            if (txterrorTipoPedido.Visibility == Visibility.Visible ||
                txterrorHora.Visibility == Visibility.Visible ||
                txterrorCliente.Visibility == Visibility.Visible ||
                txterrorPago.Visibility == Visibility.Visible ||
                txtpedidoVacio.Visibility == Visibility.Visible) txterrorPedido.Visibility = Visibility.Visible;
            else txterrorPedido.Visibility = Visibility.Hidden;
        }
        else
        {
            txterrorDomicilio.Visibility = Visibility.Hidden;
            Pedido pedido = new Pedido(local, hora, domicilio, cliente, platos, total, pago, estado, puntos);
            pendientesDePago.Add(pedido);
            pedidos.Add(pedido);
            AgregarPedidoAFase(pedido);
            Button_Click_2(sender, e);
            ActualizarTotal();
            txtlogo.Text = $"{local} {hora} {domicilio} {cliente} {platos} {total} {pago} {estado} {puntos}";
        }
    }


}
