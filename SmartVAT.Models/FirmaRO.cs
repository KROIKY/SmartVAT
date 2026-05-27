using System;
using System.ComponentModel;

namespace SmartVAT.Models
{
    public enum TipCompanie
    {
        SRL,
        SA,
        PFA
    }

    public class FirmaRO : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _displayId;
        public int DisplayId { get => _displayId; set { _displayId = value; OnPropertyChanged(nameof(DisplayId)); } }
        
        private string _denumire = string.Empty;
        public string Denumire { get => _denumire; set { _denumire = value; OnPropertyChanged(nameof(Denumire)); } }
        
        private string _cui = string.Empty;
        public string CUI { get => _cui; set { _cui = value; OnPropertyChanged(nameof(CUI)); } }
        
        private string _domiciliuFiscal = string.Empty;
        public string DomiciliuFiscal { get => _domiciliuFiscal; set { _domiciliuFiscal = value; OnPropertyChanged(nameof(DomiciliuFiscal)); } }
        
        private string _codCAEN = string.Empty;
        public string CodCAEN { get => _codCAEN; set { _codCAEN = value; OnPropertyChanged(nameof(CodCAEN)); } }

        private string _telefonContabil = string.Empty;
        public string TelefonContabil { get => _telefonContabil; set { _telefonContabil = value; OnPropertyChanged(nameof(TelefonContabil)); } }
        
        private string _emailContabil = string.Empty;
        public string EmailContabil { get => _emailContabil; set { _emailContabil = value; OnPropertyChanged(nameof(EmailContabil)); } }

        private string _contIBAN = string.Empty;
        public string ContIBAN { get => _contIBAN; set { _contIBAN = value; OnPropertyChanged(nameof(ContIBAN)); } }
        
        private string _codBICSWIFT = string.Empty;
        public string CodBIC_SWIFT { get => _codBICSWIFT; set { _codBICSWIFT = value; OnPropertyChanged(nameof(CodBIC_SWIFT)); } }

        // --- PROPRIETĂȚI NOI PENTRU CERINȚE (LAB) ---
        private TipCompanie _tip = TipCompanie.SRL;
        public TipCompanie Tip { get => _tip; set { _tip = value; OnPropertyChanged(nameof(Tip)); } }

        // Auto-completare date
        public DateTime DataAdaugare { get; set; }
        public DateTime DataActualizare { get; set; }

        public string ObtineTitularCont() => Denumire;

        public override string ToString()
        {
            return $"[{CUI}] {Denumire} - {Tip}";
        }

        // --- VALIDARE DATE (IDataErrorInfo) ---
        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case nameof(Denumire):
                        if (string.IsNullOrWhiteSpace(Denumire) || Denumire.Length < 3)
                            result = "Denumirea trebuie să aibă minim 3 caractere.";
                        break;
                    case nameof(CUI):
                        if (string.IsNullOrWhiteSpace(CUI) || CUI.Length < 6)
                            result = "CUI invalid (minim 6 caractere).";
                        break;
                }
                return result;
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                return string.IsNullOrEmpty(this[nameof(Denumire)]) &&
                       string.IsNullOrEmpty(this[nameof(CUI)]);
            }
        }
    }
}
