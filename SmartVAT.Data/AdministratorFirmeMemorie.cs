using System;
using System.Collections.Generic;
using System.Linq;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class AdministratorFirmeMemorie : IAdministratorFirme
    {
        private readonly List<FirmaRO> _firme;

        public AdministratorFirmeMemorie()
        {
            _firme = new List<FirmaRO>();
        }

        public void AdaugaFirma(FirmaRO firma)
        {
            _firme.Add(firma);
        }

        public List<FirmaRO> GetAll()
        {
            return _firme.ToList();
        }

        public FirmaRO CautaDupaCUI(string cui)
        {
            return _firme.FirstOrDefault(f => f.CUI == cui);
        }

        public void ActualizeazaFirma(FirmaRO firma)
        {
            var old = CautaDupaCUI(firma.CUI);
            if (old != null)
            {
                _firme.Remove(old);
                _firme.Add(firma);
            }
        }
    }
}
