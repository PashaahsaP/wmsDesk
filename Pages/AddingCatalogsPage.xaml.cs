using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WmsDesk.ViewModels;

namespace WmsDesk.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddingCatalogsPage.xaml
    /// </summary>
    public partial class AddingCatalogsPage : Page
    {

        public AddingCatalogViewModel viewModel;
        public AddingCatalogsPage(MainViewModel vm, IState localVm)
        {
            InitializeComponent();
            viewModel = localVm as AddingCatalogViewModel;
        }


        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listItems.SelectedItem != null)
            {
                // Получаем выбранный элемент как ListBoxItem 
                viewModel.IsEnabledAppend = false;
                viewModel.IsEnabledSave = true;
                var selectedItem = (OrderItem)listItems.SelectedItem;
                //tbName.Text = selectedItem.name;
                //tbSku.Text = selectedItem.sku;
                //CBSuppliersCatalog.SelectedIndex = 0;
                viewModel.SelectedBarcodes.Clear();
                foreach (var item in viewModel.Barcodes.Where(item => item.CatalogId == selectedItem.id))
                {
                    viewModel.SelectedBarcodes.Add(item);
                }
                ;
                for (int i = 0; i < viewModel.MainSuppliers.Count; i++)
                {
                    if (selectedItem.supplierName == viewModel.MainSuppliers[i].Name)
                    {
                        CBSuppliersCatalog.SelectedIndex = i;
                    }
                }
                viewModel.SelectedCatalogItem =  viewModel.CatalogItems.FirstOrDefault(item => item.CatalogId == selectedItem.id);

                Console.WriteLine();
            }

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = viewModel;
            CBSuppliers.SelectedItem = viewModel.Suppliers.First();
        }

      
    }
}
