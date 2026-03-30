using System;
using System.IO;

namespace SmartVAT.Data
{
    public static class FabricaDeStocare
    {
        private static readonly string FisierConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setari.config");
        private static string _tipStocare = "Memorie"; // Default

        static FabricaDeStocare()
        {
            if (File.Exists(FisierConfig))
            {
                // Format asteptat: TipSalvare=FisierText
                string linie = File.ReadAllText(FisierConfig).Trim();
                if (linie.StartsWith("TipSalvare="))
                {
                    _tipStocare = linie.Substring("TipSalvare=".Length);
                }
            }
        }

        public static IAdministratorFormulare ObtineAdministratorFormulare()
        {
            if (_tipStocare == "FisierText")
            {
                return new AdministratorFormulareFisierText();
            }
            return new AdministratorFormulareMemorie();
        }

        public static IAdministratorFirme ObtineAdministratorFirme()
        {
            if (_tipStocare == "FisierText")
            {
                return new AdministratorFirmeFisierText();
            }
            return new AdministratorFirmeMemorie();
        }

        public static IAdministratorFurnizori ObtineAdministratorFurnizori()
        {
            if (_tipStocare == "FisierText")
            {
                return new AdministratorFurnizoriFisierText();
            }
            return new AdministratorFurnizoriMemorie();
        }
    }
}
