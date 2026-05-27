using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;
using SmartVAT.Models;

namespace SmartVAT.Data
{
    public class FacturiExcelParser
    {
        // rate fixe prestabilite din EUR in moneda straina (pentru a calcula suma in moneda de rambursare)
        private static readonly Dictionary<string, decimal> _rateConversie = new Dictionary<string, decimal>
        {
            { "AT", 1.0m },   // EUR -> EUR
            { "BE", 1.0m },   // EUR -> EUR
            { "DE", 1.0m },   // EUR -> EUR
            { "IT", 1.0m },   // EUR -> EUR
            { "FR", 1.0m },   // EUR -> EUR
            { "ES", 1.0m },   // EUR -> EUR
            { "LU", 1.0m },   // EUR -> EUR
            { "HU", 390.0m }, // 1 EUR = ~390 HUF
            { "BG", 1.95m },  // 1 EUR = ~1.95 BGN
            { "PL", 4.30m },  // 1 EUR = ~4.30 PLN
            { "CZ", 25.00m }  // 1 EUR = ~25.00 CZK
        };

        public static decimal GetRataConversie(string countryCode)
        {
            if (_rateConversie.TryGetValue(countryCode.ToUpper(), out decimal rata))
                return rata;
            return 1.0m; // Default EUR
        }

        public static List<FacturaAchizitie> ParseazaExcel(string filePath)
        {
            var facturi = new List<FacturaAchizitie>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false } // Nu stim daca e prima linie
                    };
                    var dataSet = reader.AsDataSet(conf);
                    if (dataSet.Tables.Count == 0) return facturi;

                    var table = dataSet.Tables[0];
                    
                    // Găsim dicționarul de coloane (nume -> index coloană) mapând din antet
                    var colIndices = new Dictionary<string, int>();
                    int headerRowIndex = -1;

                    string[] expectedHeaders = { "valoare", "val_fara_tva", "partener_adresa", "partener_cod_fiscal", "data_doc", "nume_furn", "nr_doc" };

                    // Căutăm rândul de antet (cel care conține măcar 3 din cuvintele cheie)
                    for (int r = 0; r < Math.Min(20, table.Rows.Count); r++)
                    {
                        int matches = 0;
                        var tempIndices = new Dictionary<string, int>();

                        for (int c = 0; c < table.Columns.Count; c++)
                        {
                            string cellVal = table.Rows[r][c]?.ToString()?.ToLower() ?? "";
                            foreach (var eh in expectedHeaders)
                            {
                                if (cellVal.Contains(eh.ToLower()))
                                {
                                    matches++;
                                    tempIndices[eh] = c;
                                }
                            }
                        }

                        if (matches >= 3)
                        {
                            headerRowIndex = r;
                            colIndices = tempIndices;
                            break;
                        }
                    }

                    if (headerRowIndex == -1)
                        throw new Exception("Nu am putut identifica antetul tabelului. Asigurați-vă că Excel-ul conține coloane precum nr_doc, valoare, data_doc.");

                    var groupedRows = new Dictionary<string, List<DataRow>>();

                    for (int i = headerRowIndex + 1; i < table.Rows.Count; i++)
                    {
                        var row = table.Rows[i];
                        bool hasKeyword = false;

                        // Căutăm în TOATE celulele rândului
                        for (int c = 0; c < table.Columns.Count; c++)
                        {
                            string cellValue = row[c]?.ToString()?.ToLower() ?? "";
                            if (cellValue.Contains("extern") || cellValue.Contains("externe") || cellValue.Contains("taxare inversa"))
                            {
                                hasKeyword = true;
                                break;
                            }
                        }

                        if (hasKeyword)
                        {
                            string nrDoc = ObtineValoareDinIndex(row, colIndices, "nr_doc");
                            if (string.IsNullOrEmpty(nrDoc)) continue;

                            if (!groupedRows.ContainsKey(nrDoc))
                                groupedRows[nrDoc] = new List<DataRow>();
                            
                            groupedRows[nrDoc].Add(row);
                        }
                    }

