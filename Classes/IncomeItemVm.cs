using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsDesk
{
    public class IncomeItemVm : INotifyPropertyChanged
    {
        string _catalogId = string.Empty;
        private string _sku = string.Empty;
        private string _te = string.Empty;
        private string _name = string.Empty;
        private string _other = string.Empty;
        private int _count = 1;
        private bool _isValid = true;
        private bool _isSelected = false;


        public string CatalogId 
        {
            get
            {
                return _catalogId;
            }
            set
            {
                _catalogId = value;
                OnPropertyChanged(nameof(CatalogId));
            }
        }
        public string Sku
        {
            get => _sku;
            set
            {
                if (_sku != value)
                {
                    _sku = value;
                    OnPropertyChanged(nameof(Sku));
                }
            }
        }
        public string TE
        {
            get => _te;
            set
            {
                if (_te != value)
                {
                    _te = value;
                    OnPropertyChanged(nameof(TE));
                }
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string Other
        {
            get => _other;
            set
            {
                if (_other != value)
                {
                    _other = value;
                    OnPropertyChanged(nameof(Other));
                }
            }
        }
        public int Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
        public bool isValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(isValid));
                }
            }
        }
        public bool isSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(isSelected));
                }
            }
        }
        public string Barcode { get; set; } = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return Name;
        }
        public void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class IncomeItemWithDateVm: IncomeItemVm 
    {
        DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
    }
    public class IncomeItemWithBatchVm: IncomeItemVm 
    {
        string _batch = string.Empty;
        public string Batches 
            {
                get
                {
                    return _batch;
                }
                set
                { 
                    _batch = value;
                    OnPropertyChanged(Batches);
                }
            }
    }
    public class SelectedIncomeItemVm : IncomeItemVm {}
    public class SelectedIncomeItemWithDateVm: IncomeItemWithDateVm { }
    public class SelectedIncomeItemWithBatchVM: IncomeItemWithBatchVm { }
    public class WrongItemVm : IncomeItemVm
    {
        public DateTime Date { get; set; }
        public string Batches {  get; set; }
    }  
}
