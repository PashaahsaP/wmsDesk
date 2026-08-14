using ExcelFileParser;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WmsDesk.Classes;
using WmsDesk.Converter;
using WmsDesk.Enums;
using WmsDesk.Windows;


namespace WmsDesk.ViewModels
{
    internal class CreateIncomeSessionViewModel : INotifyPropertyChanged, IState
    {
        public Filter Filter { get; set; } = new Filter(new List<IncomeItemEntity>(), new List<Barcode>());
        private Window _window;
        private int _supplier;
        private static readonly Client client = new Client();
        private string _tbText = "";
        private bool _isSupplierSelected = false;
        private ObservableCollection<IncomeItemVm> _borkItems = new ObservableCollection<IncomeItemVm>();
        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        private ObservableCollection<IncomeItemVm> _items;
        private Supplier? _selectedSupplier;
        private Cell? _selectedCell;

        private DateTime? _date = new DateTime?();

        private MainViewModel vm;

        public string TbText
        {
            get
            {
                return _tbText;
            }
            set
            {
                _tbText = value;
                Filter.Sort = _tbText;
                _borkItems = Filter.Apply();
                OnPropertyChanged(nameof(TbText));
                OnPropertyChanged(nameof(CatalogItems));

            }
        }
        public int Supplier { get => _supplier; set { _supplier = value; } }
        public ObservableCollection<IncomeItemVm> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        public List<CatalogItemBase> CatalogBorkItems { get; set; }
        public List<Batch> Batches { get; set; } = new List<Batch>();
        public List<CellTypes> ParsedCellTypes { get; set; } = new List<CellTypes>();
        public ObservableCollection<IncomeItemVm> CatalogItems
        {
            get
            {
                return _borkItems;
            }
            set
            {
                _borkItems = value;
                OnPropertyChanged(nameof(CatalogItems));
            }
        }
        public List<IncomeItemEntity> CatalogData { get; set; } = new List<IncomeItemEntity>();
        public ObservableCollection<Supplier> Suppliers
        {
            get
            {
                return _suppliers;
            }
            set
            {
                _suppliers = value;
                OnPropertyChanged(nameof(Suppliers));
            }
        }
        public Supplier? SelectedSupplier
        {
            get
            {
                return _selectedSupplier;

            }
            set
            {
                if (value == null)
                {
                    _selectedSupplier = value;
                    OnPropertyChanged(nameof(SelectedSupplier));
                    return;
                }
                _selectedSupplier = value;
                var isEnabled = Suppliers.Any(item =>
                    item.Name == value.Name
                );
                if (isEnabled)
                {
                    var selectedItems = CatalogData.Where(item => item.SupplierId == value.Id).ToList();
                    CatalogItems = new ObservableCollection<IncomeItemVm>(selectedItems.ToVmList());
                }
                IsSupplierSelected = isEnabled;
                OnPropertyChanged(nameof(SelectedSupplier));
            }
        }
        public IncomeItemEntity SelectedCatalogItem { get; set; }
        public Cell SelectedCell { 
            get
            {
                return _selectedCell;
            }
            set
            {
                if (value == null)
                {
                    _selectedCell = value;
                    OnPropertyChanged(nameof(SelectedCell));
                    return;
                }
                _selectedCell = value;
                OnPropertyChanged(nameof(SelectedCell));
            }
        }
        public DateTime? Date
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
        public List<Cell> IncomeCells { get; set; } = new List<Cell>();
        public List<Cell> Cells { get; set; } = new List<Cell>();
        public List<Barcode> Barcodes { get; set; } = new List<Barcode>();
        public PageStates PageState => PageStates.CreateIncomeSessionPage;
        public bool IsSupplierSelected
        {
            get
            {
                return _isSupplierSelected;
            }
            set
            {
                _isSupplierSelected = value;
                OnPropertyChanged(nameof(IsSupplierSelected));
            }
        }




        public ICommand callBorkDialog { get; set; }
        public ICommand clearItems { get; set; }
        public ICommand createSession { get; set; }
        public ICommand selectBork { get; set; }
        public ICommand selectAtomy { get; set; }
        public ICommand loadFile { get; set; }
        public ICommand removeLine { get; set; }
        public ICommand pressEnterInTb { get; set; }



