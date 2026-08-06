using ExcelFileParser;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using WmsDesk.Windows;

namespace WmsDesk
{
    public class AddingExcelFile : INotifyPropertyChanged//view class
    {
        private string _fieldName = "";
        private List<Tuple<string, int>> _fileField = new List<Tuple<string, int>>();
        private Tuple<string, int> _selectedItem;
        public string FieldName
        {
            get => _fieldName;
            set
            {
                if (_fieldName != value)
                {
                    _fieldName = value;
                    OnPropertyChanged(nameof(FieldName));
                }
            }
        }
        public List<Tuple<string, int>> FileField
        {
            get => _fileField;
            set
            {
                if (_fileField != value)
                {
                    _fileField = value;
                    OnPropertyChanged(nameof(FileField));
                }
            }
        }
        public Tuple<string, int> SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!EqualityComparer<Tuple<string, int>>.Default.Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class CreateSessionByExcelFileViewModel : INotifyPropertyChanged
    {
        #region var
        private ObservableCollection<AddingExcelFile> _data = new ObservableCollection<AddingExcelFile>();
        private string _fileType;
        private List<string> _tablesList = new List<string>() { };
        private string _selectedTable;
        #endregion
        #region prop
        public ObservableCollection<AddingExcelFile> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }

        public List<string> TablesList
        {
            get => _tablesList;
            set
            {
                if (_tablesList != value)
                {
                    _tablesList = value;
                    OnPropertyChanged(nameof(TablesList)); // Уведомляет UI о замене всего списка
                }
            }
        }

        public string SelectedTable
        {
            get => _selectedTable;
            set
            {
                if (_selectedTable != value)
                {
                    _selectedTable = value;
                    OnPropertyChanged(nameof(SelectedTable));
                    //обновление столбцов для каждого элемента
                    var count = 0;
                    for (int i = 0; i < TablesList.Count; i++)
                    {
                        if (TablesList[i] == value)
                            count = i;
                    }
                    _data.Clear();
                    foreach (var item in Reader.filesInfo[count].SessionField)
                    {

                        _data.Add(new AddingExcelFile() { FieldName = item, FileField = Reader.filesInfo[count].FileField });
                    }
                    OnPropertyChanged(nameof(Data));

                }
            }
        }
        public string FileType
        {
            get => _fileType;
            set
            {
                if (_fileType != value)
                {
                    _fileType = value;
                    OnPropertyChanged(nameof(FileType)); // Оповещаем интерфейс об изменении
                }
            }
        }
        public FileReader Reader { get; set; }
        public CreateSessionByExcelFile Dialog { get; set; }
        #endregion
        #region command
        public ICommand parseData { get; set; }
        #endregion
        #region ctor
        public CreateSessionByExcelFileViewModel(string fileType)
        {
            FileType = fileType;
            parseData = new RelayCommand(o =>
            {
                var selectedSupplier = Reader.filesInfo[0].Suppliers.FirstOrDefault(inner => inner.Item2 == true);
                var result = new List<IncomeItemVm>();
                var count = 0;
                if (TablesList != null)
                {
                    for (int i = 0; i < TablesList.Count; i++)
                    {
                        if (TablesList[i] == SelectedTable)
                            count = i;
                    }
                }
                Reader.filesInfo[count].Data.RemoveAt(0);
                //

                //
                foreach (var line in Reader.filesInfo[count].Data)
                {
                    if (selectedSupplier.Item1.SupplierType == 1)
                    {
                        var newDate = line[Data.FirstOrDefault(item => item.FieldName == "date").SelectedItem.Item2];
                        result.Add(new IncomeItemWithDateVm()
                        {
                            CatalogId = "",
                            Barcode = Data.FirstOrDefault(item => item.FieldName == "barcode")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "barcode").SelectedItem.Item2] : "",
                            Count = Data.FirstOrDefault(item => item.FieldName == "count")?.SelectedItem != null ? int.Parse(line[Data.FirstOrDefault(item => item.FieldName == "count").SelectedItem.Item2]) : 0,
                            Date = DateTime.ParseExact(newDate, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                            Name = Data.FirstOrDefault(item => item.FieldName == "name")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "name").SelectedItem.Item2] : "",
                            Sku = Data.FirstOrDefault(item => item.FieldName == "sku")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "sku").SelectedItem.Item2] : "",
                            TE = Data.FirstOrDefault(item => item.FieldName == "te")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "te").SelectedItem.Item2] : "",
                        }
                        );
                    }
                    else if (selectedSupplier.Item1.SupplierType == 0)
                    {
                        result.Add(new IncomeItemVm()
                        {
                            CatalogId = "",
                            Barcode = Data.FirstOrDefault(item => item.FieldName == "barcode")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "barcode").SelectedItem.Item2] : "",
                            Count = Data.FirstOrDefault(item => item.FieldName == "count")?.SelectedItem != null ? int.Parse(line[Data.FirstOrDefault(item => item.FieldName == "count").SelectedItem.Item2]) : 0,
                            Name = Data.FirstOrDefault(item => item.FieldName == "name")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "name").SelectedItem.Item2] : "",
                            Sku = Data.FirstOrDefault(item => item.FieldName == "sku")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "sku").SelectedItem.Item2] : "",
                            TE = Data.FirstOrDefault(item => item.FieldName == "te")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "te").SelectedItem.Item2] : "",
                        }
                        );
                    }
                    else if (selectedSupplier.Item1.SupplierType == 2)
                    {
                        result.Add(new IncomeItemWithBatchVm()
                        {
                            CatalogId = "",
                            Barcode = Data.FirstOrDefault(item => item.FieldName == "barcode")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "barcode").SelectedItem.Item2] : "",
                            Count = Data.FirstOrDefault(item => item.FieldName == "count")?.SelectedItem != null ? int.Parse(line[Data.FirstOrDefault(item => item.FieldName == "count").SelectedItem.Item2]) : 0,
                            Batches = Data.FirstOrDefault(item => item.FieldName == "batch")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "batch").SelectedItem.Item2] : "",
                            Name = Data.FirstOrDefault(item => item.FieldName == "name")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "name").SelectedItem.Item2] : "",
                            Sku = Data.FirstOrDefault(item => item.FieldName == "sku")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "sku").SelectedItem.Item2] : "",
                            TE = Data.FirstOrDefault(item => item.FieldName == "te")?.SelectedItem != null ? line[Data.FirstOrDefault(item => item.FieldName == "te").SelectedItem.Item2] : "",
                        }
                        );
                    }

                }
                Dialog.Result = result;
                Dialog.DialogResult = true;
            });
        }
        #endregion
        #region OnPropChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}
