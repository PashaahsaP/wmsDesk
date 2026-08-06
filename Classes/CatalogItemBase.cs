using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsDesk
{
    public class CatalogItemBase
    {
        public string CatalogId {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int SupplierId { get; set; } = 0;
        public List<Barcode> BarcodeList { get; set; } = new List<Barcode>();
        public string Barcode { get; set; } = string.Empty;
        public string Other {  get; set; } = string.Empty;
        public override string ToString()
        {
            return Name;
        }

    }
    public class WithDate : CatalogItemBase { 
        public DateTime Date { get; set; }
    }
    public class WithBatch : CatalogItemBase, INotifyPropertyChanged
    {
        private List<Batch> _batches = new List<Batch>(); 
        private ObservableCollection<Batch> _visible = new ObservableCollection<Batch>();
        private string _searchLine = string.Empty;
        public List<Batch> Batches { 
            get
            {
                return _batches;
            } 
            set
            {
                _batches = value;
                _visible = new ObservableCollection<Batch>(value);
                OnPropertyChanged(nameof(Batches));
                OnPropertyChanged(nameof(VisibleBatches));
            }

        }
        public ObservableCollection<Batch> VisibleBatches
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
                OnPropertyChanged(nameof(Batches));
            }

        }
        public string SearchLine { get
            {
                return _searchLine;
            }
            set
            {
                _searchLine = value;
                OnPropertyChanged(nameof(SearchLine));
                _visible = new ObservableCollection<Batch> (_batches.Where(item => item.Name.Contains(SearchLine)).ToList());
                OnPropertyChanged(nameof(VisibleBatches));
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
