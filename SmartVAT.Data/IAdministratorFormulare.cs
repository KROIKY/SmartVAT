using System.Collections.Generic;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public interface IAdministratorFormulare
    {
        // Salveaza un dosar intreg compus (Declaratie + Lista Facturi)
        void SalveazaFormular(FormularD318 formular);
        
        // Preluarea intregului istoric de declaratii D318
        List<FormularD318> GetToateFormularele();
        
        // Cerinta de Cautare LINQ adaptata la Formulare (D318) - Cauta declaratiile de pe un anume an.
        List<FormularD318> CautaFormulareDupaAn(int an);
    }
}
