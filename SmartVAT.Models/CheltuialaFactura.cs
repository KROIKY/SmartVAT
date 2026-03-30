using System;

namespace SmartVAT.Models
{
    /// <summary>
    /// Reprezinta un rand din tabelul Natura bunurilor (ex: Cod 1 / Sub-cod 1.1.2)
    /// </summary>
    public class CheltuialaFactura
    {
        public int Cod { get; set; } // Gen 1 (Combustibil), 4 (Taxe), 10 (Diverse)
        public string SubCod { get; set; } = string.Empty;
        public string Descriere { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Cod: {Cod} | SubCod: {SubCod} | Descriere: {Descriere}";
        }
    }
}