                    // Procesare facturi grupate
                    foreach (var kvp in groupedRows)
                    {
                        string nrDoc = kvp.Key;
                        var rows = kvp.Value;
                        var firstRow = rows.First();

                        string cui = ObtineValoareDinIndex(firstRow, colIndices, "partener_cod_fiscal").Trim().Replace(" ", "");
                        string furnizorNume = ObtineValoareDinIndex(firstRow, colIndices, "nume_furn");
                        string adresa = ObtineValoareDinIndex(firstRow, colIndices, "partener_adresa");
                        string dataDocStr = ObtineValoareDinIndex(firstRow, colIndices, "data_doc");

                        DateTime dataFactura = DateTime.Now;
                        DateTime.TryParse(dataDocStr, out dataFactura);

                        var furnizor = new FurnizorUE
                        {
                            CuloareTVA_CIF = cui,
                            Denumire = furnizorNume,
                            Adresa = adresa
                        };

                        decimal rata = GetRataConversie(furnizor.PrefixTaraExtras);
                        decimal totalBazaEur = 0;
                        decimal totalTvaEur = 0;
                        var naturaBunurilor = new List<CheltuialaFactura>();

                        foreach (var row in rows)
                        {
                            decimal valoareTotala = ObtineDecimalDinIndex(row, colIndices, "valoare");
                            decimal bazaEur = ObtineDecimalDinIndex(row, colIndices, "val_fara_tva");
                            decimal tvaEur = valoareTotala - bazaEur;

                            totalBazaEur += bazaEur;
                            totalTvaEur += tvaEur;

                            string exp = "";
                            for (int c = 0; c < table.Columns.Count; c++)
                            {
                                string cellVal = row[c]?.ToString()?.ToLower() ?? "";
                                if (cellVal.Contains("extern") || cellVal.Contains("taxare inversa") || cellVal.Contains("motorin") || cellVal.Contains("diesel") || cellVal.Contains("tax") || cellVal.Contains("toll") || cellVal.Contains("vignet") || cellVal.Contains("adblue"))
                                {
                                    exp += " " + cellVal;
                                }
                            }

                            int cod = 10;
                            string subCod = "";
                            
                            if (exp.Contains("motorin") || exp.Contains("diesel") || exp.Contains("combustibil"))
                            {
                                cod = 1;
                                subCod = "1.1.2";
                            }
                            else if (exp.Contains("tax") || exp.Contains("toll") || exp.Contains("vignet"))
                            {
                                cod = 4;
                                subCod = "4.1";
                            }
                            else if (exp.Contains("adblue") || exp.Contains("inversa"))
                            {
                                cod = 10;
                            }

                            if (!naturaBunurilor.Any(c => c.Cod == cod && c.SubCod == subCod))
                            {
                                naturaBunurilor.Add(new CheltuialaFactura
                                {
                                    Cod = cod,
                                    SubCod = subCod,
                                    Descriere = string.IsNullOrEmpty(subCod) ? "Diverse" : "Cod extras automat"
                                });
                            }
                        }

                        decimal bazaFinala = Math.Round(totalBazaEur * rata, 2);
                        decimal tvaFinala = Math.Round(totalTvaEur * rata, 2);

                        var factura = new FacturaAchizitie
                        {
                            NumarFactura = nrDoc,
                            DataFactura = dataFactura,
                            FurnizorTemplate = furnizor,
                            BazaImpozabila = bazaFinala,
                            ValoareTVA = tvaFinala,
                            NaturaBunurilor = naturaBunurilor
                        };

                        facturi.Add(factura);
                    }
                }
            }

            return facturi;
        }

        private static string ObtineValoareDinIndex(DataRow row, Dictionary<string, int> colIndices, string prop)
        {
            if (colIndices.TryGetValue(prop, out int index))
            {
                return row[index]?.ToString() ?? "";
            }
            return "";
        }

        private static decimal ObtineDecimalDinIndex(DataRow row, Dictionary<string, int> colIndices, string nume)
        {
            string val = ObtineValoareDinIndex(row, colIndices, nume).Replace(",", ".");
            if (decimal.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal rezultat))
            {
                return rezultat;
            }
            return 0;
        }
    }
}
