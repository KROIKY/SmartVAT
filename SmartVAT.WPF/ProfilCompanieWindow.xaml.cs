using System.Windows;
using SmartVAT.Models;

namespace SmartVAT.WPF
{
    public partial class ProfilCompanieWindow : Window
    {
        public ProfilCompanieWindow(FirmaRO firma)
        {
            InitializeComponent();
            IncarcaDateFirma(firma);
        }

        private void IncarcaDateFirma(FirmaRO f)
        {
            if (f == null) return;

            txtDenumire.Text = string.IsNullOrEmpty(f.Denumire) ? "-" : f.Denumire;
            txtCUI.Text = string.IsNullOrEmpty(f.CUI) ? "-" : f.CUI;
            txtDomiciliu.Text = string.IsNullOrEmpty(f.DomiciliuFiscal) ? "-" : f.DomiciliuFiscal;
            
            txtIBAN.Text = string.IsNullOrEmpty(f.ContIBAN) ? "-" : f.ContIBAN;
            txtSWIFT.Text = string.IsNullOrEmpty(f.CodBIC_SWIFT) ? "-" : f.CodBIC_SWIFT;
            txtCAEN.Text = string.IsNullOrEmpty(f.CodCAEN) ? "-" : f.CodCAEN;

            txtTelefon.Text = string.IsNullOrEmpty(f.TelefonContabil) ? "-" : f.TelefonContabil;
            txtEmail.Text = string.IsNullOrEmpty(f.EmailContabil) ? "-" : f.EmailContabil;
        }
    }
}
