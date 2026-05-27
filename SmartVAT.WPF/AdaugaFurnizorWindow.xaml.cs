using System;
using System.Windows;
using System.Windows.Media;
using SmartVAT.Models;
using SmartVAT.Data;

namespace SmartVAT.WPF
{
    public partial class AdaugaFurnizorWindow : Window
    {
        private IAdministratorFurnizori _adminFurnizori;
        private Brush _defaultLabelForeground = Brushes.DimGray;
        private Brush _errorLabelForeground = Brushes.Red;

        public FurnizorUE FurnizorAdaugat { get; private set; }

        public AdaugaFurnizorWindow()
        {
            InitializeComponent();
            _adminFurnizori = FabricaDeStocare.ObtineAdministratorFurnizori();
        }

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            ResetVisualErori();

            string cui = tbCUIFurnizor.Text.Trim();
            string denumire = tbDenumireFurnizor.Text.Trim();
            string adresa = tbAdresaFurnizor.Text.Trim();

            if (string.IsNullOrEmpty(cui))
            {
                EvidentiazaEroare(lbCUIFurnizor, "Codul TVA este obligatoriu pentru preluarea de pe facturi!");
                return;
            }

            if (string.IsNullOrEmpty(denumire))
            {
                EvidentiazaEroare(lbDenumireFurnizor, "Denumirea companiei este obligatorie!");
                return;
            }

            FurnizorAdaugat = new FurnizorUE
            {
                CuloareTVA_CIF = cui,
                Denumire = denumire,
                Adresa = adresa,
                Telefon = string.Empty // Fără telefon conform cerinței
            };

            _adminFurnizori.AdaugaFurnizor(FurnizorAdaugat);

            DialogResult = true;
            this.Close();
        }

        private void btnAnuleaza_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void EvidentiazaEroare(System.Windows.Controls.Label targetLabel, string mesaj)
        {
            lbEroare.Visibility = Visibility.Visible;
            lbEroare.Text = mesaj;
            targetLabel.Foreground = _errorLabelForeground;
        }

        private void ResetVisualErori()
        {
            lbEroare.Visibility = Visibility.Hidden;
            lbEroare.Text = string.Empty;

            lbCUIFurnizor.Foreground = _defaultLabelForeground;
            lbDenumireFurnizor.Foreground = _defaultLabelForeground;
            lbAdresaFurnizor.Foreground = _defaultLabelForeground;
        }
    }
}
