using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using WmsDesk.Enums;

namespace WmsDesk.ViewModels
{
    public class AddingCatalogViewModel : INotifyPropertyChanged, IState
    {
        private static readonly Client client = new Client();
        private static string ip = "192.168.0.11";
        private string _tbText = "";
        private string _tbName = "";
        private string _tbSku = "";
        private string _tbBarcode = "";
        private string _tblockError = "";
        private bool _isEnabledAppend = true;
        private bool _isEnabledSave = false;

        public string TbText { get
            {
                return _tbText;
            }
            set
            {
                _tbText = value;
                Filter.Sort = _tbText;
                _items = new ObservableCollection<OrderItem>( Filter.Apply().Select(item => new OrderItem()
                {
                    id = item.CatalogId,
                    name = item.Name,
                    other = item.Other,
                    sku = item.Sku,
                    supplierId = item.SupplierId.ToString(),
                    supplierName = Suppliers.FirstOrDefault(inner => inner.Id ==  item.SupplierId).ToString()
                }));
                OnPropertyChanged(nameof(TbText));
                OnPropertyChanged(nameof(ItemsList));

            }
        }
        public string TbSku
        {
            get
            {
                return _tbSku;
            }
            set
            {
                _tbSku = value;
                OnPropertyChanged(nameof(TbSku));
            }
        }
        public string TbName
        {
            get
            {
                return _tbName;
            }
            set
            {
                _tbName = value;
                OnPropertyChanged(nameof(TbName));
            }
        }
        public string TbBarcode
        {
            get
            {
                return _tbBarcode;
            }
            set
            {
                _tbBarcode = value;
                OnPropertyChanged(nameof(TbBarcode));
            }
        }
        public string TblockError
        {
            get
            {
                return _tblockError;
            }
            set
            {
                _tblockError = value;
                OnPropertyChanged(nameof(TblockError));
            }
        }
        private ObservableCollection<OrderItem> _items = new ObservableCollection<OrderItem>();
        private ObservableCollection<CatalogItemBase> _catalogItems = new ObservableCollection<CatalogItemBase>();
        private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();
        private ObservableCollection<Supplier> _mainSuppliers = new ObservableCollection<Supplier>();
        private Supplier _selectedSupplier = new Supplier() { Id = -1, Name = "" };
        private Supplier _mainSelectedSupplier;
        private Supplier _selectedSupplierCatalog = new Supplier() { Id = -1, Name = "", SupplierType = -1 };
        private CatalogItemBase _selectedCatalogItem = null;
        private ObservableCollection<Barcode> _selectedBarcodes = new ObservableCollection<Barcode>();
        

