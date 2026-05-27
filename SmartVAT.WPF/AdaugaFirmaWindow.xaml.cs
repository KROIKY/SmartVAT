using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SmartVAT.Models;
using SmartVAT.Data;

namespace SmartVAT.WPF
{
    public partial class AdaugaFirmaWindow : Window
    {
        private IAdministratorFirme _adminFirme;
        private Brush _defaultLabelForeground = Brushes.DimGray;
        private Brush _errorLabelForeground = Brushes.Red;

        public FirmaRO FirmaAdaugata { get; private set; }
        
        // Constructor optional pentru cand editam
        public AdaugaFirmaWindow(FirmaRO firmaDeEditat = null)
        {
            InitializeComponent();
            _adminFirme = FabricaDeStocare.ObtineAdministratorFirme();
            
            if (firmaDeEditat != null)
            {
                tbTitlu.Text = "Modificare Firmă";
                btnAdauga.Content = "Actualizează";
                IncarcaDateFirma(firmaDeEditat);
            }
        }
        
        private void IncarcaDateFirma(FirmaRO f)
        {
            tbDenumire.Text = f.Denumire;
            tbCUI.Text = f.CUI;
            tbDomiciliu.Text = f.DomiciliuFiscal;
            tbContIBAN.Text = f.ContIBAN;
            tbCodBIC_SWIFT.Text = f.CodBIC_SWIFT;
            tbTelefonContabil.Text = f.TelefonContabil;
            tbEmailContabil.Text = f.EmailContabil;
            tbCUI.IsEnabled = false; // CUI-ul e PK, nu-l schimbam
        }

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            ResetVisualErori();

            string denumire = tbDenumire.Text.Trim();
            string cui = tbCUI.Text.Trim();
            string domiciliu = tbDomiciliu.Text.Trim();
            string iban = tbContIBAN.Text.Trim();
            string swift = tbCodBIC_SWIFT.Text.Trim();
            string tel = tbTelefonContabil.Text.Trim();
            string email = tbEmailContabil.Text.Trim();
            
            string caen = rbCaen4941.IsChecked == true ? "4941" : "";

            CodEroareFirma rezultatValidare = ValideazaDateFirma(denumire, cui, domiciliu, caen, iban, swift, tel, email);
            if (rezultatValidare != CodEroareFirma.SUCCESS)
            {
                EvidentiazaEroare(rezultatValidare);
                return; 
            }

            TipCompanie tipSelectat = TipCompanie.SRL; // Deocamdata doar SRL e hardcodat
            
            FirmaAdaugata = new FirmaRO
            {
                Denumire = denumire,
                CUI = cui,
                DomiciliuFiscal = domiciliu,
                CodCAEN = caen,
                ContIBAN = iban,
                CodBIC_SWIFT = swift,
                TelefonContabil = tel,
                EmailContabil = email,
                Tip = tipSelectat,
                DataAdaugare = DateTime.Now,
                DataActualizare = DateTime.Now
            };

            DialogResult = true; 
            this.Close();
        }

        private CodEroareFirma ValideazaDateFirma(string denumire, string cui, string dom, string caen, string iban, string swift, string tel, string email)
        {
            if (string.IsNullOrEmpty(denumire)) return CodEroareFirma.DENUMIRE_GOALA;
            if (denumire.Length > ConstanteFiscale.LungimeMaximaDenumire) return CodEroareFirma.DENUMIRE_PREA_LUNGA;

            if (string.IsNullOrEmpty(cui)) return CodEroareFirma.CUI_GOL;
            if (cui.Length > ConstanteFiscale.LungimeMaximaCUI) return CodEroareFirma.CUI_INVALID;

            if (string.IsNullOrEmpty(dom)) return CodEroareFirma.DOMICILIU_GOL;
            if (dom.Length > ConstanteFiscale.LungimeMaximaDomiciliu) return CodEroareFirma.DOMICILIU_PREA_LUNG;

            if (string.IsNullOrEmpty(caen)) return CodEroareFirma.CAEN_GOL;
            if (caen.Length != ConstanteFiscale.LungimeMaximaCAEN) return CodEroareFirma.CAEN_INVALID;

            if (string.IsNullOrEmpty(iban)) return CodEroareFirma.IBAN_GOL;
            if (iban.Length > ConstanteFiscale.LungimeMaximaIBAN) return CodEroareFirma.IBAN_INVALID;

            if (string.IsNullOrEmpty(swift)) return CodEroareFirma.SWIFT_GOL;
            if (swift.Length > ConstanteFiscale.LungimeMaximaSwift) return CodEroareFirma.SWIFT_INVALID;

            return CodEroareFirma.SUCCESS;
        }

