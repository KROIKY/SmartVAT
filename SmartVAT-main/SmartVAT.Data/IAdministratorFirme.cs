using System.Collections.Generic;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public interface IAdministratorFirme
    {
        void AdaugaFirma(FirmaRO firma);
        List<FirmaRO> GetAll();
        FirmaRO CautaDupaCUI(string cui);
    }
}
