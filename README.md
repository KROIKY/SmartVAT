# SmartVAT este o aplicatie desktop dezvoltata in C#, creata pentru a automatiza procesul de recuperare a TVA-ului pentru tranzactii intracomunitare (UE). Aplicatia rezolva problema procesarii manuale a volumelor mari de date prin extragerea informatiilor direct din facturile electronice in format XML si pregatirea datelor pentru declaratia oficiala (D318).

## Desi formularele oficiale pot afisa mesaje de eroare sau incompatibilitate in aplicatiile standard, aceasta aplicatie depaseste aceste limitari prin organizarea si manipularea directa a seturilor de date XML care stau la baza acestor documente.

# Functionalitati Principale:
## 1.Catalog de firme
Aplicatia permite stocarea si gestionarea detaliilor despre companiile implicate, pentru a evita redundanta datelor:

Firme Locale (Romania): Datele firmei care solicita recuperarea (CUI, sediu, cont bancar).

Firme Externe: Baza de date cu partenerii din UE de la care se recupereaza TVA-ul (Nume, Adresa, CUI).

## 2.Catalog de Utilizatori (Date Contabile)
Aplicatia include un modul dedicat pentru gestionarea profilului utilizatorului (contabilul care depune declaratia):

Profil Contabil: Stocarea datelor de identificare necesare.

Securitate: Acces securizat pe baza de user si parola pentru a proteja datele fiscale sensibile.

Personalizare: Salvarea preferintelor de export si a istoricului de depuneri pentru fiecare utilizator in parte.

## 3.Extragerea de date (fisiere tip XML) & Clasificare ANAF
Automatizarea procesului de colectare a datelor prin identificarea automata a campurilor din factura si maparea lor pe codurile specifice declaratiei:

Mapping Coduri ANAF: Clasificarea automata a cheltuielilor (ex: Cod 1 pentru Motorina, Cod 4 pentru taxe, Cod 10 pentru ADBLUE etc.).

Identificatori: Numarul facturii si data emiterii.

Informatii Fiscale: Codul de inregistrare fiscala (CUI/VAT ID) al furnizorului.

Localizare: Tara de origine a serviciului/produsului si moneda tranzactiei.

Valori Financiare: Baza impozabila si valoarea exacta a TVA-ului de recuperat.

## 4.Automatizarea Declaratiei
Centralizare: Gruparea mai multor facturi intr-o singura sesiune de lucru.

Export Inteligent: Generarea unui model de date structurat compatibil cu importul rapid in formularele electronice de tip XML utilizate de autoritatile fiscale.
