using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Lab_IPO1
{
    public class Plato : INotifyPropertyChanged
    {
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public string Nombre { get; set; }
        public string Ingredientes { get; set; }
        public int Precio { get; set; }
        public string Alergenos { get; set; }
        public Uri Imagen { get; set; }

        private int cantidad;
        public int Cantidad
        {
            get => cantidad;
            set
            {
                if (cantidad != value)
                {
                    cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                }
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}