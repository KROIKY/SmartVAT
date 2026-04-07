using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using SmartVAT.Models;
using SmartVAT.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartVAT.WPF
{
    public partial class MainWindow : Window
    {
        // Folosim ObservableCollection pentru ca UI-ul să se actualizeze în timp real când adăugăm firme
        private ObservableCollection<FirmaRO> FirmeList { get; set; }
        private IAdministratorFirme _adminFirme;

        public MainWindow()
        {
            InitializeComponent();

            // Instantiem managerul din backend (strat Data)
            _adminFirme = FabricaDeStocare.ObtineAdministratorFirme();

            // Citim firmele deja salvate
            FirmeList = new ObservableCollection<FirmaRO>(_adminFirme.GetAll());

            // Legăm lista de componenta vizuală
            lbFirme.ItemsSource = FirmeList;
        }

        private void lbFirme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbFirme.SelectedItem is FirmaRO firmaDeselectata)
            {
                // Facem vizibil panoul abia la selectare
                gridDetalii.Visibility = Visibility.Visible;

                // Populăm "card-ul" cu datele din obiectul FirmaRO
                txtNume.Text = firmaDeselectata.Denumire;
                txtCUI.Text = firmaDeselectata.CUI;
                txtAdresa.Text = firmaDeselectata.DomiciliuFiscal;
                txtCAEN.Text = firmaDeselectata.CodCAEN;

                txtIBAN.Text = string.IsNullOrEmpty(firmaDeselectata.ContIBAN) ? "Nedeclarat" : firmaDeselectata.ContIBAN;
                txtSwift.Text = string.IsNullOrEmpty(firmaDeselectata.CodBIC_SWIFT) ? "-" : firmaDeselectata.CodBIC_SWIFT;

                txtTelContabil.Text = string.IsNullOrEmpty(firmaDeselectata.TelefonContabil) ? "Nu există" : firmaDeselectata.TelefonContabil;
                txtEmailContabil.Text = string.IsNullOrEmpty(firmaDeselectata.EmailContabil) ? "Nu există" : firmaDeselectata.EmailContabil;
            }
            else
            {
                gridDetalii.Visibility = Visibility.Hidden;
            }
        }

        private void btnAddFirma_Click(object sender, RoutedEventArgs e)
        {
            // Cream o firmă de exemplu
            var firmaNoua = new FirmaRO
            {
                Denumire = "SC SMART VAT SRL ",
                CUI = "RO" + new Random().Next(10000000, 99999999),
                DomiciliuFiscal = "Strada 1 Decembrie Nr. " + new Random().Next(1, 100) + ", Suceava",
                CodCAEN = "4941",
                ContIBAN = "RO20INGB000000000000" + new Random().Next(1000, 9999),
                CodBIC_SWIFT = "INGBROBU",
                TelefonContabil = "07" + new Random().Next(10000000, 99999999),
                EmailContabil = "contabilitate@smartvat.ro"
            };

            // Adăugăm în backend
            _adminFirme.AdaugaFirma(firmaNoua);

            // Adăugăm în colecția pentru UI
            FirmeList.Add(firmaNoua);

            // Selectăm ultima firmă adăugată auto
            lbFirme.SelectedItem = firmaNoua;
        }
    }
}