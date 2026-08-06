using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsDesk
{
     
    public class DateOfSupplier
    {
        public long Date { get; set; }
    }
    public class Supplier
    {
        public int Id { get; set; }
        public int SupplierType { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }
    public class OrderItem : INotifyPropertyChanged     
    {
        public string _id;
        public string _supplierId;
        public string _name;
        public string _supplierName;
        public string _sku;
        public string _other;
        public string id { get => _id; set 
            { 
                _id = value;
                OnPropertyChanged(nameof(id));
            }
        }
        public string supplierId
        {
            get => _supplierId; set
            {
                _supplierId = value;
                OnPropertyChanged(nameof(supplierId));
            }
        }
        public string name
        {
            get => _name; set
            {
                _name = value;
                OnPropertyChanged(nameof(name));
            }
        }
        public string supplierName
        {
            get => _supplierName; set
            {
                _supplierName = value;
                OnPropertyChanged(nameof(supplierName));
            }
        }
        public string sku
        {
            get => _sku; set
            {
                _sku = value;
                OnPropertyChanged(nameof(sku));
            }
        }
        public string other
        {
            get => _other; set
            {
                _other = value;
                OnPropertyChanged(nameof(other));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString() => name;
    }
    /// <summary>
    /// Интерфейс для ui элементов
    /// </summary>
    /// <param name="Count">Сколько нужно для заявки</param>
    /// <param name="Amount">Сколько есть Фактически</param>
    public interface IUiItem
    {
        string Left { get;  }
        string Center { get;  }
        string Right { get;  }

        string Name { get; set; }
        int Count { get; set; }
        int Amount { get; set; }
        OrderItem Catalog {  get; set; }

    }
}
