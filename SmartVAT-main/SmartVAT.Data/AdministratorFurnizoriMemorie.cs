using System.Collections.Generic;
using System.Linq;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class AdministratorFurnizoriMemorie : IAdministratorFurnizori
    {
        private readonly List<FurnizorUE> _furnizori = new List<FurnizorUE>();

        public void AdaugaFurnizor(FurnizorUE furnizor)
        {
            _furnizori.Add(furnizor);
        }

        public List<FurnizorUE> GetAll()
        {
            return _furnizori.ToList();
        }

        public FurnizorUE CautaDupaCUI(string cuiVAT)
        {
            return _furnizori.FirstOrDefault(f => f.CuloareTVA_CIF == cuiVAT);
        }
    }
}
