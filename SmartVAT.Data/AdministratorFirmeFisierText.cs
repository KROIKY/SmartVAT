using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class AdministratorFirmeFisierText : IAdministratorFirme
    {
        private static readonly string Fisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firme.txt");

        private List<FirmaRO> IncarcaToate()
        {
            var rezultate = new List<FirmaRO>();
            
            if (!File.Exists(Fisier)) return rezultate;

            using (StreamReader sr = new StreamReader(Fisier))
            {
                string linie;
                while ((linie = sr.ReadLine()) != null)
                {
                    // Denumire | CUI | DomiciliuFiscal | CAEN | TelefonContabil | EmailContabil | IBAN | BIC
                    var parti = linie.Split('|');
                    if (parti.Length >= 8)
                    {
                        var firma = new FirmaRO
                        {
                            Denumire = parti[0],
                            CUI = parti[1],
                            DomiciliuFiscal = parti[2],
                            CodCAEN = parti[3],
                            TelefonContabil = parti[4],
                            EmailContabil = parti[5],
                            ContIBAN = parti[6],
                            CodBIC_SWIFT = parti[7]
                        };
                        rezultate.Add(firma);
                    }
                }
            }
            return rezultate;
        }

        public void AdaugaFirma(FirmaRO firma)
        {
            using (StreamWriter sw = new StreamWriter(Fisier, true))
            {
                string linie = $"{firma.Denumire}|" +
                               $"{firma.CUI}|" +
                               $"{firma.DomiciliuFiscal}|" +
                               $"{firma.CodCAEN}|" +
                               $"{firma.TelefonContabil}|" +
                               $"{firma.EmailContabil}|" +
                               $"{firma.ContIBAN}|" +
                               $"{firma.CodBIC_SWIFT}";
                sw.WriteLine(linie);
            }
        }

        public List<FirmaRO> GetAll()
        {
            return IncarcaToate();
        }

        public FirmaRO CautaDupaCUI(string cui)
        {
            return IncarcaToate().FirstOrDefault(f => f.CUI == cui);
        }
    }
}
