using System.Collections.Generic;
using System.Windows;
using SmartVAT.Models;

namespace SmartVAT.WPF
{
    public partial class ListaFacturiWindow : Window
    {
        public ListaFacturiWindow(CerereRecuperareGrup grup)
        {
            InitializeComponent();
            tbTitle.Text = $"Facturi Extrase pentru: {grup.Tara} (Total: {grup.Facturi.Count})";
            dgFacturi.ItemsSource = grup.Facturi;
        }
    }
}
