using System;

namespace SmartVAT.Models
{
    public class FurnizorUE
    {
        // Aceasta variabila va prinde "AT" de ex. din "ATU389..""
        public string PrefixTaraExtras { get; private set; } = string.Empty;
        
        // Aceasta prinde corpul CUI fara primele 2 cifre
        public string CorpCuloareTVA { get; private set; } = string.Empty;

        // Setter inteligent - scindare 
        private string _cuiComplet;
        public string CuloareTVA_CIF
        {
            get => _cuiComplet;
            set
            {
                _cuiComplet = value;
                if (!string.IsNullOrEmpty(value) && value.Length > 2)
                {
                    PrefixTaraExtras = value.Substring(0, 2).ToUpper();
                    CorpCuloareTVA = value.Substring(2);
                }
            }
        }

        public string Denumire { get; set; } = string.Empty;
        public string Adresa { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        // Tara a fost eliminata fiindca e redundanta, se deduce direct din D318_Header.Tara

        public override string ToString()
        {
            return $"[{PrefixTaraExtras}] {CorpCuloareTVA} | {Denumire} - {Adresa}";
        }
    }
}
