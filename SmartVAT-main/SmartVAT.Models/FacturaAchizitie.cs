using System;
using System.Collections.Generic;

namespace SmartVAT.Models
{
    public class FacturaAchizitie
    {
        public bool EsteFacturaSimplificata { get; set; } = false;
        public string NumarFactura { get; set; } = string.Empty;
        public DateTime DataFactura { get; set; }
        
        public List<CheltuialaFactura> NaturaBunurilor { get; set; } = new List<CheltuialaFactura>();

        // Total Factură
        public decimal BazaImpozabila { get; set; }
        public decimal ValoareTVA { get; set; }
        
        // Câmp SMART - Deductibilitatea este dictata automat de valoare intrinseca TVA!
        public decimal TvaDeductibila => ValoareTVA;

        public FurnizorUE FurnizorTemplate { get; set; } = new FurnizorUE();

        public override string ToString()
        {
            var detalii = $">> FACTURA NR: {NumarFactura} | DATA: {DataFactura:dd.MM.yyyy}\n";
            detalii += $"   Baza: {BazaImpozabila} EUR | TVA: {ValoareTVA} EUR (Deductibil: {TvaDeductibila} EUR)\n";
            detalii += $"   Furnizor: {FurnizorTemplate.Denumire} [{FurnizorTemplate.CorpCuloareTVA}]\n";
            return detalii;
        }
    }
}
