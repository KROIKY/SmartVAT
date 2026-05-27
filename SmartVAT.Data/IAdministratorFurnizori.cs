using System.Collections.Generic;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public interface IAdministratorFurnizori
    {
        void AdaugaFurnizor(FurnizorUE furnizor);
        List<FurnizorUE> GetAll();
        FurnizorUE CautaDupaCUI(string cuiVAT);
        void ActualizeazaFurnizor(string vechiCui, FurnizorUE furnizor);
        void StergeFurnizor(string cuiVAT);
    }
}
