using System.Collections.Generic;
using System.Linq;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class AdministratorFormulareMemorie : IAdministratorFormulare
    {
        private readonly List<FormularD318> _formulare = new List<FormularD318>();

        public void SalveazaFormular(FormularD318 formular)
        {
            _formulare.Add(formular);
        }

        public List<FormularD318> GetToateFormularele()
        {
            return _formulare.ToList();
        }

        public List<FormularD318> CautaFormulareDupaAn(int an)
        {
            return _formulare.Where(f => f.An == an).ToList();
        }
    }
}
