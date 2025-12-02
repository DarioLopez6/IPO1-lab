using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_IPO1
{
    public enum FORMAPAGO { TARGETA, EFECTIVO, BIZUM }
    public class Cliente
    {
        public string Imagen { get; set; } = "imagenes/perfil.png";
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public List<String> Direccion { get; set; }
        public List<String> Telefono { get; set; }
        public List<String> eMail { get; set; }
        public List<String> Alergias { get; set; }
        public List<String> Intolerancias { get; set; }
        public List<Pedido> Historial { get; set; } = new List<Pedido>();
        public FORMAPAGO pago { get; set; }
        public int puntosAcumulados { get; set; }
        public int puntosCangeados { get; set; }
        public Cliente(string imagen, int iD, string nombre, string apellidos, List<String> direccion, List<String> telefono, List<String> eMail, List<String> alergias, List<String> intolerancias, FORMAPAGO pago, int puntAcu, int puntCang)
        {
            Imagen = imagen;
            ID = iD;
            Nombre = nombre;
            Apellidos = apellidos;
            Direccion = direccion;
            Telefono = telefono;
            this.eMail = eMail;
            Alergias = alergias;
            Intolerancias = intolerancias;
            this.pago = pago;
            Historial = null;
            puntosAcumulados = puntAcu;
            puntosCangeados = puntCang;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public string getImagenNormal()
        {
            if (Imagen == "")
                return "imagenes/perfil.png";
            else
                return Imagen;
        }
    }
}
