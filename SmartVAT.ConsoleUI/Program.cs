using System;
using System.Collections.Generic;
using System.Linq;
using SmartVAT.Models;
using SmartVAT.Data;

namespace SmartVAT.ConsoleUI
{
    class Program
    {
        static IAdministratorFirme adminFirme = FabricaDeStocare.ObtineAdministratorFirme();
        static IAdministratorFurnizori adminFurnizori = FabricaDeStocare.ObtineAdministratorFurnizori();
        static IAdministratorFormulare adminFormulare = FabricaDeStocare.ObtineAdministratorFormulare();

        // Dosarul Curent
        static FormularD318 dosarCurent = new FormularD318();

        static void Main(string[] args)
        {
            bool ruleaza = true;

            while (ruleaza)
            {
                Console.Clear();
                Console.WriteLine("====================================================");
                Console.WriteLine(" SMARTVAT - DECLARATIE INTELIGENTA ");
                Console.WriteLine("====================================================");
                
                string taraStatus = string.IsNullOrEmpty(dosarCurent.StatMembruRambursare) ? "NESETAT!" : dosarCurent.StatMembruRambursare;
                string ns = dosarCurent.Solicitant?.Denumire;
                
                Console.WriteLine($"> STATUS DOSAR [Tara vizata: {taraStatus}] [Limba: {dosarCurent.LimbaOficiala}]");
                Console.WriteLine($"  Solicitant: {(string.IsNullOrEmpty(ns) ? "LIPSA!" : ns)}");
                Console.WriteLine($"  Facturi atasate: {dosarCurent.Achizitii.Count} | Suma Deductibila: {dosarCurent.GetTotalSumaSolicitataRambursare()} {dosarCurent.Moneda}\n");
                
                Console.WriteLine("--- ACTIUNI CATALOG ---");
                Console.WriteLine(" 1. Adauga Firma Solicitanta ");
                Console.WriteLine(" 2. Adauga Furnizor in Catalog ");
                
                Console.WriteLine("\n--- COMPLETARI DOSAR CURENT ---");
                Console.WriteLine(" 3. Setari Dosar: Alege Solicitantul & Tara Externa");
                Console.WriteLine(" 4. Insereaza Factura ");
                Console.WriteLine(" 5. >> SALVEAZA DOSARUL <<");
                
                Console.WriteLine("\n--- ARHIVA DOSARE ---");
                Console.WriteLine(" 6. Arhiva Dosare D318 Depuse");
                
                Console.WriteLine("\n 0. Iesire");
                Console.WriteLine("====================================================");
                Console.Write("Alege optiunea: ");

                string opt = Console.ReadLine();

                try
                {
                    switch (opt)
                    {
                        case "1": CitireFirmaCatalog(); break;
                        case "2": CitireFurnizorCatalog(); break;
                        case "3": SetariDosarInitiale(); break;
                        case "4": AdaugaFacturaInDosar(); break;
                        case "5": FinalizareDosarInteligenta(); break;
                        case "6": RasfoireIstoricDosare(); break;
                        case "0": ruleaza = false; break;
                        default: Console.WriteLine("Optiune invalida!"); Console.ReadLine(); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[EROARE FATALA]: {ex.Message}");
                    Console.ReadLine();
                }
            }
        }

        static void CitireFirmaCatalog()
        {
            Console.Clear();
            Console.WriteLine("--- ADAUGARE SOLICITANT NOU IN CATALOG ---");
            FirmaRO f = new FirmaRO();

            // Date Firma (Solicitant)
            Console.WriteLine("[DATE FIRMA]");
            Console.Write(" 1. Denumire Firma (Ex: SC ROMY SRL): "); f.Denumire = Console.ReadLine();
            Console.Write(" 2. CUI (Ex: RO12345678): RO"); f.CUI = "RO" + Console.ReadLine();
            Console.Write(" 3. Domiciliu Fiscal Complet: "); f.DomiciliuFiscal = Console.ReadLine();
            Console.Write(" 4. Cod CAEN Activitate (Ex: 4941): "); f.CodCAEN = Console.ReadLine();

            // Date Contabil (Reprezentant)
            Console.WriteLine("\n[DATE CONTACT CONTABIL / DESEMNAT]");
            Console.Write(" 5. Telefonul Contabilului: "); f.TelefonContabil = Console.ReadLine();
            Console.Write(" 6. E-mail-ul Contabilului: "); f.EmailContabil = Console.ReadLine();

            // Date Financiare Fixe pentru Firma
            Console.WriteLine("\n[INFORMATII FINANCIARE]");
            Console.WriteLine($"> Titularul contului va fi autosetat pe: {f.ObtineTitularCont()}");
            Console.Write(" 7. Cod IBAN Cont Recuperare: "); f.ContIBAN = Console.ReadLine();
            Console.Write(" 8. Cod BIC / SWIFT: "); f.CodBIC_SWIFT = Console.ReadLine();

            adminFirme.AdaugaFirma(f);
            Console.WriteLine("\n[SUCCES] Firma inserata. Apasa Enter.");
            Console.ReadLine();
        }

        static void CitireFurnizorCatalog()
        {
            Console.Clear();
            Console.WriteLine("--- ADAUGARE SABLON PRESTATOR/FURNIZOR UE ---");
            FurnizorUE fur = new FurnizorUE();

            // Tratarea inteligenta a CUI-ului cu prefix.
            Console.Write("CUI FURNIZOR (Ex: ATU38992006): "); 
            fur.CuloareTVA_CIF = Console.ReadLine();
            Console.WriteLine($" -> [Smart Extractor] Am preluat tara {fur.PrefixTaraExtras} si CUI efectiv {fur.CorpCuloareTVA}");

            Console.Write("Denumire Furnizor (Ex: UNION TANK GMBH): "); fur.Denumire = Console.ReadLine();
            Console.Write("Adresa Sediu Furnizor: "); fur.Adresa = Console.ReadLine();
    
            adminFurnizori.AdaugaFurnizor(fur);
            Console.WriteLine("\n[SUCCES] Sablonul e creat. Apasa Enter.");
            Console.ReadLine();
        }

        static void SetariDosarInitiale()
        {
            Console.Clear();
            Console.WriteLine("--- SETARE D318 (DECLARATIE NOUA) ---");
            
            Console.WriteLine("\n-> Tara (Statul Membru) din care se va recupera TVA-ul:");
            Console.WriteLine("    [Selecteaza numarul aferent]");
            
            for (int i = 0; i < ConstanteFiscale.TariRambursare.Count; i++)
            {
                var t = ConstanteFiscale.TariRambursare[i];
                Console.WriteLine($"      {i + 1,2}. {t.Cod} - {t.Nume}");
            }
            
            Console.Write("\nScrie un Numar (1-26): ");
            if (int.TryParse(Console.ReadLine(), out int sel) && sel >= 1 && sel <= ConstanteFiscale.TariRambursare.Count)
            {
                dosarCurent.SeteazaStatMembru(ConstanteFiscale.TariRambursare[sel - 1].Cod);
            }
            else
            {
                dosarCurent.SeteazaStatMembru("C_INVALID"); // Acesta va chema exceptia din modele
            }

            Console.WriteLine($"\n[SETAT Lb/Moneda AUTOMAT] {dosarCurent.StatMembruRambursare} -> Limba: {dosarCurent.LimbaOficiala} | Moneda: {dosarCurent.Moneda}");

            var firme = adminFirme.GetAll();
            if (firme.Count == 0)
            {
                Console.WriteLine("\n[X] Nu ai firme. Utilizeaza Optiunea 1 prima data!");
                Console.ReadLine(); return;
            }

            Console.WriteLine("\n-> Alege Solicitantul Dosarului:");
            foreach (var firma in firme) Console.WriteLine($"   {firma.CUI} | {firma.Denumire}");
            Console.Write("Scrie CUI (Atentie la prefix RO): ");
            string c = Console.ReadLine();

            var gasit = adminFirme.CautaDupaCUI(c);
            if (gasit != null)
            {
                dosarCurent.Solicitant = gasit;
                Console.WriteLine($"\n[OK] Firma atasata, contabilul aferent va fi preluat.");
            }
            Console.ReadLine();
        }

        static void AdaugaFacturaInDosar()
        {
            Console.Clear();
            Console.WriteLine("--- INSERARE FACTURA IN DOSARUL CURENT ---");

            if (string.IsNullOrEmpty(dosarCurent.StatMembruRambursare))
            {
               Console.WriteLine("Intai seteaza Tara si Solicitantul folosind optiunea 3!"); 
               Console.ReadLine(); return;
            }

            FacturaAchizitie fact = new FacturaAchizitie();

            // 1. SELECTARE FURNIZOR SABLON
            var listaFurnizori = adminFurnizori.GetAll();
            if (listaFurnizori.Count == 0) { Console.WriteLine("Nu aveti Furnizori in catalog (Optiunea 2)!"); Console.ReadLine(); return; }

            Console.WriteLine("Alege Furnizorul:");
            foreach (var furnizor in listaFurnizori) Console.WriteLine($"> [{furnizor.PrefixTaraExtras}] {furnizor.CorpCuloareTVA} | {furnizor.Denumire}");
            Console.Write("\nScrie Cod CUI COMPLET (Ex. ATU..): ");
            var furnizorSelectat = adminFurnizori.CautaDupaCUI(Console.ReadLine());
            if(furnizorSelectat == null) return;
            
            fact.FurnizorTemplate = furnizorSelectat;

            // 2. DATE FACTURA STRICTE (Cu Validare Data)
            Console.Write("\nNumar Factura: "); fact.NumarFactura = Console.ReadLine();
            
            bool okData = false;
            while (!okData)
            {
                Console.Write("Data Facturii (RESTRICTIV zz.ll.aaaa cu Puncte, ex 05.10.2025): ");
                try
                {
                    fact.DataFactura = DateTime.ParseExact(Console.ReadLine(), "dd.MM.yyyy", null);
                    okData = true;
                }
                catch { Console.WriteLine("Format invalid!"); }
            }

            // 3. NATURA MULTIPLA A BUNURILOR
            Console.WriteLine("\n-- Adaugare Linie Achizitie --");
            bool incaLinii = true;
            while(incaLinii)
            {
                CheltuialaFactura r = new CheltuialaFactura();
                Console.Write(" > Cod Bun (1=Combustibil, 4=Taxe, 10=AdBlue): "); r.Cod = int.Parse(Console.ReadLine());
                Console.Write(" > Sub-Cod: "); r.SubCod = Console.ReadLine();
                Console.Write(" > Descriere: "); r.Descriere = Console.ReadLine();
                fact.NaturaBunurilor.Add(r);
                
                Console.Write(" >> Adaugi si alta Categorie pe aceeasi factura? (D/N): ");
                incaLinii = (Console.ReadLine()?.ToUpper() == "D");
            }

            // 4. BAZA SI TVA (AUTO-DEDUCTIBIL SMART)
            Console.WriteLine("\n-- Financiare --");
            Console.Write($"Baza Impozabila ({dosarCurent.Moneda}): "); fact.BazaImpozabila = decimal.Parse(Console.ReadLine());
            Console.Write($"Valoare TVA ({dosarCurent.Moneda}): "); fact.ValoareTVA = decimal.Parse(Console.ReadLine());
            
            Console.WriteLine($"[SMART] -> TVA Deductibil preselectat si ascuns in mod egal cu Valoarea TVA ({fact.TvaDeductibila})!");

            dosarCurent.Achizitii.Add(fact);
            Console.WriteLine("\n[SUCCES] Factura a fost inregistrata automat pe statul curent. Apasa Enter.");
            Console.ReadLine();
        }

        static void FinalizareDosarInteligenta()
        {
            Console.Clear();
            Console.WriteLine("--- ANALIZA SI SALVARE DOSAR D318 ---");
            
            if(string.IsNullOrEmpty(dosarCurent.Solicitant?.Denumire) || dosarCurent.Achizitii.Count == 0)
            {
                Console.WriteLine("Dosarul este incomplet. Adauga solicitant si facturi! (1-4)");
                Console.ReadLine(); return;
            }

            // SMART Aici se face toata magia - extragem datele calendaristice deduse.
            dosarCurent.AutoCalculeazaPerioada();

            Console.WriteLine("SISTEMUL A DEDUS AUTOMAT URMATOARELE:");
            Console.WriteLine($"> Facturile graviteaza in perioada Anului {dosarCurent.An}.");
            Console.WriteLine($"> Din lunile extrese minim-maxim: LUNA {dosarCurent.LunaInceput} - LUNA {dosarCurent.LunaSfarsit}");
            Console.WriteLine($"> Total Facturi Preluat de la furnizori: {dosarCurent.Achizitii.Count}");
            Console.WriteLine($"> Suma de Recuperat: {dosarCurent.GetTotalSumaSolicitataRambursare()} {dosarCurent.Moneda} ({dosarCurent.LimbaOficiala})");

            adminFormulare.SalveazaFormular(dosarCurent);
            Console.WriteLine($"\n[SUCCESS FINAL] Declaratia fiscala (Dosar Inteligent) a fost inscrisa in baza de date TXT.");
            
            // Re-initializare pentru un viitor dosar
            dosarCurent = new FormularD318();
            Console.ReadLine();
        }

        static void RasfoireIstoricDosare()
        {
            Console.Clear();
            Console.WriteLine("--- CITITOR DECLARATII / ISTORIC ---");
            var lst = adminFormulare.GetToateFormularele();

            if (lst.Count == 0) Console.WriteLine("Arhiva goala.");

            foreach(var d in lst)
            {
                Console.WriteLine($"\n> {d.NumeReferinta} | Firma: {d.Solicitant.Denumire}");
                Console.WriteLine($"  Perioada Deduceata: L{d.LunaInceput}-L{d.LunaSfarsit} ({d.An}) | Cerere: {d.StatMembruRambursare}");
                Console.WriteLine($"  >> Rambursare Certificata: {d.GetTotalSumaSolicitataRambursare()} {d.Moneda} <<");
            }
            Console.ReadLine();
        }
    }
}
