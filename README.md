# SmartVAT este o aplicație desktop dezvoltată în C#, creată pentru a automatiza procesul de recuperare a TVA-ului pentru tranzacții intracomunitare (UE). Aplicația rezolvă problema procesării manuale a volumelor mari de date prin extragerea informațiilor direct din facturile electronice în format XML și pregătirea datelor pentru declarația oficială (D318).

## Deși formularele oficiale de tip XFA PDF pot afișa mesaje de eroare sau incompatibilitate în cititoarele PDF standard, această aplicație depășește aceste limitări prin manipularea directă a seturilor de date XML care stau la baza acestor documente.

# Funcționalități Principale:
## 1.Catalog de firme
Aplicația permite stocarea și gestionarea detaliilor despre companiile implicate, pentru a evita redundanța datelor:

Firme Locale (România): Datele firmei care solicită recuperarea (CUI, sediu, cont bancar).

Firme Externe: Bază de date cu partenerii din UE de la care se recuperează TVA-ul (Nume, Adresă, CUI).

## 2.Catalog de Utilizatori (Date Contabile)
Aplicația include un modul dedicat pentru gestionarea profilului utilizatorului (contabilul care depune declarația):

Profil Contabil: Stocarea datelor de identificare necesare.

Securitate: Acces securizat pe bază de user și parolă pentru a proteja datele fiscale sensibile.

Personalizare: Salvarea preferințelor de export și a istoricului de depuneri pentru fiecare utilizator în parte.

## 3.Extragerea de date (fișiere tip XML) & Clasificare ANAF
Automatizarea procesului de colectare a datelor prin identificarea automată a câmpurilor din factură și maparea lor pe codurile specifice declarației:

Mapping Coduri ANAF: Clasificarea automată a cheltuielilor (ex: Cod 1 pentru Motorină, Cod 4 pentru taxe, Cod 10 pentru ADBLUE etc.).

Identificatori: Numărul facturii și data emiterii.

Informații Fiscale: Codul de înregistrare fiscală (CUI/VAT ID) al furnizorului.

Localizare: Țara de origine a serviciului/produsului și moneda tranzacției.

Valori Financiare: Baza impozabilă și valoarea exactă a TVA-ului de recuperat.

## 4.Automatizarea Declarației
Centralizare: Gruparea mai multor facturi într-o singură sesiune de lucru.

Export Inteligent: Generarea unui fișier structurat compatibil cu importul de date în formularele inteligente (XFA) utilizate de autoritățile fiscale.
