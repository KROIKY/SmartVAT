using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartVAT.Models
{
    public class FormularD318
    {
        public string NumeReferinta { get; set; } = $"Dosar D318 {DateTime.Now:yyyy-MM-dd}";

        public int An { get; private set; }
        public int LunaInceput { get; private set; }
        public int LunaSfarsit { get; private set; }
        
        // Logica Auto-calculare An&Perioada conform Cerintei.
        public void AutoCalculeazaPerioada()
        {
            if (Achizitii.Count > 0)
            {
                var minDate = Achizitii.Min(f => f.DataFactura);
                var maxDate = Achizitii.Max(f => f.DataFactura);
                An = maxDate.Year;
                LunaInceput = minDate.Month;
                LunaSfarsit = maxDate.Month;

                // Fortam o perioada minimade 3 lu
                // Diferenta dintre LunaSfarsit si LunaInceput trebuie sa fie >= 2 (Ex: Luna 1 la 3 inseamna 3-1=2).
                while (LunaSfarsit - LunaInceput < 2)
                {
                    if (LunaSfarsit < 12)
                    {
                        LunaSfarsit++; // Extindem inainte daca avem loc pana la Decembrie
                    }
                    else if (LunaInceput > 1)
                    {
                        LunaInceput--; // Daca e Decembrie (12) si factura pe o singura luna, extindem inapoi (10-12)
                    }
                }
            }
        }

        public bool CerereInitiala { get; set; } = true;
        
        // Setare Automata Smart-Field
        public string StatMembruRambursare { get; private set; } = string.Empty;
        public string LimbaOficiala { get; private set; } = string.Empty;
        public string Moneda { get; private set; } = string.Empty;

        public void SeteazaStatMembru(string codTara)
        {
            var tara = ConstanteFiscale.TariRambursare.FirstOrDefault(t => t.Cod == codTara.ToUpper());
            if (tara != null)
            {
                StatMembruRambursare = $"{tara.Cod} - {tara.Nume}";
                LimbaOficiala = $"{tara.Limba}";
                Moneda = tara.Moneda;
            }
            else
            {
                throw new ArgumentException($"Țara cu codul introdus ('{codTara}') nu este eligibilă sau nu există în nomenclatorul celor 26 de țări D318.");
            }
        }

        public FirmaRO Solicitant { get; set; } = new FirmaRO();

        public List<FacturaAchizitie> Achizitii { get; set; } = new List<FacturaAchizitie>();

        public decimal GetTotalSumaSolicitataRambursare()
        {
            decimal total = 0;
            foreach (var fact in Achizitii)
            {
                total += fact.TvaDeductibila; // care la randul ei e acum legata automat de TVAGeneral
            }
            return total;
        }
    }
}
