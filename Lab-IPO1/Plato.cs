using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_IPO1
{
    class Plato
    {
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public string Nombre { get; set; }
        public string Ingredientes { get; set; }
        public int Precio { get; set; }
        public string Alergenos { get; set; }
        public Uri Imagen { get; set; }
        public int Cantidad { get; set; }

        public Plato() { }

        public Plato(string categoria, string subcategoria, string nombre, string ingredientes, int precio, string alergenos, Uri imagen, int cantidad)
        {
            Categoria = categoria;
            Subcategoria = subcategoria;
            Nombre = nombre;
            Ingredientes = ingredientes;
            Precio = precio;
            Alergenos = alergenos;
            Imagen = imagen;
            Cantidad = cantidad;
        }
    }

}