        private void EvidentiazaEroare(CodEroareFirma eroare)
        {
            lbEroare.Visibility = Visibility.Visible;

            switch (eroare)
            {
                case CodEroareFirma.DENUMIRE_GOALA:
                    lbDenumire.Foreground = _errorLabelForeground;
                    lbEroare.Text = "Denumirea firmei trebuie completată!";
                    break;
                case CodEroareFirma.DENUMIRE_PREA_LUNGA:
                    lbDenumire.Foreground = _errorLabelForeground;
                    lbEroare.Text = $"Denumirea depășește maximul de {ConstanteFiscale.LungimeMaximaDenumire} caractere.";
                    break;
                case CodEroareFirma.CUI_GOL:
                    lbCUI.Foreground = _errorLabelForeground;
                    lbEroare.Text = "Codul Unic de Înregistrare trebuie completat!";
                    break;
                case CodEroareFirma.CUI_INVALID:
                    lbCUI.Foreground = _errorLabelForeground;
                    lbEroare.Text = $"CUI-ul trebuie să conțină maxim {ConstanteFiscale.LungimeMaximaCUI} caractere.";
                    break;
                case CodEroareFirma.DOMICILIU_GOL:
                    lbDomiciliu.Foreground = _errorLabelForeground;
                    lbEroare.Text = "Domiciliul fiscal este obligatoriu.";
                    break;
                case CodEroareFirma.DOMICILIU_PREA_LUNG:
                    lbDomiciliu.Foreground = _errorLabelForeground;
                    lbEroare.Text = $"Domiciliul este prea lung (max {ConstanteFiscale.LungimeMaximaDomiciliu}).";
                    break;
                case CodEroareFirma.CAEN_GOL:
                case CodEroareFirma.CAEN_INVALID:
                    lbCodCAEN.Foreground = _errorLabelForeground;
                    lbEroare.Text = "Codul CAEN trebuie să conțină exact 4 cifre.";
                    break;
                case CodEroareFirma.IBAN_GOL:
                case CodEroareFirma.IBAN_INVALID:
                    lbContIBAN.Foreground = _errorLabelForeground;
                    lbEroare.Text = "Contul IBAN este invalid sau necompletat.";
                    break;
                case CodEroareFirma.SWIFT_GOL:
                case CodEroareFirma.SWIFT_INVALID:
                    lbCodBIC_SWIFT.Foreground = _errorLabelForeground;
                    lbEroare.Text = "Codul SWIFT/BIC este invalid.";
                    break;
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (tbCUI.IsEnabled) // doar daca nu suntem in mod editare
            {
                tbCUI.Text = string.Empty;
            }
            tbDenumire.Text = string.Empty;
            tbDomiciliu.Text = string.Empty;
            tbContIBAN.Text = string.Empty;
            tbCodBIC_SWIFT.Text = string.Empty;
            tbTelefonContabil.Text = string.Empty;
            tbEmailContabil.Text = string.Empty;
            rbCaen4941.IsChecked = true;
            cmbTipCompanie.SelectedIndex = 0;

            ResetVisualErori();
        }

        private void ResetVisualErori()
        {
            lbEroare.Visibility = Visibility.Hidden;
            lbEroare.Text = string.Empty;

            lbDenumire.Foreground = _defaultLabelForeground;
            lbCUI.Foreground = _defaultLabelForeground;
            lbDomiciliu.Foreground = _defaultLabelForeground;
            lbCodCAEN.Foreground = _defaultLabelForeground;
            lbContIBAN.Foreground = _defaultLabelForeground;
            lbCodBIC_SWIFT.Foreground = _defaultLabelForeground;
            lbTelefonContabil.Foreground = _defaultLabelForeground;
            lbEmailContabil.Foreground = _defaultLabelForeground;
        }
    }
}
