using System.Collections.Generic;

namespace SmartVAT.Models
{
    public class CerereRecuperareGrup
    {
        public string Tara { get; set; } = string.Empty;
        public int NumarFacturi { get; set; }
        public decimal TotalTvaDeductibil { get; set; }
        public List<FacturaAchizitie> Facturi { get; set; } = new List<FacturaAchizitie>();
    }
}
