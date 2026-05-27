using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SmartVAT.Models
{
    public class D318XmlGenerator
    {
        public static void GenerateXml(FormularD318 dosar, string outputPath)
        {
            decimal totalRambursare = dosar.GetTotalSumaSolicitataRambursare();
            string countryCode = dosar.StatMembruRambursare.Length >= 2 ? dosar.StatMembruRambursare.Substring(0, 2) : "AT";

            XNamespace ns = "mfp:anaf:dgti:d318:declaratie:v1";
            var vatRefundApplication = new XElement(ns + "VATRefundApplication",
                new XElement(ns + "codF1",
                    new XElement(ns + "formType", "D318"),
                    new XElement(ns + "formId", "318"),
                    new XElement(ns + "universalCode", "D318_A1.0.0")
                ),
                new XElement(ns + "ApplicationReference",
                    new XElement(ns + "RefundingCountryCode", countryCode),
                    new XElement(ns + "Currency", dosar.Moneda),
                    new XElement(ns + "Language", "RO"),
                    new XElement(ns + "Year", dosar.An),
                    new XElement(ns + "ApplicationType", 1),
                    new XElement(ns + "ReferenceNumber", dosar.CerereInitiala ? "" : dosar.NumeReferinta),
                    new XElement(ns + "Annual", 0),
                    new XElement(ns + "an_r", dosar.An),
                    new XElement(ns + "d_rec", dosar.CerereInitiala ? 0 : 1),
                    new XElement(ns + "luna_r", dosar.LunaSfarsit),
                    new XElement(ns + "cif", dosar.Solicitant.CUI.Replace("RO", "").Trim()),
                    new XElement(ns + "totalPlata_A", 0)
                ),
                new XElement(ns + "RefundPeriod",
                    new XElement(ns + "StartDate", dosar.LunaInceput),
                    new XElement(ns + "EndDate", dosar.LunaSfarsit)
                ),
                new XElement(ns + "ProrateAdjustment",
                    new XElement(ns + "Year", ""),
                    new XElement(ns + "FinalProrate", "")
                ),
                new XElement(ns + "Applicant",
                    new XElement(ns + "NameFree", dosar.Solicitant.Denumire),
                    new XElement(ns + "VATIdentificationNumber", dosar.Solicitant.CUI.Replace("RO", "").Trim()),
                    new XElement(ns + "AddressFree", dosar.Solicitant.DomiciliuFiscal),
                    new XElement(ns + "PostCode", ""),
                    new XElement(ns + "Phone", "0000000000"),
                    new XElement(ns + "EmailAddress", string.IsNullOrEmpty(dosar.Solicitant.EmailContabil) ? "contact@firma.ro" : dosar.Solicitant.EmailContabil)
                ),
                new XElement(ns + "BusinessDescription",
                    new XElement(ns + "BusinessActivity", dosar.Solicitant.CodCAEN)
                ),
                new XElement(ns + "Representative",
                    new XElement(ns + "NameFree", ""),
                    new XElement(ns + "RepresentativeID", ""),
                    new XElement(ns + "AddressFree", ""),
                    new XElement(ns + "PostCode", ""),
                    new XElement(ns + "Phone", ""),
                    new XElement(ns + "EmailAddress", ""),
                    new XElement(ns + "CountryCode", ""),
                    new XElement(ns + "identificationType", "TIN")
                ),
                new XElement(ns + "DetailedBankAccount",
                    new XElement(ns + "Total", totalRambursare.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)),
                    new XElement(ns + "Currency", "Empty"),
                    new XElement(ns + "OwnerType", 1),
                    new XElement(ns + "OwnerName", dosar.Solicitant.Denumire),
                    new XElement(ns + "IBAN", dosar.Solicitant.ContIBAN.Replace(" ", "")),
                    new XElement(ns + "BIC", dosar.Solicitant.CodBIC_SWIFT)
                )
            );

            var purchaseInformation = new XElement(ns + "PurchaseInformation");
            int seqNum = 1;

            foreach (var achizitie in dosar.Achizitii)
            {
                var furnizorCui = achizitie.FurnizorTemplate.CuloareTVA_CIF ?? "";
                string fCountryCode = furnizorCui.Length >= 2 ? furnizorCui.Substring(0, 2) : "EU";
                string cleanCui = furnizorCui.Length > 2 ? furnizorCui.Substring(2) : furnizorCui;

                var invoice = new XElement(ns + "Invoice",
                    new XElement(ns + "SequenceNumber", seqNum++),
                    new XElement(ns + "simplifiedInvoice", achizitie.EsteFacturaSimplificata ? 1 : 0),
                    new XElement(ns + "ReferenceNumber", achizitie.NumarFactura),
                    new XElement(ns + "IssuingDate", achizitie.DataFactura.ToString("yyyy-MM-dd"))
                );

                var goodsDescription = new XElement(ns + "GoodsDescription");
                
                // Add goods items based on nature
                if (achizitie.NaturaBunurilor.Count == 0)
                {
                    goodsDescription.Add(new XElement(ns + "GoodsItem",
                        new XElement(ns + "Code", 1),
                        new XElement(ns + "SubCode", "1.1"),
                        new XElement(ns + "FreeText", "")
                    ));
                }
                else
                {
                    foreach (var nat in achizitie.NaturaBunurilor)
                    {
                        if (nat.Descriere == "Motorina" || nat.Descriere == "Diesel")
                        {
                            goodsDescription.Add(new XElement(ns + "GoodsItem",
                                new XElement(ns + "Code", 1),
                                new XElement(ns + "SubCode", "1.1"),
                                new XElement(ns + "FreeText", "")
                            ));
                        }
                        else if (nat.Descriere == "AdBlue" || nat.Descriere == "AD BLUE")
                        {
                            goodsDescription.Add(new XElement(ns + "GoodsItem",
                                new XElement(ns + "Code", 10),
                                new XElement(ns + "SubCode", ""),
                                new XElement(ns + "FreeText", "AD BLUE")
                            ));
                        }
                        else
                        {
                            goodsDescription.Add(new XElement(ns + "GoodsItem",
                                new XElement(ns + "Code", 4),
                                new XElement(ns + "SubCode", "4.1"),
                                new XElement(ns + "FreeText", "")
                            ));
                        }
                    }
                }
                invoice.Add(goodsDescription);

                invoice.Add(new XElement(ns + "TransactionDescription",
                    new XElement(ns + "TaxableAmount", achizitie.BazaImpozabila.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)),
                    new XElement(ns + "VATAmount", achizitie.ValoareTVA.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))
                ));

                invoice.Add(new XElement(ns + "Deduction",
                    new XElement(ns + "ProRataRate", ""),
                    new XElement(ns + "DeductibleVATAmount", achizitie.TvaDeductibila.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))
                ));

                invoice.Add(new XElement(ns + "EUSupplier",
                    new XElement(ns + "NameFree", achizitie.FurnizorTemplate.Denumire),
                    new XElement(ns + "AddressFree", "Sediu"), // Fallback if no full address is collected
                    new XElement(ns + "CountryCode", fCountryCode),
                    new XElement(ns + "Phone", ""),
                    new XElement(ns + "EUTraderID",
                        new XElement(ns + "VATIdentificationNumber", cleanCui)
                    )
                ));

                purchaseInformation.Add(invoice);
            }

            vatRefundApplication.Add(purchaseInformation);

            // Adaugam sectiuni goale pentru ImportInformation si restul ca sa respecte fix schema
            vatRefundApplication.Add(new XElement(ns + "ImportInformation"));
            vatRefundApplication.Add(new XElement(ns + "DocumentCopy"));
            vatRefundApplication.Add(new XElement(ns + "NumberOfDocuments",
                new XElement(ns + "AtachatedFiles", 0),
                new XElement(ns + "PurchaseOrders", dosar.Achizitii.Count),
                new XElement(ns + "ImportOrders", 0)
            ));
            vatRefundApplication.Add(new XElement(ns + "ApplicantSignature",
                new XElement(ns + "NameFree", dosar.Solicitant.Denumire),
                new XElement(ns + "Position", "Administrator")
            ));
            vatRefundApplication.Add(new XElement(ns + "DocumentCopy1"));

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", null), vatRefundApplication);
            doc.Save(outputPath);
        }
    }
}
