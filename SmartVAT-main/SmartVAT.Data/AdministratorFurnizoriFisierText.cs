using System.Collections.Generic;
using System.IO;
using System.Linq;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class AdministratorFurnizoriFisierText : IAdministratorFurnizori
    {
        private static readonly string Fisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "furnizori.txt");

        private List<FurnizorUE> IncarcaToate()
        {
            var rezultate = new List<FurnizorUE>();
            
            if (!File.Exists(Fisier)) return rezultate;

            using (StreamReader sr = new StreamReader(Fisier))
            {
                string linie;
                while ((linie = sr.ReadLine()) != null)
                {
                    // CUIComplet | Denumire | Adresa | Telefon
                    var parti = linie.Split('|');
                    if (parti.Length >= 3)
                    {
                        var furnizor = new FurnizorUE
                        {
                            CuloareTVA_CIF = parti[0],
                            Denumire = parti[1],
                            Adresa = parti[2],
                            Telefon = parti.Length > 3 ? parti[3] : ""
                        };
                        rezultate.Add(furnizor);
                    }
                }
            }
            return rezultate;
        }

        public void AdaugaFurnizor(FurnizorUE furnizor)
        {
            using (StreamWriter sw = new StreamWriter(Fisier, true))
            {
                string linie = $"{furnizor.CuloareTVA_CIF}|{furnizor.Denumire}|{furnizor.Adresa}|{furnizor.Telefon}";
                sw.WriteLine(linie);
            }
        }

        public List<FurnizorUE> GetAll()
        {
            return IncarcaToate();
        }

        public FurnizorUE CautaDupaCUI(string cuiVAT)
        {
            return IncarcaToate().FirstOrDefault(f => f.CuloareTVA_CIF == cuiVAT);
        }
    }
}