        public CreateIncomeSessionViewModel(string catalogAndSuppliers, string suppliers, string barcodes, string incomeCells, string batches, string cellTypes, Window window)
        {
            _window = window;
            Items = new ObservableCollection<IncomeItemVm>();
            selectAtomy = new RelayCommand(o =>
            {
                _supplier = 0;
            });
            selectBork = new RelayCommand(o =>
            {
                _supplier = 1;
            });
            callBorkDialog = new RelayCommand(o =>
            {
                var dialog = new DialogWindow(o, CatalogBorkItems, Items);
                //dialog.Owner = window;
                dialog.listItems.ItemsSource = CatalogBorkItems;
                dialog.Show();
            }, c =>
            {
                var isBork = c.GetType().GetProperty("TE") == null;

                if (isBork)
                {
                    var catalogExist = (BorkItem)c;
                    if (catalogExist.Catalog == null)
                        return true;
                }
                return false;
            });
            clearItems = new RelayCommand(o =>
            {
                Items = new ObservableCollection<IncomeItemVm>();
            });
            createSession = new RelayCommand(async o =>
            {
                // Проверить что все элементы валидные
                bool isOkElements = Items.All(inner => inner.isValid);
                // Проверить что ячейка выбрана для приемки
                bool isSelectedCell = SelectedCell != null;
                bool isTeOK = Items.All(item =>
                {
                    if (item.TE == "")
                        return true;
                    if(GetCellType(item.TE, ParsedCellTypes) != null)
                        return true;
                    return false;
                }
                );
                if (!isOkElements)
                {
                    MessageBox.Show("Не все элементы валидны");
                }
                if (!isSelectedCell)
                {
                    MessageBox.Show("Ячейка не выбрана");
                }
                if (!isTeOK)
                {
                    foreach (var item in Items)
                    {
                        if(item.TE != "" && GetCellType(item.TE, ParsedCellTypes) == null)
                        {
                            MessageBox.Show($"Транспортной единицы {item.TE} не существует.");
                        }
                    }
                }


                if (isSelectedCell && isOkElements && isTeOK)
                {
                    // Найти нужную ячейку приемки
                    var jsonIp = File.ReadAllText("config.json");
                    var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
                    var ip = setting["Ip"];
                    var cell = await client.GetCellIdByName(SelectedCell.Name, ip);
                    // Создать заявку на приемку
                    var session = new IncomeSession()
                    {
                        SupplierId = SelectedSupplier.Id,
                        IncomeCellId = cell.Id,
                        Status = (int)StatusType.Created,
                        CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        IsDeleted = false
                    };
                    var sessionId = await client.SendIncomeSession(session, ip);
                    Console.WriteLine(sessionId);
                    // Создать goods в ячейке на приемку
                    var goodsTasks = Items.Select(async inner =>
                    {
                        var resultCellId = cell.Id;

                        // Проверить есть ли в бд такая ячейка
                        var checkCell = await client.GetCellIdByName(inner.TE, ip);
                        CellTypes cellType = null;
                        if (checkCell != null && checkCell is Cell)
                        {
                            resultCellId = checkCell.Id;
                        }
                        else
                        {
                            // Иначе надо определить какой тип те используется
                            cellType = GetCellType(inner.TE, ParsedCellTypes);
                            // Сгенерировать id  и отправить на сервер и новый id использовать
                        }


                        if (!string.IsNullOrWhiteSpace(inner.TE) && cellType != null)
                        {
                            var teResult = await client.SendTE(inner.TE, cell.Id, cellType.Id, ip);
                            resultCellId = teResult.id;
                        }

                        return new Goods()
                        {
                            amount = inner.Count,
                            catalogId = inner.CatalogId,
                            cellId = resultCellId,
                            isDeleted = false,
                            createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            updatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        };
                    });

                    // 2. Дожидаемся выполнения ВСЕХ задач. Теперь в `resultGoods` лежат чистые объекты Goods, а не Task!
                    Goods[] resultGoods = await Task.WhenAll(goodsTasks);

                    var incomeItems = new List<IncomeItem>();

                    // 3. Спокойно и безопасно отправляем товары на сервер через чистый await
                    foreach (var item in resultGoods)
                    {
                        var idResult = await client.SendGoods(item, ip);
                        item.id = idResult.id; // Теперь это свойство самого объекта Goods, а не .Result

                        incomeItems.Add(new IncomeItem()
                        {
                            GoodsId = item.id,
                            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            IsDeleted = false,
                            SessionId = sessionId.id
                        });
                    }
                    var result = new List<StringID>();
                    foreach (var item in incomeItems)
                    {
                        result.Add( await client.SendIncomeItem(item, ip));
                    }
                    if (result.All(item => item != null))
                    {
                        MessageBox.Show("Заявка создана");
                        Items.Clear();
                        SelectedSupplier = null;
                        SelectedCell = null;
                        Date = null;
                    }
                    // Создать IncomeItem в ячейке на приемку
                }
            });
            loadFile = new RelayCommand(async o =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
                // dialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    string path = dialog.FileName;
                    FileReader reader = new FileReader(path, Suppliers.Select(item => (
                    new ExcelFileParser.Supplier()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        SupplierType = item.SupplierType,
                    },
                    item == SelectedSupplier)).ToList());
                    // проверять тип файла и запускать нужное окно
                    var innerDialog = new CreateSessionByExcelFile(new CreateSessionByExcelFileViewModel(reader.filesInfo[0].FileType), reader);
                    innerDialog.Owner = _window;

                    bool? innerResult = innerDialog.ShowDialog();
                    if (innerResult == true)
                    {
                        var isValid = false;
                        // надо для каждого элемента получить каталог id и дальше...)
                        foreach (var item in innerDialog.Result)
                        {
                            isValid = false;
                            if (Items.Any(inner => inner.Sku == item.Sku))
                            {
                                isValid = true;
                                item.CatalogId = Items.FirstOrDefault(inner => inner.Sku == item.Sku).CatalogId;
                            }
                            if (Barcodes.Any(inner => inner.Name == item.Name))
                            {
                                isValid = true;
                                item.CatalogId = Barcodes.FirstOrDefault(inner => inner.Name == item.Barcode).CatalogId;
                            }
                            item.isValid = isValid;
                        }
                        // надо проверить по артикулу и шк в бд
                        Items = new ObservableCollection<IncomeItemVm>(innerDialog.Result);
                        var updateCollection = new ObservableCollection<IncomeItemVm>();
                        foreach (var item in innerDialog.Result)
                        {
                            if ((item.Sku != "" && CatalogData.Any(inner => inner.Sku == item.Sku))
                            || (Barcodes.Any(bar => bar.Name == item.Barcode) && item.Barcode != ""))
                            {
                                updateCollection.Add(item);
                            }
                            else
                            {
                                updateCollection.Add(new WrongItemVm()
                                {
                                    Barcode = item.Barcode,
                                    CatalogId = item.CatalogId,
                                    Count = item.Count,
                                    isSelected = item.isSelected,
                                    isValid = item.isValid,
                                    Name = item.Name,
                                    Other = item.Other,
                                    Sku = item.Sku,
                                    TE = item.TE,
                                    Date = item is IncomeItemWithDateVm ? (item as IncomeItemWithDateVm).Date : DateTime.Now,
                                    Batches = item is IncomeItemWithBatchVm ? (item as IncomeItemWithBatchVm).Batches : ""
                                }
                                );
                            }
                        }
                        Items = updateCollection;
                    }
                }
            });
            removeLine = new RelayCommand(async o =>
            {
                Items.Remove(o as IncomeItemVm);
            });
            pressEnterInTb = new RelayCommand(async o =>
            {

                var result = new List<IncomeItemVm>();
                var element = Items.FirstOrDefault(inner => inner.isSelected);
                var temprary = new IncomeItemVm();
                foreach (var item in Items)
                {
                    if (item == element)
                    {
                        if (element is IncomeItemWithDateVm)
                        {
                            temprary = new IncomeItemWithDateVm()
                            {
                                Count = element.Count,
                                Sku = element.Sku,
                                Name = element.Name,
                                isValid = element.isValid,
                                TE = element.TE,
                                CatalogId = element.CatalogId,
                                Date = ((IncomeItemWithDateVm)element).Date,
                                isSelected = element.isSelected,
                                Other = element.Other
                            };

                        }
                        else if (element is IncomeItemWithBatchVm)
                        {
                            temprary = new IncomeItemWithBatchVm()
                            {
                                Count = element.Count,
                                Sku = element.Sku,
                                Name = element.Name,
                                isValid = element.isValid,
                                TE = element.TE,
                                CatalogId = element.CatalogId,
                                Batches = ((IncomeItemWithBatchVm)element).Batches,
                                isSelected = element.isSelected,
                                Other = element.Other
                            };
                        }
                        else if (element is WrongItemVm)
                        {
                            temprary = new WrongItemVm()
                            {
                                Count = element.Count,
                                Sku = element.Sku,
                                Name = element.Name,
                                isValid = element.isValid,
                                TE = element.TE,
                                CatalogId = element.CatalogId,
                                Batches = ((WrongItemVm)element).Batches,
                                isSelected = element.isSelected,
                                Other = element.Other
                            };
                        }
                        else
                        {
                            temprary = new IncomeItemVm()
                            {
                                Count = element.Count,
                                Sku = element.Sku,
                                Name = element.Name,
                                isValid = element.isValid,
                                TE = element.TE,
                                CatalogId = element.CatalogId
                            };
                        }

                        result.Add(temprary);
                    }
                    else
                    {
                        // var temp = new IncomeItemVm() { Count = item.Count, Sku = item.Sku, Name = item.Name, isValid = item.isValid, TE = item.TE, CatalogId = item.CatalogId };
                        result.Add(item);
                    }
                }

                Items = new ObservableCollection<IncomeItemVm>(result);

            });
            //parse suppliers
            var supplierData = JsonConvert.DeserializeObject<ObservableCollection<Supplier>>(suppliers);
            foreach (var item in supplierData)
            { Suppliers.Add(item); }

            //parse catalogs
            // make switch case for client types
            // creating income session items for each type
            // var temp = new IncomeBaseItem();
            //parse cells
            var parsedIncomeCells = JsonConvert.DeserializeObject<List<Cell>>(incomeCells);
            foreach (var item in parsedIncomeCells)
            {
                Cells.Add(item);

            }

            Filter.Cells = Cells;
            //parse cellTypes
            ParsedCellTypes = JsonConvert.DeserializeObject<List<CellTypes>>(cellTypes);
            var parsedData = JsonConvert.DeserializeObject<ObservableCollection<IncomeItemEntity>>(catalogAndSuppliers);
            IncomeItemEntity temp = new IncomeItemEntity();

            foreach (var item in parsedData)
            {

                var sup = Suppliers.FirstOrDefault(inner => inner.Id == item.SupplierId);
                if (Enum.IsDefined(typeof(ClientType), sup.SupplierType))
                {
                    ClientType currentStatus = (ClientType)sup.SupplierType;

                    switch (currentStatus)
                    {
                        case ClientType.Base:
                            temp = new IncomeItemEntity()
                            {
                                Name = item.Name,
                                Sku = item.Sku,
                                CatalogId = item.CatalogId,
                                SupplierId = sup.Id,
                                SupplierName = sup.Name,
                                Other = item.Other
                            };
                            break;
                        case ClientType.WithDate:
                            temp = new IncomeItemWithDateEntity()
                            {
                                Name = item.Name,
                                Sku = item.Sku,
                                CatalogId = item.CatalogId,
                                SupplierId = sup.Id,
                                SupplierName = sup.Name,
                                Other = item.Other,
                                Date = "10.10.2024"
                            };
                            break;
                        case ClientType.WithBatch:
                            temp = new IncomeItemWithBatchEntity()
                            {
                                Name = item.Name,
                                Sku = item.Sku,
                                CatalogId = item.CatalogId,
                                SupplierId = sup.Id,
                                SupplierName = sup.Name,
                                Other = item.Other,
                                Batches = "234"
                            };
                            break;
                    }
                }

                CatalogData.Add(temp);
                Items.Add(temp.ToVm());
                CatalogItems.Add(temp.ToVm());

            }
            Filter.Items = parsedData.ToList();


            //parse barcodes
            var parsedBarcodes = JsonConvert.DeserializeObject<ObservableCollection<Barcode>>(barcodes);
            foreach (var item in parsedBarcodes)
            {
                Barcodes.Add(item);

            }

            //parse batches
            var parsedBatches = JsonConvert.DeserializeObject<ObservableCollection<Batch>>(batches);
            foreach (var item in parsedBatches)
            {
                Batches.Add(item);
            }

        }

        private bool IsTE(Cell cell, List<CellTypes> list)
        {
            var t = list.Any(cellType =>
            {
                var mask = cellType.Mask;
                if (mask == null)
                    return false;

                return mask.Length == cell.Name.Length &&
                       mask.Select((c, i) => new { MaskChar = c, Index = i })
                           .All(x =>
                           {
                               switch (x.MaskChar)
                               {
                                   case '#':
                                       return char.IsDigit(cell.Name[x.Index]);
                                   case '*':
                                       return char.IsLetter(cell.Name[x.Index]);
                                   default:
                                       return x.MaskChar == cell.Name[x.Index];
                               }
                           });
            });

            return t;
        }
        private CellTypes GetCellType(string name, List<CellTypes> list)
        {
            var t = list.FirstOrDefault(cellType =>
            {
                var mask = cellType.Mask;
                if (mask == null)
                    return false;

                return mask.Length == name.Length &&
                       mask.Select((c, i) => new { MaskChar = c, Index = i })
                           .All(x =>
                           {
                               switch (x.MaskChar)
                               {
                                   case '#':
                                       return char.IsDigit(name[x.Index]);
                                   case '*':
                                       return char.IsLetter(name[x.Index]);
                                   default:
                                       return x.MaskChar == name[x.Index];
                               }
                           });
            });

            return t;
        }

        public static async Task<CreateIncomeSessionViewModel> CreateAsync(Window window)
        {
            var jsonIp = File.ReadAllText("config.json");
            var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
            var ip = setting["Ip"];
            var catalogAndSuppliers = await client.GetAllCatalogsWithSuppliers(ip);
            var suppliers = await client.GetSuppliers(ip);
            var batches = await client.GetBatches(ip);
            var barcodes = await client.GetBarcodes(ip);
            var incomeCells = await client.GetIncomeCells(ip);
            var cellTypes = await client.GetCellTypes(ip);
            return new CreateIncomeSessionViewModel(catalogAndSuppliers, suppliers, barcodes, incomeCells, batches, cellTypes, window);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

