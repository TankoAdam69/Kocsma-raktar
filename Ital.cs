using System;

namespace KocsmaLeltar
{
    public class Ital : BaseViewModel
    {
        private string _nev = "Új ital";
        public string Nev
        {
            get => _nev;
            set
            {
                if (_nev != value)
                {
                    _nev = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _kategoria = "Sör";
        public string Kategoria
        {
            get => _kategoria;
            set
            {
                if (_kategoria != value)
                {
                    _kategoria = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _mennyiseg;
        public int Mennyiseg
        {
            get => _mennyiseg;
            set
            {
                if (_mennyiseg != value)
                {
                    _mennyiseg = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _keszletenVan = true;
        public bool KeszletenVan
        {
            get => _keszletenVan;
            set
            {
                if (_keszletenVan != value)
                {
                    _keszletenVan = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _utolsoRendelesDatum = DateTime.Today;
        public DateTime UtolsoRendelesDatum
        {
            get => _utolsoRendelesDatum;
            set
            {
                if (_utolsoRendelesDatum != value)
                {
                    _utolsoRendelesDatum = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}