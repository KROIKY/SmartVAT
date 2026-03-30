using System;

namespace SmartVAT.Models
{
    public class FirmaRO
    {
        // Date Identificare Strict FIRMA
        public string Denumire { get; set; } = string.Empty;
        public string CUI { get; set; } = string.Empty;
        public string DomiciliuFiscal { get; set; } = string.Empty;
        public string CodCAEN { get; set; } = string.Empty;

        // Date Contact CONTABIL
        public string TelefonContabil { get; set; } = string.Empty;
        public string EmailContabil { get; set; } = string.Empty;

        // Informatii Financiare (OBLIGATORII LA CREAREA FIRMEI)
        public string ContIBAN { get; set; } = string.Empty;
        public string CodBIC_SWIFT { get; set; } = string.Empty;

        // Titularul Contului este dedus automat ca fiind insasi denumirea Firmei
        public string ObtineTitularCont() => Denumire;

        public override string ToString()
        {
            return $"[{CUI}] {Denumire} - CAEN: {CodCAEN} | Domiciliu: {DomiciliuFiscal} | Contact(Contabil): {TelefonContabil}";
        }
    }
}
