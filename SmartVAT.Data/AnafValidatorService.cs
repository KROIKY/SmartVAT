using System;
using System.Diagnostics;
using System.IO;

namespace SmartVAT.Data
{
    public class AnafValidatorService
    {
        public static string ValideazaXmlSiGenereazaPdf(string xmlPath, string anafToolsDirectory)
        {
            // The directory passed from UI should point to 'dist' folder containing DUKIntegrator.jar
            string validatorJar = Path.Combine(anafToolsDirectory, "DUKIntegrator.jar");
            if (!File.Exists(validatorJar))
            {
                return $"Eroare: Nu s-a găsit DUKIntegrator.jar în {anafToolsDirectory}";
            }

            try
            {
                // Deschidem folderul unde s-a generat XML-ul ca să îi fie ușor utilizatorului
                Process.Start("explorer.exe", $"/select,\"{xmlPath}\"");

                var processInfo = new ProcessStartInfo("java")
                {
                    Arguments = $"-jar \"{validatorJar}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = anafToolsDirectory
                };

                Process.Start(processInfo);
                
                return $"XML-ul a fost generat cu succes!\n\nS-a deschis programul ANAF (DUKIntegrator) și folderul cu fișierul D318.xml.\n\nApasă pe 'Alege fișiere' în DUKIntegrator, selectează fișierul D318.xml, și dă click pe 'Validare + creare PDF'!";
            }
            catch (Exception ex)
            {
                return "A apărut o excepție la rularea Java: " + ex.Message;
            }
        }
    }
}
