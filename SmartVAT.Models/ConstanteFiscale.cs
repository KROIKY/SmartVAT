namespace SmartVAT.Models
{
    public class TaraUE
    {
        public string Cod { get; set; } = string.Empty;
        public string Nume { get; set; } = string.Empty;
        public string Limba { get; set; } = string.Empty;
        public string Moneda { get; set; } = string.Empty;
    }

    /// <summary>
    /// Stocheaza valorile care nu se pot modifica (Constante - compile time).
    /// </summary>
    public static class ConstanteFiscale
    {
        // 50 Euro limitare standard de recuperare
        public const decimal LimitaMinimaRecuperareTVA = 50.0m;
        
        // Versiunea Formularului suportata
        public const string VersiuneFormular = "v1.02";

        // Tabelul complet oficial cu Tarile de Rambursare, Limbile si Monedele XSD ANAF
        public static readonly List<TaraUE> TariRambursare = new List<TaraUE>
        {
            new TaraUE { Cod = "AT", Nume = "Austria", Limba = "DE", Moneda = "EUR" },
            new TaraUE { Cod = "BE", Nume = "Belgia", Limba = "FR", Moneda = "EUR" },
            new TaraUE { Cod = "BG", Nume = "Bulgaria", Limba = "BG", Moneda = "BGN" },
            new TaraUE { Cod = "CY", Nume = "Cipru", Limba = "EL", Moneda = "EUR" },
            new TaraUE { Cod = "CZ", Nume = "Republica Ceha", Limba = "CS", Moneda = "CZK" },
            new TaraUE { Cod = "HR", Nume = "Croatia", Limba = "HR", Moneda = "EUR" },
            new TaraUE { Cod = "DK", Nume = "Danemarca", Limba = "DA", Moneda = "DKK" },
            new TaraUE { Cod = "EE", Nume = "Estonia", Limba = "ET", Moneda = "EUR" },
            new TaraUE { Cod = "FI", Nume = "Finlanda", Limba = "FI", Moneda = "EUR" },
            new TaraUE { Cod = "FR", Nume = "Franta", Limba = "FR", Moneda = "EUR" },
            new TaraUE { Cod = "DE", Nume = "Germania", Limba = "DE", Moneda = "EUR" },
            new TaraUE { Cod = "EL", Nume = "Grecia", Limba = "EL", Moneda = "EUR" },
            new TaraUE { Cod = "IE", Nume = "Irlanda", Limba = "EN", Moneda = "EUR" },
            new TaraUE { Cod = "IT", Nume = "Italia", Limba = "IT", Moneda = "EUR" },
            new TaraUE { Cod = "LV", Nume = "Letonia", Limba = "LV", Moneda = "EUR" },
            new TaraUE { Cod = "LT", Nume = "Lituania", Limba = "LT", Moneda = "EUR" },
            new TaraUE { Cod = "LU", Nume = "Luxemburg", Limba = "FR", Moneda = "EUR" },
            new TaraUE { Cod = "MT", Nume = "Malta", Limba = "MT", Moneda = "EUR" },
            new TaraUE { Cod = "NL", Nume = "Olanda", Limba = "NL", Moneda = "EUR" },
            new TaraUE { Cod = "PL", Nume = "Polonia", Limba = "PL", Moneda = "PLN" },
            new TaraUE { Cod = "PT", Nume = "Portugalia", Limba = "PT", Moneda = "EUR" },
            new TaraUE { Cod = "SK", Nume = "Slovacia", Limba = "SK", Moneda = "EUR" },
            new TaraUE { Cod = "SI", Nume = "Slovenia", Limba = "SL", Moneda = "EUR" },
            new TaraUE { Cod = "ES", Nume = "Spania", Limba = "ES", Moneda = "EUR" },
            new TaraUE { Cod = "SE", Nume = "Suedia", Limba = "SV", Moneda = "SEK" },
            new TaraUE { Cod = "HU", Nume = "Ungaria", Limba = "HU", Moneda = "HUF" }
        };
    }
}
