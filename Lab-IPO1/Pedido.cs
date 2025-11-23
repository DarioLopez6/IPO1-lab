using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_IPO1
{
    class Pedido
    {
        // Tipo de pedido: true = local, false = a domicilio
        public bool Local { get; set; }

        // Hora de recogida o entrega
        public string Hora { get; set; }

        // Dirección de entrega (si no es local)
        public string Domicilio { get; set; }

        // Identificador o nombre del cliente
        public string Cliente { get; set; }

        // Lista de productos del pedido
        public List<Producto> Productos { get; set; } = new List<Producto>();

        // Total del pedido
        public double Total { get; set; }

        // Forma de pago: 1=tarjeta, 2=efectivo, 3=bizum
        public int Pago { get; set; }

        // Estado del pedido: 1=pendiente de pago, 2=en elaboracion, 3=listo para entregar, 4=historial
        public int Estado { get; set; }

        // Puntos o cupones canjeados en este pedido
        public string Puntos { get; set; }

        // Constructor opcional
        public Pedido(bool local, string hora, string domicilio, string cliente, List<Producto> productos, double total, int pago, int estado, string puntos)
        {
            Local = local;
            Hora = hora;
            Domicilio = domicilio;
            Cliente = cliente;
            Productos = new List<Producto>();
            Total = total;
            Pago = pago;
            Estado = estado;
            Puntos = puntos;
        }

    }
}
