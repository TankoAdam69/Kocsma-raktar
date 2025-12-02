using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace KocsmaLeltar
{
    public class MainViewModel : BaseViewModel
    {
        private const string LELTAR_FAJL = "leltar.json";
        private const string KATEGORIA_FAJL = "kategoriak.json";

        public ObservableCollection<Ital> Leltar { get; set; }
        public List<string> Kategoriak { get; set; }

        private Ital _kivalasztottItal;
        public Ital KivalasztottItal
        {
            get => _kivalasztottItal;
            set
            {
                if (_kivalasztottItal != value)
                {
                    _kivalasztottItal = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand UjItalCommand { get; }
        public ICommand MentesCommand { get; }
        public ICommand TorlesCommand { get; }

        public MainViewModel()
        {
            KategoriakBetoltese();
            AdatokBetoltese();

            UjItalCommand = new RelayCommand(UjItal);
            MentesCommand = new RelayCommand(Mentes, VanKivalasztva);
            TorlesCommand = new RelayCommand(Torles, VanKivalasztvaEsMentett);
        }

        private void UjItal(object obj)
        {
            KivalasztottItal = new Ital();
        }

        private void Mentes(object obj)
        {
            if (KivalasztottItal == null) return;

            if (string.IsNullOrWhiteSpace(KivalasztottItal.Nev) || KivalasztottItal.Nev == "Új ital")
            {
                MessageBox.Show("A név megadása kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 1. ESET: A felhasználó egy ÚJ TÉTELT hoz létre
            if (!Leltar.Contains(KivalasztottItal))
            {
                // Ellenőrizzük, hogy logikailag létezik-e már (azonos név + kategória)
                var letezoItal = Leltar.FirstOrDefault(ital =>
                    ital.Nev.Equals(KivalasztottItal.Nev, StringComparison.OrdinalIgnoreCase) &&
                    ital.Kategoria.Equals(KivalasztottItal.Kategoria, StringComparison.OrdinalIgnoreCase));

                // 1A. ESET: IGEN, LÉTEZIK ILYEN -> Készlet hozzáadása és dátum frissítése
                if (letezoItal != null)
                {
                    DateTime ujDatum = KivalasztottItal.UtolsoRendelesDatum;
                    DateTime regiDatum = letezoItal.UtolsoRendelesDatum;

                    // Dátum validálása
                    if (ujDatum.Date < regiDatum.Date)
                    {
                        MessageBox.Show($"Hiba: Az új rendelés dátuma ({ujDatum:yyyy.MM.dd}) nem lehet korábbi, mint az utolsó ismert rendelés dátuma ({regiDatum:yyyy.MM.dd}).", "Dátumhiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Adatok frissítése: Mennyiség hozzáadása és dátum frissítése
                    letezoItal.Mennyiseg += KivalasztottItal.Mennyiseg;
                    letezoItal.UtolsoRendelesDatum = ujDatum;

                    AdatokMentese();
                    MessageBox.Show($"A meglévő '{letezoItal.Nev}' tétel készlete frissítve!", "Siker");
                    KivalasztottItal = letezoItal;
                }
                // 1B. ESET: NEM LÉTEZIK ILYEN -> Új tétel felvétele
                else
                {
                    Leltar.Add(KivalasztottItal);
                    AdatokMentese();
                    MessageBox.Show("Új ital elmentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            // 2. ESET: A felhasználó egy MEGLÉVŐ TÉTELT módosít
            else
            {
                AdatokMentese();
                MessageBox.Show("Módosítások elmentve!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Torles(object obj)
        {
            if (KivalasztottItal == null) return;

            var valasz = MessageBox.Show($"Biztosan törölni szeretnéd a(z) '{KivalasztottItal.Nev}' tételt?",
                                         "Törlés megerősítése", MessageBoxButton.YesNo);

            if (valasz == MessageBoxResult.Yes)
            {
                Leltar.Remove(KivalasztottItal);
                KivalasztottItal = null!;
                AdatokMentese();
            }
        }

        private bool VanKivalasztva(object obj)
        {
            return KivalasztottItal != null;
        }

        private bool VanKivalasztvaEsMentett(object obj)
        {
            return KivalasztottItal != null && Leltar.Contains(KivalasztottItal);
        }

        // --- Az adatkezelő metódusok (Betöltés/Mentés) változatlanok ---

        private void KategoriakBetoltese()
        {
            try
            {
                if (File.Exists(KATEGORIA_FAJL))
                {
                    string json = File.ReadAllText(KATEGORIA_FAJL);
                    Kategoriak = JsonSerializer.Deserialize<List<string>>(json)!;
                }
                else
                {
                    Kategoriak = new List<string> { "Sör", "Bor", "Rövidital", "Üdítő", "Egyéb" };
                    KategoriakMentese();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a kategóriák betöltésekor: {ex.Message}");
                Kategoriak = new List<string> { "Hiba" };
            }
        }

        private void KategoriakMentese()
        {
            try
            {
                string json = JsonSerializer.Serialize(Kategoriak, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(KATEGORIA_FAJL, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a kategóriák mentésekor: {ex.Message}");
            }
        }

        private void AdatokBetoltese()
        {
            try
            {
                if (File.Exists(LELTAR_FAJL))
                {
                    string json = File.ReadAllText(LELTAR_FAJL);
                    var adatok = JsonSerializer.Deserialize<List<Ital>>(json);
                    Leltar = new ObservableCollection<Ital>(adatok!);
                }
                else
                {
                    Leltar = new ObservableCollection<Ital>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a leltár betöltésekor: {ex.Message}");
                Leltar = new ObservableCollection<Ital>();
            }
        }

        private void AdatokMentese()
        {
            try
            {
                string json = JsonSerializer.Serialize(Leltar.ToList(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(LELTAR_FAJL, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a leltár mentésekor: {ex.Message}");
            }
        }
    }
}