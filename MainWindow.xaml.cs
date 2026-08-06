using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WmsDesk;

namespace WmsDesk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class Cell
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TypeCellId { get; set; }
        public string ParentCellId { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }
    public class ID
    {
        public int id { get; set; }

    }
    public class StringID
    {
        public string id { get; set; }

    }

    public class Name
    {
        public string name { get; set; }

    }
    public class AssemblySession
    {
        public string id { get; set; }
        public int supplierId { get; set; }
        public string outCell { get; set; }
        public string createdAt { get; set; }
        public string finishedAt { get; set; }
        public string deletedAt { get; set; }
        public int status { get; set; }
        public string date { get; set; }
        public int amount { get; set; }
        public int lines { get; set; }


    }
    public class AssemblyBorkItem
    {
        public int goodsId { get; set; }
        public int assemblyId { get; set; }
        public int cellId { get; set; }
        public string startedAt { get; set; }
        public string finishedAt { get; set; }
        public string status { get; set; }
    }
    public class AssemblyAtomyItem
    {
        public int goodsId { get; set; }
        public int assemblyId { get; set; }
        public int cellId { get; set; }
        public string startedAt { get; set; }
        public string finishedAt { get; set; }
        public string status { get; set; }
    }
    public class UIItems
    {
        public string Art { get; set; }
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Count { get; set; }
        public OrderItem Catalog { get; set; }
        public override string ToString() => Name;
    }
    public class BarcodeItem
    {
        public string name { get; set; }
        public string catalogId { get; set; }
        public int supplierId { get; set; }
    }
    public class AtomyGoodsItem
    {
        public string id { get; set; }
        public string catalogId { get; set; }
        public string cellId { get; set; }
        public int amount { get; set; }
        public string TE { get; set; }
        public string date { get; set; }
        public string createdAt { get; set; }
        public override string ToString() => TE;
    }
    public class GoodsBorkItem
    {
        public string id { get; set; }
        public string catalogId { get; set; }
        public string cellId { get; set; }
        public int amount { get; set; }
        public string createdAt { get; set; }

        public override string ToString() => id;
    }
    public class Catalog
    {
        public string name { get; set; }
        public int supplierId { get; set; }
        public string sku { get; set; }

    }
    public partial class MainWindow : Window
    {
        private MainViewModel vm;

        private readonly Client client = new Client();
        private string ip = "192.168.0.11";

        public MainWindow()
        {
            vm = new MainViewModel(this);
            vm.MainWindow = this;
            DataContext = vm;
            ExcelPackage.License.SetNonCommercialPersonal("Pavel Semenov");
            InitializeComponent();

            vm.LeftMenuItems = new ObservableCollection<WmsDesk.MenuItem>();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var jsonIp = File.ReadAllText("config.json");
            var setting = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonIp);
            ip = setting["Ip"];
            vm.ip = ip;
        }





        /// <summary>
        /// Work for closing menuItem by middle button of mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as Button)?.CommandParameter as WmsDesk.MenuItem;
            if (item == null)
                return;
            if (item != null)
            {
                if (item.IsSelected == true)
                {
                    vm.LeftMenuItems.Remove(item);
                    if (vm.LeftMenuItems.Count != 0)
                    {
                        vm.LeftMenuItems[vm.LeftMenuItems.Count - 1].IsSelected = true;
                        vm.LeftCurrentPage = vm.LeftMenuItems[vm.LeftMenuItems.Count - 1].Page;
                    }
                    else
                    {
                        vm.LeftCurrentPage = null;
                    }
                }
                //vm.LeftMenuItems.Remove(item);
            }
            if (item != null)
            {
                if (item.IsSelected == true)
                {
                    vm.RightMenuItems.Remove(item);
                    if (vm.RightMenuItems.Count != 0)
                    {
                        vm.RightMenuItems[vm.RightMenuItems.Count - 1].IsSelected = true;
                        vm.RightCurrentPage = vm.RightMenuItems[vm.RightMenuItems.Count - 1].Page;
                    }
                    else
                    {
                        vm.RightCurrentPage = null;
                    }
                }
                var temp = new ObservableCollection<WmsDesk.MenuItem>();
                foreach (var item1 in vm.RightMenuItems)
                {
                    temp.Add(item1);
                }
                vm.RightMenuItems = temp;
                //vm.LeftMenuItems.Remove(item);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var menuItem = vm.LeftMenuItems.First(item => item.IsSelected == true);
                vm.LeftMenuItems.Remove(menuItem);
                if (vm.LeftMenuItems.Count != 0)
                {
                    vm.LeftMenuItems[vm.LeftMenuItems.Count - 1].IsSelected = true;
                    vm.LeftCurrentPage = vm.LeftMenuItems[vm.LeftMenuItems.Count - 1].Page;
                }
                else
                {
                    vm.LeftCurrentPage = null;

                }

            }
        }
    }
}