using ExcelFileParser;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
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
    public class SessionItem
    {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string GoodsId { get; set; }
        public string CellId { get; set; }
        public int Status { get; set; }
        public long StartedAt { get; set; }
        public long FinishedAt { get; set; }
        public long CreatedAt { get; set; }

        // Конструктор для инициализации всех полей
        public SessionItem(
            string id,
            string sessionId,
            string goodsId,
            string cellId,
            int status,
            long startedAt,
            long finishedAt,
            long createdAt)
        {
            Id = id;
            SessionId = sessionId;
            GoodsId = goodsId;
            CellId = cellId;
            Status = status;
            StartedAt = startedAt;
            FinishedAt = finishedAt;
            CreatedAt = createdAt;
        }
    }

    internal class CreateAssemblySessionViewModel : INotifyPropertyChanged, IState
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
        private Supplier _selectedSupplier;
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
        public Supplier SelectedSupplier
        {
            get
            {
                return _selectedSupplier;
            }
            set
            {
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
        public Cell SelectedCell { get; set; }
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
        public List<Cell> AllCells { get; set; } = new List<Cell>();
        public List<Barcode> Barcodes { get; set; } = new List<Barcode>();
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
        public PageStates PageState => PageStates.CreateAssemblySessionPage;




        public ICommand callBorkDialog { get; set; }
        public ICommand clearItems { get; set; }
        public ICommand createSession { get; set; }
        public ICommand selectBork { get; set; }
        public ICommand selectAtomy { get; set; }
        public ICommand loadFile { get; set; }
        public ICommand removeLine { get; set; }
        public ICommand pressEnterInTb { get; set; }



        public CreateAssemblySessionViewModel(string catalogAndSuppliers, string suppliers, string barcodes, string outcomeCells, string batches, string cellTypes, Window window, string cells)
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
                var jsonIp = File.ReadAllText("config.json");
                var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
                var ip = setting["Ip"];
                var goods = await client.GetAllGoods(ip);
                var parsedGoods = JsonConvert.DeserializeObject<List<Goods>>(goods).Where(item => 
                item.isAvailable == true
                        && !(AllCells.First(cell => cell.Id == item.cellId)).Name.Contains("IN")).ToList();
                // Проверить что все элементы валидные
                bool isOkElements = Items.All(inner => inner.isValid);
                // Проверить что ячейка выбрана для приемки
                bool isSelectedCell = SelectedCell != null;
                // Проверить есть ли указанные те в бд, Проверить есть ли товары на 
                bool balanceIsOk = true;
                var result = new List<string>();
                foreach (var item in Items)
                {
                    var te = item.TE != "" ? await client.GetCellIdByName(item.TE, ip) : null;
                    if (item.TE != "")
                    {
                        //TODO как будто ошибка в плане условия и 
                        if (te != null)
                        {
                            var innerGoods = parsedGoods.Where(inner => inner.catalogId == item.CatalogId && inner.cellId == te.Id).First();
                            if (innerGoods.amount < item.Count)
                            {
                                result.Add($"{item.Name} товар в наличии {innerGoods.amount}, а фактически требуется {item.Count}");
                                balanceIsOk = false;
                            }
                        }
                        else
                        {
                            result.Add($"Транспортной единицы {item.TE} не существует в бд");
                            balanceIsOk = false;

                        }
                    }
                    else
                    {
                        var totalAmount = parsedGoods.Where(inner => 
                        inner.catalogId == item.CatalogId ).Sum(innerGoodsItem => innerGoodsItem.amount);
                        if (totalAmount < item.Count)
                        {
                            balanceIsOk = false;
                            result.Add($"{item.Name} товар в наличии {totalAmount}, а фактически требуется {item.Count}");
                        }
                    }

                }
                if (!isOkElements)
                {
                    MessageBox.Show("Не все элементы валидны");
                }
                if (!isSelectedCell)
                {
                    MessageBox.Show("Ячейка не выбрана");
                }
                if (!balanceIsOk)
                {
                    foreach (var message in result)
                    {
                        MessageBox.Show(message);
                    }
                }
                if (isSelectedCell && isOkElements && balanceIsOk)
                {

                    var sessionForRequest = new AssemblySession();
                    var goodsForRequest = new List<Goods>();
                    var itemsForRequest = new List<SessionItem>();
                    // Найти нужную ячейку отгрузки
                    var cell = await client.GetCellIdByName(SelectedCell.Name, ip);

                    // Создать заявку на сборку
                    // Создать assembly item, отделить goods и заблокировать

                    try
                    {
                        // 1. Создаем заявку на сборку
                        sessionForRequest = new AssemblySession
                        {
                            id = "",
                            supplierId = SelectedSupplier.Id,
                            lines = Items.Count,
                            createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                            date = Date.ToString(),
                            outCell = SelectedCell.Id,
                            amount = 0,
                            status = 0
                        };
                        foreach (var collItem in Items)
                        {
                            // Находим исходный товар на остатке в ячейке
                            if (collItem.TE != "")
                            {
                                var te = await client.GetCellIdByName(collItem.TE, ip);
                                var goodsInTE = parsedGoods.First(inner => inner.cellId == te.Id);
                                if (goodsInTE.amount == collItem.Count)
                                {
                                    //обновить goods и создать assembly item
                                    var newGoods = goodsInTE.CloneGoods();
                                    newGoods.isAvailable = false;//TODO передать в client
                                    var newAssemblyItem = new SessionItem(
                                        "",
                                        sessionForRequest.id,
                                        goodsInTE.id,
                                        goodsInTE.cellId,
                                        (int)StatusType.Created,
                                        0,
                                        0,
                                        DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                    goodsForRequest.Add(newGoods);
                                    itemsForRequest.Add(newAssemblyItem);
                                    parsedGoods.Remove(goodsInTE);//local update
                                }
                                else
                                {
                                    var prevGoods = goodsInTE.CloneGoods();
                                    var newGoods = goodsInTE.CloneGoods();
                                    prevGoods.amount = prevGoods.amount - collItem.Count;
                                    newGoods.isAvailable = false;
                                    newGoods.id = "";
                                    newGoods.amount = collItem.Count;
                                    var newAssemblyItem = new SessionItem(
                                        "",
                                        sessionForRequest.id,
                                        newGoods.id,
                                        newGoods.cellId,
                                        (int)StatusType.Created,
                                        0,
                                        0,
                                        DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                    goodsForRequest.Add(prevGoods);
                                    goodsForRequest.Add(newGoods);
                                    itemsForRequest.Add(newAssemblyItem);
                                    goodsInTE.amount = goodsInTE.amount - collItem.Count;//local update
                                }
                            }
                            else
                            {
                                //найти goods и разделить если надо количество
                                var equalItem = parsedGoods.First(item => item.amount == collItem.Count);
                                if (equalItem != null)
                                {
                                    //обновить goods и создать assembly item
                                    var newGoods = equalItem.CloneGoods();
                                    newGoods.isAvailable = false;//TODO передать в client
                                    var newAssemblyItem = new SessionItem(
                                        "",
                                        sessionForRequest.id,
                                        equalItem.id,
                                        equalItem.cellId,
                                        (int)StatusType.Created,
                                        0,
                                        0,
                                        DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                    goodsForRequest.Add(newGoods);
                                    itemsForRequest.Add(newAssemblyItem);
                                    parsedGoods.Remove(equalItem);
                                }
                                else
                                {
                                    var localCounter = collItem.Count;
                                    //надо брать в которых количество меньше и уменьшать остаток
                                    foreach (var item in parsedGoods.OrderBy(inner => inner.amount))
                                    {
                                        if (item.amount < localCounter)
                                        {
                                            // goods текущий обновить и создать assemblyItem
                                            //обновить goods и создать assembly item
                                            var newGoods = item.CloneGoods();
                                            newGoods.isAvailable = false;//TODO передать в client
                                            var newAssemblyItem = new SessionItem(
                                                "",
                                                sessionForRequest.id,
                                                newGoods.id,
                                                newGoods.cellId,
                                                (int)StatusType.Created,
                                                0,
                                                0,
                                                DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                            goodsForRequest.Add(newGoods);
                                            itemsForRequest.Add(newAssemblyItem);
                                            localCounter -= item.amount;
                                            parsedGoods.Remove(item);
                                        }
                                        else
                                        {
                                            break;
                                        }
                                        if (localCounter == 0)
                                            break;
                                    }
                                    //если нет элементов у которых есть меньшее количество то элемент с большим количеством и его изменить
                                    Goods moreItemCount = null;
                                    moreItemCount = parsedGoods.First(item => item.amount == localCounter);
                                    if (moreItemCount == null)
                                    {
                                        moreItemCount = parsedGoods.First(item => item.amount > localCounter);
                                        var newGoods = moreItemCount.CloneGoods();
                                        newGoods.isAvailable = false;//TODO передать в client
                                        var newAssemblyItem = new SessionItem(
                                            "",
                                            sessionForRequest.id,
                                            equalItem.id,
                                            equalItem.cellId,
                                            (int)StatusType.Created,
                                            0,
                                            0,
                                            DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                        goodsForRequest.Add(newGoods);
                                        itemsForRequest.Add(newAssemblyItem);
                                        parsedGoods.Remove(equalItem);
                                    }
                                    else
                                    {
                                        var prevGoods = moreItemCount.CloneGoods();
                                        var newGoods = moreItemCount.CloneGoods();
                                        prevGoods.amount = prevGoods.amount - collItem.Count;
                                        newGoods.isAvailable = false;
                                        newGoods.id = "";
                                        newGoods.amount = collItem.Count;
                                        var newAssemblyItem = new SessionItem(
                                            "",
                                            sessionForRequest.id,
                                            newGoods.id,
                                            newGoods.cellId,
                                            (int)StatusType.Created,
                                            0,
                                            0,
                                            DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                                        goodsForRequest.Add(prevGoods);
                                        goodsForRequest.Add(newGoods);
                                        itemsForRequest.Add(newAssemblyItem);
                                        moreItemCount.amount = moreItemCount.amount - collItem.Count;//local update
                                    }



                                }


                            }

                        }

                        //TODO Отправить все данные на сервер
                        await client.CreateAssebmlySession(sessionForRequest, goodsForRequest, itemsForRequest, ip);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Создание сессии на сборку", ex.Message);
                        Log.CloseAndFlush();
                        throw;
                    }
                }

            });
            loadFile = new RelayCommand(async o =>
            {
                OpenFileDialog dialog = new OpenFileDialog();
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
            var parsedOutcomeCells = JsonConvert.DeserializeObject<List<Cell>>(outcomeCells);
            foreach (var item in parsedOutcomeCells)
            {
                Cells.Add(item);

            }
            var parsedCells = JsonConvert.DeserializeObject<List<Cell>>(cells);
            foreach (var item in parsedCells)
            {
                AllCells.Add(item);

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

        public static async Task<CreateAssemblySessionViewModel> CreateAsync(Window window)
        {
            var jsonIp = File.ReadAllText("config.json");
            var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
            var ip = setting["Ip"];
            var catalogAndSuppliers = await client.GetAllCatalogsWithSuppliers(ip);
            var suppliers = await client.GetSuppliers(ip);
            var batches = await client.GetBatches(ip);
            var barcodes = await client.GetBarcodes(ip);
            var outcomeCells = await client.GetOutcomeCells(ip);
            var cells = await client.GetCells(ip);
            var cellTypes = await client.GetCellTypes(ip);
            return new CreateAssemblySessionViewModel(catalogAndSuppliers, suppliers, barcodes, outcomeCells, batches, cellTypes, window, cells);
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
