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
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace Lab_IPO1;

public partial class MainWindow : Window
{
    // Listas de pedidos
    private List<Pedido> pendientesDePago = new List<Pedido>();
    private List<Pedido> enElaboracion = new List<Pedido>();
    private List<Pedido> listosParaEntregar = new List<Pedido>();
    private List<Pedido> historial = new List<Pedido>();
    private List<Pedido> pedidos = new List<Pedido>();

    // Listas de productos y clientes
    private ObservableCollection<Plato> productosActuales = new ObservableCollection<Plato>();
    private ObservableCollection<Cliente> clientesFiltrados;
    private List<Plato> productList = new List<Plato>();
    private List<Plato> listadoPlatos = new List<Plato>();
    private List<Cliente> misClientes = new List<Cliente>();
    private List<Cliente> clientesBase;
    private Cliente clienteSeleccionado;
    private Cliente clienteSeleccionadoBox;
    private CollectionViewSource PlatosViewSource;

    private int puntosUsados = 0;
    private int puntosCliente = 0; // se asigna al seleccionar cliente
    private double envioBase = 3.00;
    private double totalBase = 0;
    private double envioFinal = 0;
    private double totalFinal = 0;



    public MainWindow()
    {
        InitializeComponent();



        // Cargar datos de platos desde XML
        listadoPlatos = CargarContenidoXML();
        PlatosViewSource = new CollectionViewSource { Source = listadoPlatos };
        PlatosViewSource.Filter += PlatosFiltrado;
        PlatosListView.ItemsSource = PlatosViewSource.View;


        // Cargar pedidos de prueba
        CargarPedidos();
        CargarEjemplosPedidos();

        // Inicializar clientes
        misClientes = new List<Cliente>
            {
                new Cliente("imagenes/perfil.png", 1, "Juan", "Pérez", new List<string> { "Calle Falsa 123" }, new List<string> { "666 555 444" }, new List<string>(), new List<string>(), new List<string>(), FORMAPAGO.EFECTIVO, 10, 0),
                new Cliente("imagenes/perfil.png", 2, "Ana", "García", new List<string> { "Avenida Siempre Viva 45" }, new List<string> { "699 111 222" }, new List<string>(), new List<string>(), new List<string>(), FORMAPAGO.TARGETA, 10, 0),
                new Cliente("imagenes/perfil.png", 3, "Juan", "Pérez", new List<string> { "Calle Mayor 12" }, new List<string> { "666 555 444" }, new List<string>(), new List<string>(), new List<string>(), FORMAPAGO.BIZUM, 10, 0),
                new Cliente("imagenes/perfil.png", 4, "Juan", "Pérez", new List<string> { "Calle Luna 7" }, new List<string> { "666 555 444" }, new List<string>(), new List<string>(), new List<string>(), FORMAPAGO.BIZUM, 10, 0)
            };

        clientesBase = misClientes;
        clientesFiltrados = new ObservableCollection<Cliente>(clientesBase);
        cbBuscarCliente.ItemsSource = clientesFiltrados;
        



        TxtTotalClientes.Inlines.Clear();
        TxtTotalClientes.Inlines.Add(new Run($"{clientesFiltrados.Count} clientes registrados"));


        listaProductos.ItemsSource = productosActuales;

        // Suscribirse a cambios en tipo de pedido
        btnenLocal.Checked += TipoPedido_CheckedChanged;
        btntelfono.Checked += TipoPedido_CheckedChanged;
        btnenLocal.Unchecked += TipoPedido_CheckedChanged;
        btntelfono.Unchecked += TipoPedido_CheckedChanged;

        TxtBuscarPlato.TextChanged += (s, e) => PlatosViewSource.View.Refresh();
    
        btnPrimeros.Checked += MostrarSegundaCategoria;
        btnSegundos.Checked += MostrarSegundaCategoria;

        btnPrimeros.Unchecked += OcultarSegundaCategoriaSiNoHaySeleccion;
        btnSegundos.Unchecked += OcultarSegundaCategoriaSiNoHaySeleccion;

        btnEntrantes.Checked += ActualizarFiltro;
        btnPrimeros.Checked += ActualizarFiltro;
        btnSegundos.Checked += ActualizarFiltro;
        btnPostres.Checked += ActualizarFiltro;
        btnBebidas.Checked += ActualizarFiltro;

        btnEntrantes.Unchecked += ActualizarFiltro;
        btnPrimeros.Unchecked += ActualizarFiltro;
        btnSegundos.Unchecked += ActualizarFiltro;
        btnPostres.Unchecked += ActualizarFiltro;
        btnBebidas.Unchecked += ActualizarFiltro;

        // Subcategorías
        btnEnsaladas.Checked += ActualizarFiltro;
        btnHuevos.Checked += ActualizarFiltro;
        btnArrocesYPastas.Checked += ActualizarFiltro;
        btnPescados.Checked += ActualizarFiltro;

        btnEnsaladas.Unchecked += ActualizarFiltro;
        btnHuevos.Unchecked += ActualizarFiltro;
        btnArrocesYPastas.Unchecked += ActualizarFiltro;
        btnPescados.Unchecked += ActualizarFiltro;

        // Estado inicial
        UpdateDomicilioState();
        ActualizarClientes();
        ActualizarTotal();

    }
    private void RestarCantidad_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is Plato plato)
        {
            if (plato.Cantidad > 1)
            {
                plato.Cantidad--;
            }
            else
            {
                // Si la cantidad llega a 0, puedes optar por eliminarlo completamente:
                productosActuales.Remove(plato);
            }

            ActualizarTotal();
        }
    }





    private void PlatosFiltrado(object sender, FilterEventArgs e)
    {
        if (e.Item is Plato plato)
        {
            // Categoría
            string categoria = btnEntrantes.IsChecked == true ? "Entrante" :
                               btnPrimeros.IsChecked == true ? "Primero" :
                               btnSegundos.IsChecked == true ? "Segundo" :
                               btnPostres.IsChecked == true ? "Postre" :
                               btnBebidas.IsChecked == true ? "Bebida" : null;

            // Subcategoría
            string subcategoria = null;
            if (FilaSegundaCategoria.Visibility == Visibility.Visible)
            {
                subcategoria = btnEnsaladas.IsChecked == true ? "Ensalada" :
                               btnHuevos.IsChecked == true ? "Carne" :
                               btnArrocesYPastas.IsChecked == true ? "Arroces y Pastas" :
                               btnPescados.IsChecked == true ? "Pescado" : null;
            }

            // Texto del buscador
            string busqueda = TxtBuscarPlato.Text?.Trim().ToLower() ?? "";

            // Evaluar coincidencias
            bool coincideCategoria = categoria == null || plato.Categoria == categoria;
            bool coincideSubcategoria = subcategoria == null || plato.Subcategoria == subcategoria;
            bool coincideTexto = string.IsNullOrEmpty(busqueda) ||
                                  plato.Nombre.ToLower().Contains(busqueda) ||
                                  plato.Ingredientes.ToLower().Contains(busqueda);

            e.Accepted = coincideCategoria && coincideSubcategoria && coincideTexto;
        }
    }

    private void ActualizarFiltro(object sender, RoutedEventArgs e)
    {
        PlatosViewSource.View.Refresh();
    }

    private void TipoPedido_CheckedChanged(object? sender, RoutedEventArgs e) => UpdateDomicilioState();


    private void UpdateDomicilioState()
    {
        bool domicilioHabilitado = btntelfono.IsChecked == true;

        if (btnDomicilioContainer != null)
        {
            btnDomicilioContainer.IsEnabled = domicilioHabilitado;
            btnDomicilioContainer.Opacity = domicilioHabilitado ? 1.0 : 0.6;
            btnDomicilioContainer.BorderBrush = domicilioHabilitado
                ? new SolidColorBrush(Color.FromRgb(204, 204, 204))
                : new SolidColorBrush(Color.FromRgb(220, 220, 220));
        }

        if (txtdomicilio != null)
        {
            txtdomicilio.IsEnabled = domicilioHabilitado;
            txtdomicilio.Foreground = domicilioHabilitado ? Brushes.Black : Brushes.Gray;
        }
    }

    private void cbBuscarCliente_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        string texto = cbBuscarCliente.Text.Trim().ToLower();

        var filtrados = clientesBase
            .Where(c => c.Nombre.ToLower().Contains(texto) ||
                        c.Apellidos.ToLower().Contains(texto))
            .ToList();

        clientesFiltrados.Clear();
        foreach (var c in filtrados)
            clientesFiltrados.Add(c);

        cbBuscarCliente.IsDropDownOpen = true; // muestra sugerencias en tiempo real
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

        // Ruta absoluta al XML dentro de la carpeta Datos
        string rutaXml = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "platos.xml");

        if (!System.IO.File.Exists(rutaXml))
        {
            MessageBox.Show($"No se encontró el archivo XML en: {rutaXml}");
            return listado;
        }

        XmlDocument doc = new XmlDocument();
        doc.Load(rutaXml);

        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            if (node.Attributes == null) continue;

            Plato plato = new Plato
            {
                Nombre = node.Attributes["nombre"]?.Value ?? "",
                Categoria = node.Attributes["categoria"]?.Value ?? "",
                Subcategoria = node.Attributes["subcategoria"]?.Value ?? "",
                Ingredientes = node.Attributes["ingredientes"]?.Value ?? "",
                Precio = double.TryParse(node.Attributes["precio"]?.Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double precio) ? precio : 0,
                Alergenos = node.Attributes["alergenos"]?.Value ?? "",
                Cantidad = 1, // por defecto 1
                Imagen = CargarImagen(node.Attributes["imagen"]?.Value)
            };

            listado.Add(plato);
        }

        return listado;
    }

    // Función para cargar imágenes correctamente
    private Uri CargarImagen(string ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta))
            return new Uri("imagenes/logo.png", UriKind.Relative);

        try
        {
            return new Uri(ruta, UriKind.Relative);
        }
        catch
        {
            return new Uri("imagenes/logo.png", UriKind.Relative);
        }
    }




    private void PlatosListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PlatosListView.SelectedItem is Plato prodSeleccionado)
        {
            var existente = productosActuales.FirstOrDefault(p => p.Nombre == prodSeleccionado.Nombre);
            if (existente != null) existente.Cantidad++;
            else
                productosActuales.Add(new Plato
                {
                    Nombre = prodSeleccionado.Nombre,
                    Precio = prodSeleccionado.Precio,
                    Imagen = prodSeleccionado.Imagen ?? new Uri("imagenes/logo.png", UriKind.Relative),
                    Cantidad = 1
                });

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
        // Recalcular los totales base
        totalBase = productosActuales.Sum(p => p.Precio * p.Cantidad);
        envioBase = 3;

        // Y AHORA aplicar puntos correctamente usando los valores base
        AplicarPuntos();
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
        detalle.Children.Add(new TextBlock { Text = $"Fecha/Hora realización: {DateTime.Now}" });
        detalle.Children.Add(new TextBlock { Text = $"Fecha/Hora: {p.Hora}" });
        detalle.Children.Add(new TextBlock { Text = $"Dirección: {(string.IsNullOrEmpty(p.Domicilio) ? "—" : p.Domicilio)}" });
        detalle.Children.Add(new TextBlock { Text = $"Forma pago: {(p.Pago == 1 ? "Tarjeta" : p.Pago == 2 ? "Efectivo" : "Bizum")}" });

        var productosLabel = new TextBlock { Text = "Productos:", FontWeight = FontWeights.Bold };
        detalle.Children.Add(productosLabel);

        foreach (var plato in p.Platos)
        {
            detalle.Children.Add(new TextBlock
            {
                Text = $"{plato.Nombre} x{plato.Cantidad} - {plato.Precio * plato.Cantidad:0.00}€",
                Margin = new Thickness(10, 0, 0, 0)
            });
        }

        var acciones = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 8, 0, 0), HorizontalAlignment = HorizontalAlignment.Left };

        // Botón "Siguiente fase" - Naranja
        var btnMover = CrearBoton(
            "Siguiente fase",
            Color.FromRgb(255, 127, 0),
            Colors.White,
            null,
            (s, e) => MoverPedidoSiguiente(p, card)
        );

        // Botón "Editar" - Blanco con borde gris
        var btnEditar = CrearBoton(
            "Editar",
            Colors.White,
            Colors.Black,
            Color.FromRgb(200, 200, 200),
            (s, e) =>
            {
                BtnEditarPedido_Click(s, e,p);
            }
        );

        // Botón "Eliminar" - Rojo
        var btnEliminar = CrearBoton(
            "Eliminar",
            Color.FromRgb(220, 53, 69),
            Colors.White,
            null,
            (s, e) =>
            {
                var resultado = MessageBox.Show(
                    $"¿Está usted seguro de que desea eliminar el pedido del cliente {p.Cliente}? Esta acción no podrá deshacerse.",
                    "Confirmación de eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );
                if (resultado == MessageBoxResult.Yes)
                    EliminarPedido(p, card);
            }
        );

        acciones.Children.Add(btnMover);
        acciones.Children.Add(btnEditar); // EDITAR antes de eliminar
        acciones.Children.Add(btnEliminar);
        detalle.Children.Add(acciones);

        root.Children.Add(detalle);

        // Toggle detalle al hacer clic en la tarjeta
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

    private void CargarPedidoActual(Pedido p)
    {
        // Limpiar pedido actual
        productosActuales.Clear();

        // Tipo de pedido
        btnenLocal.IsChecked = p.Local;
        btntelfono.IsChecked = !p.Local;
        UpdateDomicilioState();

        // Hora y domicilio
        txthora.Text = p.Hora;
        txtdomicilio.Text = p.Domicilio;

        // Cliente
        cbBuscarCliente.Text = p.Cliente;
        clienteSeleccionadoBox = misClientes.FirstOrDefault(c => $"{c.Nombre} {c.Apellidos}" == p.Cliente);

        // Forma de pago
        btntarjeta.IsChecked = p.Pago == 1;
        btnefectivo.IsChecked = p.Pago == 2;
        btnbizum.IsChecked = p.Pago == 3;

        // Productos
        foreach (var plato in p.Platos)
        {
            productosActuales.Add(new Plato
            {
                Nombre = plato.Nombre,
                Precio = plato.Precio,
                Imagen = plato.Imagen,
                Cantidad = plato.Cantidad,
                Categoria = plato.Categoria,
                Subcategoria = plato.Subcategoria,
                Ingredientes = plato.Ingredientes,
                Alergenos = plato.Alergenos
            });
        }

        // Actualizar total
        ActualizarTotal();
    }


    private Button CrearBoton(string texto, Color colorFondo, Color colorTexto, Color? colorBorde, RoutedEventHandler clickHandler)
    {
        var txt = new TextBlock
        {
            Text = texto,
            Foreground = new SolidColorBrush(colorTexto),
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = FontWeights.SemiBold
        };

        var border = new Border
        {
            Background = new SolidColorBrush(colorFondo),
            BorderBrush = colorBorde.HasValue ? new SolidColorBrush(colorBorde.Value) : null,
            BorderThickness = colorBorde.HasValue ? new Thickness(2) : new Thickness(0),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 6, 12, 6),
            Child = txt
        };

        var btn = new Button
        {
            Content = border,
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Transparent,
            Cursor = Cursors.Hand
        };

        btn.Click += clickHandler;
        return btn;
    }



    private void BtnEditarPedido_Click(object sender, RoutedEventArgs e, Pedido pedidoSeleccionado)
    {
        if (pedidoSeleccionado == null)
            return;

        // Mensaje de aviso
        var resultado = MessageBox.Show(
            "Se usará la sección de 'Pedido Actual' para modificar este pedido.\n" +
            "Todos los datos que hubiera escrito se borrarán. ¿Desea continuar?",
            "Editar Pedido",
            MessageBoxButton.YesNo,
            MessageBoxImage.Information
        );

        if (resultado != MessageBoxResult.Yes)
            return;

        // Limpiar pedido actual
        productosActuales.Clear();
        txtdomicilio.Text = "";
        txthora.Text = "";
        cbBuscarCliente.Text = "";
        btntarjeta.IsChecked = false;
        btnefectivo.IsChecked = false;
        btnbizum.IsChecked = false;
        btnenLocal.IsChecked = false;
        btntelfono.IsChecked = false;
        ActualizarTotal();

        // Rellenar con datos del pedido seleccionado
        if (pedidoSeleccionado.Local)
            btnenLocal.IsChecked = true;
        else
            btntelfono.IsChecked = true;

        txthora.Text = pedidoSeleccionado.Hora;
        txtdomicilio.Text = pedidoSeleccionado.Domicilio;
        cbBuscarCliente.Text = pedidoSeleccionado.Cliente;

        switch (pedidoSeleccionado.Pago)
        {
            case 1: btntarjeta.IsChecked = true; break;
            case 2: btnefectivo.IsChecked = true; break;
            case 3: btnbizum.IsChecked = true; break;
        }

        // Copiar los productos al pedido actual
        foreach (var p in pedidoSeleccionado.Platos)
        {
            productosActuales.Add(new Plato
            {
                Nombre = p.Nombre,
                Precio = p.Precio,
                Imagen = p.Imagen,
                Cantidad = p.Cantidad,
                Categoria = p.Categoria,
                Subcategoria = p.Subcategoria,
                Ingredientes = p.Ingredientes,
                Alergenos = p.Alergenos
            });
        }

        ActualizarTotal();

        // Cambiar a pestaña de productos
        MainTabControl.SelectedItem = TabProductos;
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

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        HelpWindow help = new HelpWindow("• Pestañas productos, pedidos y clientes:", "Gestiona cada cosa con la interfaz proporcionada") { Owner = this };
        help.ShowDialog();
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        btnenLocal.IsChecked = false;
        btntelfono.IsChecked = false;
        btnbizum.IsChecked = false;
        btnefectivo.IsChecked = false;
        btntarjeta.IsChecked = false;
        cbBuscarCliente.Text = "";
        txtdomicilio.Text = "";
        txthora.Text = "";
        txterrorTipoPedido.Visibility = Visibility.Hidden;
        txterrorHora.Visibility = Visibility.Hidden;
        txterrorDomicilio.Visibility = Visibility.Hidden;
        txterrorCliente.Visibility = Visibility.Hidden;
        txterrorPago.Visibility = Visibility.Hidden;
        txtpedidoVacio.Visibility = Visibility.Hidden;
        txterrorPedido.Visibility = Visibility.Hidden;
        productosActuales.Clear();
        txttotal.Text = "0.00€";
        
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        expander.IsExpanded = true;
        Button_Click_2(sender, e);
    }

    private void Btnautoria_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Aplicación desarrollada por Rubén, Víctor y Darío. \nFecha: 1/12/2025.\nPrimer prototipo(V0.6).", "Acerca de", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void Button_Click_4(object sender, RoutedEventArgs e)
    {
        bool local = btnenLocal.IsChecked == true;
        string hora = txthora.Text;
        string domicilio = txtdomicilio.Text;
        string cliente = cbBuscarCliente.Text;
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
            if (productosActuales.Count == 0) txtpedidoVacio.Visibility = Visibility.Visible; else txtpedidoVacio.Visibility = Visibility.Hidden;
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

            // Creamos una copia profunda de los productos actuales
            List<Plato> platos = productosActuales.Select(p => new Plato
            {
                Nombre = p.Nombre,
                Precio = p.Precio,
                Imagen = p.Imagen,
                Cantidad = p.Cantidad,
                Categoria = p.Categoria,
                Subcategoria = p.Subcategoria,
                Ingredientes = p.Ingredientes,
                Alergenos = p.Alergenos
            }).ToList();

            Pedido pedido = new Pedido(local, hora, domicilio, cliente, platos, total, pago, estado, puntos);
            pendientesDePago.Add(pedido);
            pedidos.Add(pedido);
            AgregarPedidoAFase(pedido);
            if (clienteSeleccionado != null)
            {
                if (clienteSeleccionado.Historial == null)
                    clienteSeleccionado.Historial = new List<Pedido>();

                clienteSeleccionado.Historial.Add(pedido);
                FichaClienteBorder.Visibility=Visibility.Collapsed;
            }

            if (clienteSeleccionadoBox != null)
            {
                // 1️⃣ Restar puntos usados (solo si se usaron)
                if (puntosUsados > 0)
                {
                    clienteSeleccionadoBox.puntosAcumulados -= puntosUsados;
                    if (clienteSeleccionadoBox.puntosAcumulados < 0)
                        clienteSeleccionadoBox.puntosAcumulados = 0;
                }

                // 2️⃣ Calcular lo que realmente pagó sin puntos
                double totalSinPuntos = totalBase + envioBase;
                double totalPagadoReal = totalSinPuntos - puntosUsados;

                // 3️⃣ Si ha pagado 20€ o más → gana +3 puntos
                if (totalPagadoReal >= 20)
                    clienteSeleccionadoBox.puntosAcumulados += 3;

                // 4️⃣ Ocultar el panel de puntos
                borderPuntos.Visibility = Visibility.Hidden;

                // 5️⃣ Guardar y refrescar los puntos en pantalla
                TxtPuntosAcumulados.Text = clienteSeleccionadoBox.puntosAcumulados.ToString();
            }

            // Limpiar pedido actual
            productosActuales.Clear();
            ActualizarTotal();

            // Opcional: limpiar inputs
            Button_Click_2(sender, e);
        }
    }

    private void btnanadirCliente_Click(object sender, RoutedEventArgs e)
    {
        CrearCliente ventana = new CrearCliente();

        ventana.Owner = this;     // <-- IMPORTANTE para bloquear la ventana principal
        ventana.ShowInTaskbar = false;

        if (ventana.ShowDialog() == true)
        {
            Cliente nuevo = ventana.NuevoCliente;
            misClientes.Add(nuevo);
            ActualizarClientes();
        }
        FichaClienteBorder.Visibility = Visibility.Collapsed;
        
    }
    private void ActualizarClientes()
    {
        ListaClientes.Items.Clear();

        foreach (var cliente in misClientes)
        {
            // Crear el borde principal
            Border border = new Border
            {
                Width = 180,
                Height = 55,
                Margin = new Thickness(5),
                Background = Brushes.White,
                Padding = new Thickness(10),
                CornerRadius = new CornerRadius(10),
                BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand
            };
            border.MouseLeftButtonDown += (s, e) =>
            {
                MostrarFichaCliente(cliente);
            };

            // Crear el Grid interno
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Imagen de perfil
            Border imgBorder = new Border
            {
                Width = 38,
                Height = 38,
                CornerRadius = new CornerRadius(19),
                ClipToBounds = true,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Crear imagen con fallback a imagen por defecto
            Image img = new Image { Stretch = Stretch.UniformToFill };
            BitmapImage bitmap;

            if (string.IsNullOrWhiteSpace(cliente.Imagen))
            {
                // Imagen por defecto
                bitmap = new BitmapImage(new Uri("imagenes/perfil.png", UriKind.Relative));
            }
            else
            {
                try
                {
                    bitmap = new BitmapImage(new Uri(cliente.Imagen, UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    bitmap = new BitmapImage(new Uri("imagenes/perfil.png", UriKind.Relative));
                }
            }

            img.Source = bitmap;
            imgBorder.Child = img;
            grid.Children.Add(imgBorder);

            // Datos del cliente
            StackPanel sp = new StackPanel
            {
                Margin = new Thickness(10, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock nombre = new TextBlock
            {
                Text = cliente.Nombre + " " + cliente.Apellidos,
                FontWeight = FontWeights.Bold,
                FontSize = 14
            };

            TextBlock telefono = new TextBlock
            {
                Text = cliente.Telefono.Count > 0 ? cliente.Telefono[0] : "",
                FontSize = 12,
                Foreground = Brushes.Gray
            };

            sp.Children.Add(nombre);
            sp.Children.Add(telefono);

            Grid.SetColumn(sp, 1);
            grid.Children.Add(sp);

            // Añadir Grid al Border
            border.Child = grid;

            // Añadir Border al ItemsControl
            ListaClientes.Items.Add(border);
        }
    }


    private void BuscadorClientes_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro = BuscadorClientes.Text.Trim().ToLower();

        ListaClientes.Items.Clear();

        foreach (var cliente in misClientes)
        {
            // Concatenamos nombre, apellidos y primer teléfono
            string datosConcatenados = (cliente.Nombre + " " + cliente.Apellidos + " " +
                                       (cliente.Telefono.Count > 0 ? cliente.Telefono[0] : "")).ToLower();

            if (datosConcatenados.Contains(filtro))
            {
                // Crear el borde principal
                Border border = new Border
                {
                    Width = 180,
                    Height = 55,
                    Margin = new Thickness(5),
                    Background = Brushes.White,
                    Padding = new Thickness(10),
                    CornerRadius = new CornerRadius(10),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    BorderThickness = new Thickness(1),
                    Cursor = Cursors.Hand
                };
                border.MouseLeftButtonDown += (s, e) =>
                {
                    MostrarFichaCliente(cliente);
                };

                // Grid interno
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                // Imagen de perfil
                Border imgBorder = new Border
                {
                    Width = 38,
                    Height = 38,
                    CornerRadius = new CornerRadius(19),
                    ClipToBounds = true,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri("/imagenes/perfil.png", UriKind.Relative)),
                    Stretch = Stretch.UniformToFill
                };
                imgBorder.Child = img;
                grid.Children.Add(imgBorder);

                // Datos del cliente
                StackPanel sp = new StackPanel
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                TextBlock nombre = new TextBlock
                {
                    Text = cliente.Nombre + " " + cliente.Apellidos,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14
                };
                TextBlock telefono = new TextBlock
                {
                    Text = cliente.Telefono.Count > 0 ? cliente.Telefono[0] : "",
                    FontSize = 12,
                    Foreground = Brushes.Gray
                };
                sp.Children.Add(nombre);
                sp.Children.Add(telefono);

                Grid.SetColumn(sp, 1);
                grid.Children.Add(sp);

                border.Child = grid;
                ListaClientes.Items.Add(border);
            }
        }

    }

    private void MostrarFichaCliente(Cliente cliente)
    {
        clienteSeleccionado = cliente;
        // Hacer visible la ficha
        FichaClienteBorder.Visibility = Visibility.Visible;

        // Cabecera
        TxtFichaCliente.Text = $"Ficha de cliente - {cliente.Nombre} {cliente.Apellidos}";

        // Datos personales
        TxtNombreApellido.Text = $"{cliente.Nombre} {cliente.Apellidos}";
        TxtTelefono.Text = cliente.Telefono.Count > 0 ? cliente.Telefono[0] : "";
        TxtCorreo.Text = cliente.eMail.Count > 0 ? cliente.eMail[0] : "";

        // Direcciones
        TxtDireccionPrincipal.Text = cliente.Direccion.Count > 0 ? cliente.Direccion[0] : "";
        TxtDireccionSecundaria.Text = cliente.Direccion.Count > 1 ? cliente.Direccion[1] : "";

        // Alergias e intolerancias
        TxtAlergias.Text = cliente.Alergias.Count > 0 ? string.Join(", ", cliente.Alergias) : "Ninguna";
        TxtIntolerancias.Text = cliente.Intolerancias.Count > 0 ? string.Join(", ", cliente.Intolerancias) : "Ninguna";

        // Forma de pago
        TxtFormaPagoPreferida.Text = cliente.pago switch
        {
            FORMAPAGO.TARGETA => "Tarjeta",
            FORMAPAGO.EFECTIVO => "Efectivo",
            FORMAPAGO.BIZUM => "Bizum",
            _ => ""
        };

        // Puntos
        TxtPuntosAcumulados.Text = cliente.puntosAcumulados.ToString();
        TxtPuntosCanjeados.Text = cliente.puntosCangeados.ToString();

        // Historial de pedidos
        if (cliente.Historial != null && cliente.Historial.Count > 0)
            TxtHistorialPedidos.Text = string.Join(", ", cliente.Historial.Select(p => p.ToString()));
        else
            TxtHistorialPedidos.Text = "No hay pedidos";
    }

    private void BtnEditarCliente_Click(object sender, RoutedEventArgs e)
    {
        if (clienteSeleccionado == null)
        {
            MessageBox.Show("No hay cliente seleccionado para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        EditarCliente ventanaEditar = new EditarCliente(clienteSeleccionado);
        ventanaEditar.Owner = this;
        bool? resultado = ventanaEditar.ShowDialog();

        if (resultado == true)
        {
            // Aquí puedes refrescar cualquier control que muestre al cliente
            ActualizarClientes();
            FichaClienteBorder.Visibility = Visibility.Collapsed;
        }
    }

    private void cbBuscarCliente_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        clienteSeleccionadoBox = (Cliente)cbBuscarCliente.SelectedItem;

        if (clienteSeleccionadoBox != null)
        {
            borderPuntos.Visibility = Visibility.Visible;
            puntosCliente = clienteSeleccionadoBox.puntosAcumulados;
            puntosUsados = 0;
            AplicarPuntos();
        }
        else
        {
            borderPuntos.Visibility = Visibility.Hidden;
            puntosUsados = 0;
            AplicarPuntos();
        }
    }

    private void SumarPuntos_Click(object sender, MouseButtonEventArgs e)
    {
        if (clienteSeleccionadoBox == null) return;

        // Precio FINAL actual
        double precioActual = envioFinal + totalFinal;

        // No dejar sumar puntos si ya no hay nada que descontar
        if (precioActual <= 0)
            return;

        // No permitir más puntos que el cliente tiene
        if (puntosUsados >= puntosCliente)
            return;

        puntosUsados++;
        AplicarPuntos();
    }



    private void RestarPuntos_Click(object sender, MouseButtonEventArgs e)
    {
        if (puntosUsados > 0)
        {
            puntosUsados--;

            // Recalcular SIEMPRE en base al precio actual
            AplicarPuntos();
        }
    }

    private void txthora_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (Regex.IsMatch(txthora.Text, @"^\d{1,2}:\d{2}$") || txthora.Text == "")
            txterrorletras.Visibility = Visibility.Collapsed;
        else
            txterrorletras.Visibility = Visibility.Visible;

        txterrorletras.Text = "Formato válido: HH:mm";
        
    }
    private void AplicarPuntos()
    {
        double envio = envioBase;
        double total = totalBase;

        // Precio total actual
        double precioTotal = envio + total;

        // Si no hay cliente, no tocar nada
        if (clienteSeleccionadoBox == null)
        {
            envioFinal = envio;
            totalFinal = total;
            txtEnvio.Text = envioFinal.ToString("0.00") + "€";
            txttotal.Text = totalFinal.ToString("0.00") + "€";
            return;
        }

        // 🔥 Ajustar puntos usados para no malgastarlos
        if (puntosUsados > precioTotal)
            puntosUsados = (int)Math.Floor(precioTotal);

        if (puntosUsados > puntosCliente)
            puntosUsados = puntosCliente;

        int puntos = puntosUsados;

        // 1) Descontar del envío
        double restarEnvio = Math.Min(envio, puntos);
        envio -= restarEnvio;
        puntos -= (int)restarEnvio;

        // 2) Descontar del total
        double restarTotal = Math.Min(total, puntos);
        total -= restarTotal;
        puntos -= (int)restarTotal;

        // Evitar negativos
        envioFinal = Math.Max(0, envio);
        totalFinal = Math.Max(0, total);

        // Actualizar interfaz
        txtPuntosUsados.Text = puntosUsados.ToString();
        txtEnvio.Text = envioFinal.ToString("0.00") + "€";
        txttotal.Text = totalFinal.ToString("0.00") + "€";
    }



    private void BtnEliminarCliente_Click(object sender, RoutedEventArgs e)
    {
        if (clienteSeleccionado == null)
        {
            MessageBox.Show(
                "No se ha seleccionado ningún cliente para proceder con la eliminación.",
                "Aviso",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        // Mensaje de confirmación en tono culto
        MessageBoxResult resultado = MessageBox.Show(
            $"¿Está usted seguro de que desea eliminar al distinguido cliente " +
            $"{clienteSeleccionado.Nombre} {clienteSeleccionado.Apellidos}? " +
            "Esta acción será definitiva y no podrá ser revertida.",
            "Confirmación de eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (resultado == MessageBoxResult.Yes)
        {
            // Eliminamos al cliente de la lista principal
            misClientes.Remove(clienteSeleccionado);

            // Actualizamos la lista visible de clientes
            ActualizarClientes();

            // Limpiamos la ficha del cliente
            FichaClienteBorder.Visibility = Visibility.Collapsed;
            clienteSeleccionado = null;

            
        }
    }
    private void BtnInformacion_Click(object sender, RoutedEventArgs e)
    {
        // Obtener el plato seleccionado
        if (sender is not Button btn || btn.DataContext is not Plato platoSeleccionado)
        {
            MessageBox.Show("No se pudo cargar el plato seleccionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Abrir ventana de edición
        EditarPlato ventana = new EditarPlato(platoSeleccionado);
        bool? resultado = ventana.ShowDialog();

        if (resultado != true) return;

        // Si se eliminó el plato
        if (ventana.PlatoEliminado != null)
        {
            var eliminado = ventana.PlatoEliminado;

            // 1️⃣ Eliminar del listado principal
            listadoPlatos.Remove(eliminado);

            // 2️⃣ Eliminar del pedido actual
            var enPedidoActual = productosActuales.FirstOrDefault(p => p.Imagen == eliminado.Imagen);
            if (enPedidoActual != null)
            {
                productosActuales.Remove(enPedidoActual);
                ActualizarTotal();
            }

            // 3️⃣ Eliminar de todos los pedidos existentes
            foreach (var p in pedidos.ToList())
            {
                var platosAEliminar = p.Platos.Where(pl => pl.Imagen == eliminado.Imagen).ToList();
                foreach (var pl in platosAEliminar)
                    p.Platos.Remove(pl);

                // Si un pedido queda sin platos, eliminarlo
                if (!p.Platos.Any())
                {
                    pedidos.Remove(p);
                    // También remover de la UI según estado
                    switch (p.Estado)
                    {
                        case 1: listaPendientesPago.Children.Clear(); break;
                        case 2: listaEnElaboracion.Children.Clear(); break;
                        case 3: listaListos.Children.Clear(); break;
                        case 4: listaHistorial.Children.Clear(); break;
                    }
                }
            }

            // Refrescar vista de UI
            PlatosViewSource.View.Refresh();
            FiltrarYBuscarPedidos();
            return;
        }

        // Si se editó el plato
        var edited = platoSeleccionado;

        // 1️⃣ Actualizar pedido actual
        foreach (var p in productosActuales.Where(p => p.Imagen == edited.Imagen))
        {
            p.Nombre = edited.Nombre;
            p.Precio = edited.Precio;
            p.Categoria = edited.Categoria;
            p.Subcategoria = edited.Subcategoria;
            p.Ingredientes = edited.Ingredientes;
            p.Alergenos = edited.Alergenos;
        }

        // 2️⃣ Actualizar todos los pedidos existentes
        foreach (var pedido in pedidos)
        {
            foreach (var pl in pedido.Platos.Where(pl => pl.Imagen == edited.Imagen))
            {
                pl.Nombre = edited.Nombre;
                pl.Precio = edited.Precio;
                pl.Categoria = edited.Categoria;
                pl.Subcategoria = edited.Subcategoria;
                pl.Ingredientes = edited.Ingredientes;
                pl.Alergenos = edited.Alergenos;
            }
        }

        // Refrescar UI
        PlatosViewSource.View.Refresh();
        FiltrarYBuscarPedidos();
        ActualizarTotal();
        CollectionViewSource.GetDefaultView(listaProductos.ItemsSource).Refresh();
    }

    private void MostrarSegundaCategoria(object sender, RoutedEventArgs e)
    {
        if (btnPrimeros.IsChecked == true || btnSegundos.IsChecked == true)
            FilaSegundaCategoria.Visibility = Visibility.Visible;
    }

    private void OcultarSegundaCategoriaSiNoHaySeleccion(object sender, RoutedEventArgs e)
    {
        if (btnPrimeros.IsChecked != true && btnSegundos.IsChecked != true)
            FilaSegundaCategoria.Visibility = Visibility.Collapsed;
    }

    private void BtnCrearPlato_Click(object sender, RoutedEventArgs e)
    {
        // Abrir la ventana de creación
        CrearPlato ventana = new CrearPlato();
        bool? resultado = ventana.ShowDialog();

        // Si el usuario creó un plato
        if (resultado == true && ventana.PlatoCreado != null)
        {
            // Añadir el plato a la lista principal
            listadoPlatos.Add(ventana.PlatoCreado);

            // Refrescar interfaz (productos)
            CollectionViewSource.GetDefaultView(PlatosListView.ItemsSource).Refresh();

        }
    }




}