        public ICommand addBarcode { get; set; }
        public ICommand addLot { get; set; }
        public ICommand removeBarcode { get; set; }
        public ICommand clearFields { get; set; }
        public ICommand addEntity { get; set; }
        public ICommand updateEntity { get; set; }
        public bool IsEnabledAppend
        {
            get
            {
                return _isEnabledAppend;
            }
            set
            {
                _isEnabledAppend = value;
                OnPropertyChanged(nameof(IsEnabledAppend));
            }
        }
        public bool IsEnabledSave
        {
            get
            {
                return _isEnabledSave;
            }
            set
            {
                _isEnabledSave = value;
                OnPropertyChanged(nameof(IsEnabledSave));
            }
        }
        public Visibility PartyVisibility { get; set; } = Visibility.Collapsed;
        public CatalogFilter Filter { get; set; } = new CatalogFilter(new List<CatalogItemBase>(), new List<Barcode>());
        public CatalogItemBase SelectedCatalogItem
        {
            get
            {
                return _selectedCatalogItem;
            }
            set
            {
                _selectedCatalogItem = value;
                OnPropertyChanged(nameof(SelectedCatalogItem));
            }
        } 
        public Supplier MainSelectedSupplier
        {
            get => _mainSelectedSupplier;
            set
            {
                //set prop
                _mainSelectedSupplier = value;
                //set id for selectedCatalogItem and change type. if is new copy data, else copy from collection
                if (SelectedCatalogItem != null)
                {
                    SelectedCatalogItem.SupplierId = value.Id;
                    if(SelectedCatalogItem.CatalogId == "")// copy of data in new type
                    {
                        switch (MainSelectedSupplier.SupplierType)
                        {
                            case (int)ClientType.Base: SelectedCatalogItem = new CatalogItemBase()
                            { 
                                Barcode = SelectedCatalogItem.Barcode,
                                BarcodeList = SelectedCatalogItem.BarcodeList,
                                Name = SelectedCatalogItem.Name,
                                Other = SelectedCatalogItem.Other,
                                Sku = SelectedCatalogItem.Sku,
                                SupplierId = MainSelectedSupplier.Id,
                            }; break;
                            case (int)ClientType.WithDate: SelectedCatalogItem = new WithDate()
                            {
                                Barcode = SelectedCatalogItem.Barcode,
                                BarcodeList = SelectedCatalogItem.BarcodeList,
                                Name = SelectedCatalogItem.Name,
                                Other = SelectedCatalogItem.Other,
                                Sku = SelectedCatalogItem.Sku,
                                SupplierId = MainSelectedSupplier.Id,
                            }; break;
                            case (int)ClientType.WithBatch: SelectedCatalogItem = new WithBatch()
                            {
                                Barcode = SelectedCatalogItem.Barcode,
                                BarcodeList = SelectedCatalogItem.BarcodeList,
                                Name = SelectedCatalogItem.Name,
                                Other = SelectedCatalogItem.Other,
                                Sku = SelectedCatalogItem.Sku,
                                SupplierId = MainSelectedSupplier.Id,
                            }; break;
                        }
                    }
                    else// copy of data in new type. if data is "", copy from old collection
                    {
                        var valueInCollection = CatalogItems.First(item => item.CatalogId == SelectedCatalogItem.CatalogId);
                        switch (MainSelectedSupplier.SupplierType)
                        {
                            case (int)ClientType.Base:
                                SelectedCatalogItem = new CatalogItemBase()
                                {
                                    Barcode = SelectedCatalogItem.Barcode,
                                    BarcodeList = SelectedCatalogItem.BarcodeList,
                                    Name = SelectedCatalogItem.Name,
                                    Other = SelectedCatalogItem.Other,
                                    Sku = SelectedCatalogItem.Sku,
                                    SupplierId = MainSelectedSupplier.Id,
                                    CatalogId = SelectedCatalogItem.CatalogId
                                }; break;
                            case (int)ClientType.WithDate:
                                SelectedCatalogItem = new WithDate()
                                {
                                    Barcode = SelectedCatalogItem.Barcode,
                                    BarcodeList = SelectedCatalogItem.BarcodeList,
                                    Name = SelectedCatalogItem.Name,
                                    Other = SelectedCatalogItem.Other,
                                    Sku = SelectedCatalogItem.Sku,
                                    SupplierId = MainSelectedSupplier.Id,
                                    CatalogId = SelectedCatalogItem.CatalogId
                                }; break;
                            case (int)ClientType.WithBatch:
                                SelectedCatalogItem = new WithBatch()
                                {
                                    Barcode = SelectedCatalogItem.Barcode,
                                    BarcodeList = SelectedCatalogItem.BarcodeList,
                                    Name = SelectedCatalogItem.Name,
                                    Other = SelectedCatalogItem.Other,
                                    Sku = SelectedCatalogItem.Sku,
                                    SupplierId = MainSelectedSupplier.Id,
                                    CatalogId = SelectedCatalogItem.CatalogId,
                                    Batches = valueInCollection as WithBatch != null &&  ((WithBatch)valueInCollection).Batches.Count != 0 
                                        ? ((WithBatch)valueInCollection).Batches 
                                        : Batches.Where(item => item.CatalogId == SelectedCatalogItem.CatalogId).ToList(),
                                }; break;
                        }
                    }
                }
                //change class type
                if (SelectedCatalogItem == null && MainSelectedSupplier != null)//при отсутствующем левом поле, т.е. элемент не выбран
                {
                    switch (MainSelectedSupplier.SupplierType)
                    {
                       case (int)ClientType.Base: SelectedCatalogItem = new CatalogItemBase(); break;   
                       case (int)ClientType.WithDate: SelectedCatalogItem = new WithDate(); break;   
                       case (int)ClientType.WithBatch: SelectedCatalogItem = new WithBatch(); break;
                    }
                }
                else if(SelectedCatalogItem != null && MainSelectedSupplier != null)
                {

                }
                OnPropertyChanged(nameof(MainSelectedSupplier));// Добавить применение фильтром и прочее
                
            }
        }
        public Supplier SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                _selectedSupplier = value;
                Filter.Supplier = _selectedSupplier;
                _items = new ObservableCollection<OrderItem> (Filter.Apply().Select(item => new OrderItem() 
                { 
                    id = item.CatalogId, 
                    name = item.Name, 
                    sku= item.Sku, 
                    other = item.Other, 
                    supplierId = item.SupplierId.ToString(), 
                    supplierName = Suppliers.First(inner => inner.Id == item.SupplierId).Name
                }));
                OnPropertyChanged(nameof(SelectedSupplier));// Добавить применение фильтром и прочее
                OnPropertyChanged(nameof(ItemsList));// Добавить применение фильтром и прочее
            }
        }
        public Supplier SelectedSupplierCatalog
        {
            get => _selectedSupplierCatalog;
            set
            {
                _selectedSupplierCatalog = value;
                if(value.Name == "Atomy") 
                    PartyVisibility = Visibility.Visible;
                else
                    PartyVisibility=Visibility.Collapsed;

                OnPropertyChanged(nameof(PartyVisibility));// Добавить применение фильтром и прочее
                OnPropertyChanged(nameof(SelectedSupplierCatalog));// Добавить применение фильтром и прочее
            }
        }
        public List<Barcode> Barcodes {  get; set; } = new List<Barcode>();
        public List<Batch> Batches {  get; set; } = new List<Batch>();
        public ObservableCollection<Barcode> SelectedBarcodes {  
            get
            {
                return _selectedBarcodes;
            }
            set 
            {
                _selectedBarcodes = value;
                OnPropertyChanged(nameof(SelectedBarcodes));

            } 
        }
        public ObservableCollection<Supplier> MainSuppliers
        {
            get
            {
                return _mainSuppliers;
            }
            set
            {
                _suppliers = value;
                OnPropertyChanged(nameof(MainSuppliers));
            }
        }
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
        public ObservableCollection<OrderItem> ItemsList { 
            get
            {
                return _items;
                
            }
            set
            {
                _items = value;
                OnPropertyChanged(nameof(ItemsList));
            }

        }
        public ObservableCollection<CatalogItemBase> CatalogItems { 
            get
            {
                return _catalogItems;
            }
            set
            {
                _catalogItems = value;
                OnPropertyChanged(nameof(CatalogItems));
            }
        }
        public OrderItem SelectedItem { get; set; }

        public PageStates PageState => PageStates.AddingCatalogPage;

        public  AddingCatalogViewModel(string data, string suppliers, string barcodes, string batches)
        {
            var batchData = JsonConvert.DeserializeObject<List<Batch>>(batches);
            Batches = batchData;

            var supplierData = JsonConvert.DeserializeObject<ObservableCollection<Supplier>>(suppliers);
            Suppliers.Add(SelectedSupplier);
            //MainSuppliers.Add(MainSelectedSupplier);
            foreach (var item in supplierData)
            {
                Suppliers.Add(item);
                MainSuppliers.Add(item);
            }
            //parse barcodes
            var parsedBarcodes = JsonConvert.DeserializeObject<ObservableCollection<Barcode>>(barcodes);
            foreach (var item in parsedBarcodes)
            {
                Barcodes.Add(item);

            }
            //parse catalog items
            var parsedData = JsonConvert.DeserializeObject<ObservableCollection<CatalogItemBase>>(data);
            foreach (var item in parsedData)
            {
                item.BarcodeList = Barcodes.Where(inner => inner.CatalogId == item.CatalogId).ToList();
                //sorting by type of supplier
                var sup = Suppliers.First(x => x.Id == item.SupplierId);
                switch(sup.SupplierType)
                {
                    case  (int)ClientType.Base:
                        CatalogItems.Add(item);
                        break;
                    case (int)ClientType.WithDate:
                        var newObj = new WithDate()
                        {
                            CatalogId = item.CatalogId,
                            Name = item.Name,
                            Sku = item.Sku,
                            SupplierId = item.SupplierId,
                            Barcode = item.Barcode,
                            BarcodeList = item.BarcodeList,
                            Other = item.Other,
                        };
                        CatalogItems.Add(newObj);
                        break;
                    case (int)ClientType.WithBatch:
                        var newObjWithBatch = new WithBatch()
                        {
                            CatalogId = item.CatalogId,
                            Name = item.Name,
                            Sku = item.Sku,
                            SupplierId = item.SupplierId,
                            Barcode = item.Barcode,
                            BarcodeList = item.BarcodeList,
                            Other = item.Other,
                            Batches = Batches.Where(inner => item.CatalogId == inner.CatalogId).ToList(),
                        };
                        CatalogItems.Add(newObjWithBatch);
                        
                        break;
                    default:
                        throw new Exception();
                }
                
                

            }
            var temp = new ObservableCollection<OrderItem>(_catalogItems.Where(x => x.Name.ToLower().Contains(TbText.ToLower()))
                .Select(inner => new OrderItem() { 
                id = inner.CatalogId,
                name = inner.Name,
                other = inner.Other,
                sku = inner.Sku,
                supplierId = inner.SupplierId.ToString(),
                supplierName = Suppliers.FirstOrDefault(item => item.Id == inner.SupplierId)?.Name,
                }
                ));
            ItemsList = temp;
            //parse suppliers
            Filter.Items = temp.Select(item => new CatalogItemBase()
            {
                CatalogId = item.id,
                SupplierId = int.Parse(item.supplierId),
                Name = item.name,
                Sku = item.sku,
                Other = item.other,
                BarcodeList = Barcodes.Where(inner => inner.CatalogId == item.id).ToList()

            }).ToList();
            Filter.Barcodes = parsedBarcodes.ToList();
            removeBarcode = new RelayCommand(o =>
            {
                var barcode = (Barcode)o;
                var item = SelectedBarcodes.First(it => it.Id == barcode.Id && it.Name == barcode.Name);
                for (int i = 0; i < SelectedBarcodes.Count; i++)
                {
                    if (SelectedBarcodes[i].Id == item.Id && SelectedBarcodes[i].Name == item.Name)
                    {
                        SelectedBarcodes.RemoveAt(i);
                    }
                    
                }
                
            });
            clearFields = new RelayCommand(o => {
                
                SelectedCatalogItem = null;
                IsEnabledSave = false;
                IsEnabledAppend = true;
                TbBarcode = "";
                TbName = "";
                TbSku = "";
                TblockError = "";
                SelectedBarcodes.Clear();
                MainSelectedSupplier = null;
            });
            addBarcode = new RelayCommand(async o =>
            {
                var curCatalog = SelectedCatalogItem;
                if (TbBarcode != "")
                {
                    var newBarcode = new Barcode("?", TbBarcode, curCatalog != null ? curCatalog.CatalogId : "none");
                    SelectedBarcodes.Add(newBarcode);
                    SelectedCatalogItem.BarcodeList = new List<Barcode>(SelectedBarcodes);
                    TbBarcode = "";
                }
            });   
            addLot = new RelayCommand(async o =>
            {
                var t = SelectedCatalogItem as WithBatch;
                if (Batches.Any(item => item.Name == t.SearchLine))
                    return;
                var newBatch = new Batch() { CatalogId = t.CatalogId, Id = -1, Name = t.SearchLine};
                Batches.Add(newBatch);
                t.Batches.Add(newBatch);
                t.SearchLine = t.SearchLine;
            });
            addEntity = new RelayCommand(async o => {
                var text = "";
                bool isOk = true;
                if (MainSelectedSupplier.Name == "") {
                    isOk = false;
                    text = "Выберите поставщика\n";
                }
                if (SelectedCatalogItem.Name == "")
                {
                    isOk = false;
                    text += "Добавьте наименование товара\n";
                }

                TblockError = text;
                if (isOk) {
                    //clear field
                    TblockError = "";
                    // update db
                    var suppId = SelectedSupplierCatalog.Id;
                    var catalogId = await client.SendCatalog(new Catalog() { name = SelectedCatalogItem.Name, supplierId = MainSelectedSupplier.Id, sku = SelectedCatalogItem.Sku}, ip);
                    var barcodeId = "";
                    foreach (var barcode in SelectedBarcodes)
                    {
                        barcodeId = await client.SendBarcode(
                        new BarcodeItem() { name = barcode.Name, supplierId = MainSelectedSupplier.Id, catalogId = catalogId }, ip);
                        Barcodes.Add(new Barcode(barcodeId, barcode.Name, catalogId));
                    }
                    foreach (var item in Batches)
                    {
                        item.CatalogId = catalogId;
                        if (item.Id == -1)//not in bd
                        {
                            var addedId = await client.SendBatch(item, ip);
                            item.Id = addedId;
                        }
                    }
                    //TODO update ui
                    ItemsList.Add(new OrderItem()
                    {
                        id = catalogId,
                        name = SelectedCatalogItem.Name,
                        sku = SelectedCatalogItem.Sku,
                        supplierId = MainSelectedSupplier.Id.ToString(),
                        supplierName = MainSelectedSupplier.Name
                    });
                    Filter.Items = new List<CatalogItemBase>(ItemsList.Select(item => new CatalogItemBase() { 
                        CatalogId = item.id,
                        Name = item.name,
                        Sku = item.sku,
                        Other = item.other,
                        BarcodeList = Barcodes.Where(inner => inner.CatalogId == item.id).ToList(),
                        SupplierId = int.Parse(item.supplierId)
                    }));
                    CatalogItems.Add(Filter.Items.Last());
                    clearFields.Execute(null);
                }
            });
            updateEntity = new RelayCommand(async o =>{
                if(SelectedCatalogItem != null)
                {
                    var updatedId = await client.UpdateCatalog(SelectedCatalogItem, ip);
                    if(updatedId == SelectedCatalogItem.CatalogId)
                    {
                        //сброс коллекции
                        var curSup = Filter.Supplier;
                        Filter.Supplier = Suppliers.First();
                        ItemsList = new ObservableCollection<OrderItem>( Filter.Apply().Select(item =>
                        new OrderItem() { id = item.CatalogId, name = item.Name, other = item.Other, sku = item.Sku, supplierId = item.SupplierId.ToString(), supplierName = Suppliers.First(inner => inner.Id == item.SupplierId).Name}));

                        

                        var t = ItemsList.First(it => it.id == SelectedCatalogItem.CatalogId);
                        t.name = SelectedCatalogItem.Name;
                        t.sku = SelectedCatalogItem.Sku;
                        t.supplierId = MainSelectedSupplier.Id.ToString();

                        //обработка коллекции. TODO сделать коллецию в которой базовая коллекция хранится
                        Filter.Supplier = curSup;
                        ItemsList = new ObservableCollection<OrderItem>(Filter.Apply().Select(item =>
                        new OrderItem() { id = item.CatalogId, name = item.Name, other = item.Other, sku = item.Sku, supplierId = item.SupplierId.ToString(), supplierName = Suppliers.First(inner => inner.Id == item.SupplierId).Name }));


                        //ItemsList = new ObservableCollection<OrderItem>(ItemsList);
                        //MainSelectedSupplier = SuppliersList.FirstOrDefault(s => s.Id == SelectedSupplier.Id);
                        //SelectedSupplier = new Supplier() { Id = SelectedSupplier.Id, Name = SelectedSupplier.Name, Type = SelectedSupplier.Type};
                    }

                    //remove barcodes
                    var barcodesToRemove = Barcodes.Where(bar => bar.CatalogId == SelectedCatalogItem.CatalogId).ToList();
                    foreach (var item in barcodesToRemove)
                    {
                        if (!SelectedBarcodes.Any(it => it.Name == item.Name))
                        {
                            var q = await client.RemoveBarcode(item, ip);
                            Barcodes.Remove(item);
                        }
                    }
                    //send barcodes
                    foreach (var item in SelectedBarcodes)
                    {
                        if (item.Id == "?")
                        {
                            //добавить в бд
                            var newBarcode = new BarcodeItem()
                            {
                                name = item.Name,
                                catalogId = item.CatalogId,
                                supplierId = MainSelectedSupplier.Id
                            };
                            var addedId = await client.SendBarcode(newBarcode, ip);
                            //добавить в локальное хранилище
                            Barcodes.Add(new Barcode(addedId, item.Name, item.CatalogId));
                        }
                    }
                    //send batches
                    foreach (var item in Batches)
                    {
                        if (item.Id == -1)//not in bd
                        {
                            var addedId = await client.SendBatch(item, ip);
                            item.Id = addedId;
                        }
                    }
                }
            }); 
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public static async Task<AddingCatalogViewModel> CreateAsync()
        {
            var jsonIp = File.ReadAllText("config.json");
            var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
            ip = setting["Ip"];
            var batches = await client.GetBatches(ip);
            var data = await client.GetAllCatalogsWithSuppliers(ip, 0);
            var suppliers = await client.GetSuppliers(ip);
            var barcodes = await client.GetBarcodes(ip);
            return new AddingCatalogViewModel(data, suppliers, barcodes, batches);
        }
         
    }
}
