using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsDesk.Classes;
using WmsDesk.Converter;

namespace WmsDesk.ViewModels
{
    public class AddItemViewModel : INotifyPropertyChanged
    {
        public string _tbText = string.Empty;
        public ObservableCollection<IncomeItemVm> _items = new ObservableCollection<IncomeItemVm>();

        private IncomeItemVm _selectedItem;
        public IncomeItemVm SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        public ObservableCollection<IncomeItemVm>  Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));// Добавить применение фильтром и прочее

            }
        }
        public Filter Filter { get; set; } = new Filter(new List<IncomeItemEntity>(), new List<Barcode>());
        public string TbText {  
            get
            {
                return _tbText;
            }
            set 
            {
                _tbText = value;
                Filter.Sort = value;
                Items = Filter.Apply();
                OnPropertyChanged(nameof(TbText));// Добавить применение фильтром и прочее
                OnPropertyChanged(nameof(Items));// Добавить применение фильтром и прочее

            }
        }

        public AddItemViewModel(ObservableCollection<IncomeItemVm> items, List<IncomeItemEntity> catalogData)
        {
            Items = items;
            Filter.Items = items.ToEntityList(catalogData);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
