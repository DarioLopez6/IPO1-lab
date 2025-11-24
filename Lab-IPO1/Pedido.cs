using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_IPO1
{
    class Pedido
    {
        public int Id { get; set; }
        public bool Local { get; set; }           // true = en local, false = telefono/domicilio
        public string Hora { get; set; }
        public string Domicilio { get; set; }
        public string Cliente { get; set; }
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public double Total { get; set; }
        public int Pago { get; set; }             // 1=tarjeta,2=efectivo,3=bizum
        public int Estado { get; set; }           // 1..4
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
        public override string ToString()
        {
            return $"ID:{Id} - {Cliente} ({(Local ? "Local" : "Teléfono")}) {Total:0.00}€";
        }

    }
}
