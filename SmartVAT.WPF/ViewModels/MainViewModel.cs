using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using SmartVAT.Data;
using SmartVAT.Models;
using SmartVAT.WPF.Commands;

namespace SmartVAT.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private readonly IAdministratorFirme _adminFirme;
        private readonly IAdministratorFurnizori _adminFurnizori;

        // --- NAVIGATION VISIBILITY ---
        private string _currentTab = "Firme";
        public string VisFirme => _currentTab == "Firme" ? "Visible" : "Collapsed";
        public string VisFurnizori => _currentTab == "Furnizori" ? "Visible" : "Collapsed";
        public string VisDosare => _currentTab == "Dosare" ? "Visible" : "Collapsed";

        // --- DOSARE EXCEL ---
        public ObservableCollection<CerereRecuperareGrup> GrupuriCereri { get; set; } = new ObservableCollection<CerereRecuperareGrup>();
        public ICollectionView GrupuriCereriView { get; set; }
        private FirmaRO _firmaSelectataPentruDosar;
        public FirmaRO FirmaSelectataPentruDosar
        {
            get => _firmaSelectataPentruDosar;
            set { _firmaSelectataPentruDosar = value; OnPropertyChanged(nameof(FirmaSelectataPentruDosar)); }
        }

        public ICommand NavDosareCommand { get; }
        public ICommand NavFirmeCommand { get; }
        public ICommand NavFurnizoriCommand { get; }

        // --- LISTE ---
        public ObservableCollection<FirmaRO> Firme { get; set; }
        public ObservableCollection<FurnizorUE> Furnizori { get; set; }
        public ICollectionView FirmeView { get; set; }
        public ICollectionView FurnizoriView { get; set; }

        public List<string> JudeteDisponibile { get; } = new List<string> { "București", "Cluj", "Timiș", "Iași", "Constanța" };

        // --- FIRME ---
        private FirmaRO _selectedFirma;
        public FirmaRO SelectedFirma
        {
            get => _selectedFirma;
            set
            {
                _selectedFirma = value;
                OnPropertyChanged(nameof(SelectedFirma));
                OnPropertyChanged(nameof(VisEditFirma));
            }
        }
        public string VisEditFirma => SelectedFirma != null ? "Visible" : "Collapsed";

        private string _searchFirmeText = "";
        public string SearchFirmeText
        {
            get => _searchFirmeText;
            set
            {
                _searchFirmeText = value;
                OnPropertyChanged(nameof(SearchFirmeText));
                FirmeView.Refresh();
            }
        }

        public ICommand ActualizeazaFirmaCommand { get; }
        public ICommand AdaugaFirmaPopupCommand { get; }

        // --- FURNIZORI ---
        private string _oldCuiFurnizor;
        private FurnizorUE _selectedFurnizor;
        public FurnizorUE SelectedFurnizor
        {
            get => _selectedFurnizor;
            set
            {
                _selectedFurnizor = value;
                if (value != null) _oldCuiFurnizor = value.CuloareTVA_CIF;
                OnPropertyChanged(nameof(SelectedFurnizor));
                OnPropertyChanged(nameof(VisEditFurnizor));
            }
        }
        public string VisEditFurnizor => SelectedFurnizor != null ? "Visible" : "Collapsed";

        private string _searchFurnizoriText = "";
        public string SearchFurnizoriText
        {
            get => _searchFurnizoriText;
            set
            {
                _searchFurnizoriText = value;
                OnPropertyChanged(nameof(SearchFurnizoriText));
                FurnizoriView.Refresh();
            }
        }

        public ICommand ActualizeazaFurnizorCommand { get; }
        public ICommand StergeFurnizorCommand { get; }
        public ICommand AdaugaFurnizorPopupCommand { get; }
        public ICommand AdaugaDosarPopupCommand { get; }

        public ICommand InfoFirmaCommand { get; }
        public ICommand ModificaFirmaCommand { get; }

        public ICommand ImportExcelCommand { get; }
        public ICommand ClearDosareCommand { get; }
        public ICommand VeziFacturiCommand { get; }
        public ICommand GenereazaCerereCommand { get; }

        public MainViewModel()
        {
            _adminFirme = FabricaDeStocare.ObtineAdministratorFirme();
            _adminFurnizori = FabricaDeStocare.ObtineAdministratorFurnizori();

            Firme = new ObservableCollection<FirmaRO>(_adminFirme.GetAll());
            Furnizori = new ObservableCollection<FurnizorUE>(_adminFurnizori.GetAll());

            FirmeView = CollectionViewSource.GetDefaultView(Firme);
            FirmeView.Filter = f => string.IsNullOrEmpty(SearchFirmeText) || ((FirmaRO)f).Denumire.Contains(SearchFirmeText, StringComparison.OrdinalIgnoreCase) || ((FirmaRO)f).CUI.Contains(SearchFirmeText, StringComparison.OrdinalIgnoreCase);

            FurnizoriView = CollectionViewSource.GetDefaultView(Furnizori);
            FurnizoriView.Filter = f => string.IsNullOrEmpty(SearchFurnizoriText) || ((FurnizorUE)f).Denumire.Contains(SearchFurnizoriText, StringComparison.OrdinalIgnoreCase) || ((FurnizorUE)f).CuloareTVA_CIF.Contains(SearchFurnizoriText, StringComparison.OrdinalIgnoreCase);

            // Navigare
            NavDosareCommand = new RelayCommand(_ => SetTab("Dosare"));
            NavFirmeCommand = new RelayCommand(_ => SetTab("Firme"));
            NavFurnizoriCommand = new RelayCommand(_ => SetTab("Furnizori"));

            // Firme Commands
            ActualizeazaFirmaCommand = new RelayCommand(_ =>
            {
                _adminFirme.ActualizeazaFirma(SelectedFirma);
                MessageBox.Show("Firma a fost actualizată!");
                FirmeView.Refresh();
            }, _ => SelectedFirma != null);

            AdaugaFirmaPopupCommand = new RelayCommand(_ =>
            {
                var addFirmaWin = new AdaugaFirmaWindow();
                if (addFirmaWin.ShowDialog() == true && addFirmaWin.FirmaAdaugata != null)
                {
                    Firme.Add(addFirmaWin.FirmaAdaugata);
                    _adminFirme.AdaugaFirma(addFirmaWin.FirmaAdaugata);
                }
            });

            InfoFirmaCommand = new RelayCommand(f =>
            {
                var firma = f as FirmaRO;
                if (firma != null)
                {
                    var infoWin = new ProfilCompanieWindow(firma);
                    infoWin.ShowDialog();
                }
            });

            ModificaFirmaCommand = new RelayCommand(f =>
            {
                var firma = f as FirmaRO;
                if (firma != null)
                {
                    var editWin = new AdaugaFirmaWindow(firma);
                    if (editWin.ShowDialog() == true && editWin.FirmaAdaugata != null)
                    {
                        // Înlocuim datele
                        _adminFirme.ActualizeazaFirma(editWin.FirmaAdaugata);
                        var idx = Firme.IndexOf(firma);
                        if (idx >= 0) Firme[idx] = editWin.FirmaAdaugata;
                        MessageBox.Show("Firma a fost actualizată cu succes!");
                    }
                }
            });

            // Furnizori Commands
            ActualizeazaFurnizorCommand = new RelayCommand(_ =>
            {
                _adminFurnizori.ActualizeazaFurnizor(_oldCuiFurnizor, SelectedFurnizor);
                _oldCuiFurnizor = SelectedFurnizor.CuloareTVA_CIF; // actualizam daca s-a schimbat
                MessageBox.Show("Furnizorul a fost actualizat!");
                FurnizoriView.Refresh();
            }, _ => SelectedFurnizor != null);

            StergeFurnizorCommand = new RelayCommand(_ =>
            {
                _adminFurnizori.StergeFurnizor(SelectedFurnizor.CuloareTVA_CIF);
                Furnizori.Remove(SelectedFurnizor);
                SelectedFurnizor = null;
            }, _ => SelectedFurnizor != null);

            AdaugaFurnizorPopupCommand = new RelayCommand(_ =>
            {
                var addFurnizorWin = new AdaugaFurnizorWindow();
                if (addFurnizorWin.ShowDialog() == true && addFurnizorWin.FurnizorAdaugat != null)
                {
                    Furnizori.Add(addFurnizorWin.FurnizorAdaugat);
                    _adminFurnizori.AdaugaFurnizor(addFurnizorWin.FurnizorAdaugat);
                }
            });

            AdaugaDosarPopupCommand = new RelayCommand(_ =>
            {
                MessageBox.Show("Sectiune pastrata identic. Creare dosar indisponibila dupa stergerea AI-ului din requestul anterior.");
            });

            // Dosare / Cereri Commands
            GrupuriCereriView = CollectionViewSource.GetDefaultView(GrupuriCereri);

            ImportExcelCommand = new RelayCommand(_ =>
            {
                if (FirmaSelectataPentruDosar == null)
                {
                    MessageBox.Show("Vă rugăm să selectați o Firmă Solicitantă mai întâi!");
                    return;
                }

                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files|*.xlsx;*.xls",
                    Title = "Selectați Fișierul Excel cu Facturi"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var facturiNoi = FacturiExcelParser.ParseazaExcel(openFileDialog.FileName);
                        if (facturiNoi.Count == 0)
                        {
                            MessageBox.Show("Nu s-au găsit facturi valide în Excel (cu EXTERN sau Taxare Inversa).");
                            return;
                        }

                        // Grupare pe țări
                        var grupate = facturiNoi.GroupBy(f => f.FurnizorTemplate.PrefixTaraExtras);
                        foreach (var grup in grupate)
                        {
                            if (string.IsNullOrEmpty(grup.Key)) continue;

                            var existing = GrupuriCereri.FirstOrDefault(g => g.Tara == grup.Key);
                            if (existing != null)
                            {
                                existing.Facturi.AddRange(grup);
                                existing.NumarFacturi = existing.Facturi.Count;
                                existing.TotalTvaDeductibil = existing.Facturi.Sum(f => f.ValoareTVA);
                            }
                            else
                            {
                                GrupuriCereri.Add(new CerereRecuperareGrup
                                {
                                    Tara = grup.Key,
                                    Facturi = grup.ToList(),
                                    NumarFacturi = grup.Count(),
                                    TotalTvaDeductibil = Math.Round(grup.Sum(f => f.ValoareTVA), 2)
                                });
                            }
                        }

                        // Opțional: Salvăm și furnizorii în baza de date
                        foreach (var factura in facturiNoi)
                        {
                            if (_adminFurnizori.CautaDupaCUI(factura.FurnizorTemplate.CuloareTVA_CIF) == null)
                            {
                                _adminFurnizori.AdaugaFurnizor(factura.FurnizorTemplate);
                                Furnizori.Add(factura.FurnizorTemplate);
                            }
                        }

                        GrupuriCereriView.Refresh();
                        MessageBox.Show($"Au fost importate {facturiNoi.Count} facturi noi!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare la citirea Excel-ului: " + ex.Message);
                    }
                }
            });

            ClearDosareCommand = new RelayCommand(_ =>
            {
                GrupuriCereri.Clear();
                MessageBox.Show("Baza de date temporală pentru dosare a fost golită.");
            });

            VeziFacturiCommand = new RelayCommand(param =>
            {
                if (param is CerereRecuperareGrup grup)
                {
                    var win = new ListaFacturiWindow(grup);
                    win.ShowDialog();
                }
            });

            GenereazaCerereCommand = new RelayCommand(param =>
            {
                if (FirmaSelectataPentruDosar == null)
                {
                    MessageBox.Show("Alegeți Firma Solicitantă de sus!");
                    return;
                }

                if (param is CerereRecuperareGrup grup)
                {
                    try
                    {
                        var formular = new FormularD318
                        {
                            Solicitant = FirmaSelectataPentruDosar,
                            Achizitii = grup.Facturi
                        };
                        formular.SeteazaStatMembru(grup.Tara);
                        formular.AutoCalculeazaPerioada();

                        string numeFisier = $"D318_{FirmaSelectataPentruDosar.CUI}_{grup.Tara}_{System.DateTime.Now:yyyyMMdd_HHmm}.xml";
                        string caleDesktop = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), numeFisier);
                        
                        D318XmlGenerator.GenerateXml(formular, caleDesktop);

                        MessageBox.Show($"Dosarul XML D318 a fost generat cu succes pe Desktop!\n\nFișier: {numeFisier}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Eroare la generarea XML-ului: " + ex.Message);
                    }
                }
            });
        }

        private void SetTab(string tab)
        {
            _currentTab = tab;
            OnPropertyChanged(nameof(VisFirme));
            OnPropertyChanged(nameof(VisFurnizori));
            OnPropertyChanged(nameof(VisDosare));
        }
    }
}
