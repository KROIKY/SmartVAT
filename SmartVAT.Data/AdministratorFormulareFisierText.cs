using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class AdministratorFormulareFisierText : IAdministratorFormulare
    {
        private static readonly string Fisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "formulare_salvate.txt");

        private List<FormularD318> IncarcaToate()
        {
            var rezultate = new List<FormularD318>();

            if (!File.Exists(Fisier)) return rezultate;

            using (StreamReader sr = new StreamReader(Fisier))
            {
                string linie;
                while ((linie = sr.ReadLine()) != null)
                {
                    // Evitam deserializarea daca linia e goala
                    if (string.IsNullOrWhiteSpace(linie)) continue;
                    
                    try
                    {
                        var formular = JsonSerializer.Deserialize<FormularD318>(linie);
                        if (formular != null)
                        {
                            rezultate.Add(formular);
                        }
                    }
                    catch
                    {
                        // In caz ca fisierul se corupe manual, ignoram linia compromisa.
                    }
                }
            }
            return rezultate;
        }

        public void SalveazaFormular(FormularD318 formular)
        {
            // true = append text la sfarsit
            using (StreamWriter sw = new StreamWriter(Fisier, true))
            {
                // Vom salva starea intr-un singur string JSON (ca text plat) conform cerintei "structura prin care obiectele reies magic din fisierul text".
                // Scriem fiecare Dosar/Declaratie ca o linie separata
                string linieText = JsonSerializer.Serialize(formular);
                sw.WriteLine(linieText);
            }
        }

        public List<FormularD318> GetToateFormularele()
        {
            return IncarcaToate();
        }

        public List<FormularD318> CautaFormulareDupaAn(int an)
        {
            return IncarcaToate().Where(f => f.An == an).ToList();
        }
    }
}
