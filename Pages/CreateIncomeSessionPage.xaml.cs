using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WmsDesk.Converter;
using WmsDesk.Enums;
using WmsDesk.ViewModels;
using WmsDesk.Windows;

namespace WmsDesk.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreateSessionPage.xaml
    /// </summary>
    public partial class CreateIncomeSessionPage : Page
    {
        private readonly Client client = new Client();
        private MainViewModel _vm = null;
        private CreateIncomeSessionViewModel localVm = null;
        private Window _window = null;
      
        public CreateIncomeSessionPage(MainViewModel vm, Window window, IState state)
        {
            InitializeComponent();
            _window = window;
            DataContext = state as CreateIncomeSessionViewModel;
            localVm = state as CreateIncomeSessionViewModel;
            _vm = vm;
        }
        private void WrapPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length == 1)
                {
                    GetDataFromFile(files[0], client);
                }
            }
        }//DONE
        private async Task GetDataFromFile(String pathToFile, Client client)
        {
           /* var func = AdapterHelper.getDataFromFile[localVm.Supplier];
            var result = func(pathToFile, client, ip);
            localVm.Items = new ObservableCollection<IUiItem>(await result);*/
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)// для выделения элемента
        {
            var ui = (sender as FrameworkElement);
            var context = ui.DataContext;
            var element = context as IncomeItemVm;
            var result = new List<IncomeItemVm>();
            // Сбросить выделения всех элементов
                // Определить тип и присвоить соответствующий тип
            // Определить тип нового выделенного элемента и присвоить соответствующий
            foreach (var item in localVm.Items)
            {
               // if(item.Name == element.Name && item.Sku == element.Sku && item.Count == element.Count)
                if (item == element)
                {
                    IncomeItemVm temp = CreateSelectedItems(element, item);
                    result.Add(temp);
                }
                else
                {//надо определить что за тип сначало, а потом создать соответствующий
                    IncomeItemVm temp = CreateUnselectedItem(element, item);

                    result.Add(temp);
                }
            }

            localVm.Items = new ObservableCollection<IncomeItemVm>(result);
        }

        private static IncomeItemVm CreateSelectedItems(IncomeItemVm element, IncomeItemVm item)
        {
            var temp = new IncomeItemVm();
            if (element is IncomeItemWithDateVm)
            {

                temp = new SelectedIncomeItemWithDateVm()
                {
                    Count = element.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = element.TE,
                    CatalogId = element.CatalogId,
                    isSelected = true,
                    Other = element.Other,
                    Date = ((IncomeItemWithDateVm)element).Date
                };
            }
            else if (element is IncomeItemWithBatchVm)
            {
                temp = new SelectedIncomeItemWithBatchVM()
                {
                    Count = element.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = element.TE,
                    CatalogId = element.CatalogId,
                    isSelected = true,
                    Other = element.Other,
                    Batches = ((IncomeItemWithBatchVm)element).Batches
                };
            }
            else
            {
                temp = new SelectedIncomeItemVm()
                {
                    Count = element.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = element.TE,
                    CatalogId = element.CatalogId,
                    isSelected = true,
                    Other = element.Other
                };
            }

            return temp;
        }
        private static IncomeItemVm CreateUnselectedItem(IncomeItemVm element, IncomeItemVm item)
        {
            var temp = new IncomeItemVm();
            if (item is IncomeItemWithDateVm || item is SelectedIncomeItemWithDateVm)
            {
                temp = new IncomeItemWithDateVm()
                {
                    Count = item.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = item.TE,
                    CatalogId = item.CatalogId,
                    isSelected = false,
                    Other = item.Other,
                    Date = ((IncomeItemWithDateVm)item).Date
                };
            }
            else if (item is IncomeItemWithBatchVm || item is SelectedIncomeItemWithBatchVM)
            {
                temp = new IncomeItemWithBatchVm()
                {
                    Count = item.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = item.TE,
                    CatalogId = item.CatalogId,
                    isSelected = false,
                    Other = item.Other,
                    Batches = ((IncomeItemWithBatchVm)item).Batches
                };
            }
            else if (item is WrongItemVm)
            {
                temp = new WrongItemVm()
                {
                    Count = item.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = item.TE,
                    CatalogId = item.CatalogId,
                    isSelected = false,
                    Other = item.Other,
                    Batches = ((WrongItemVm)item).Batches,
                    Date = ((WrongItemVm)item).Date,
                    Barcode = item.Barcode,
                };
            }
            else
            {
                temp = new IncomeItemVm()
                {
                    Count = item.Count,
                    Sku = item.Sku,
                    Name = item.Name,
                    isValid = item.isValid,
                    TE = item.TE,
                    CatalogId = item.CatalogId,
                    isSelected = false,
                    Other = item.Other
                };
            }

            return temp;
        }

        private void TextBox_KeyDown_Count(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            var frameElem = (sender as TextBox);
            var context = (sender as FrameworkElement).DataContext;
            var element = context as IncomeItemVm; // selected
            var result = new List<IncomeItemVm>();
            var temp = new IncomeItemVm();
            foreach (var item in localVm.Items)
            {
                if (item == element)
                {
                    if(element is IncomeItemWithDateVm)
                    {
                        temp = new IncomeItemWithDateVm() { 
                            Count = int.Parse(frameElem.Text), 
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
                        temp = new IncomeItemWithBatchVm()
                        {
                            Count = int.Parse(frameElem.Text),
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
                    else
                    {
                        temp = new IncomeItemVm() { 
                            Count = int.Parse(frameElem.Text), 
                            Sku = element.Sku, 
                            Name = element.Name, 
                            isValid = element.isValid, 
                            TE = element.TE, 
                            CatalogId = element.CatalogId };
                    }
                    
                    result.Add(temp);
                }
                else
                {
                   // var temp = new IncomeItemVm() { Count = item.Count, Sku = item.Sku, Name = item.Name, isValid = item.isValid, TE = item.TE, CatalogId = item.CatalogId };
                    result.Add(item);
                }
            }

            localVm.Items = new ObservableCollection<IncomeItemVm>(result);
        }
        private void TextBox_KeyDown_TE(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            var frameElem = (sender as TextBox);
            var context = (sender as FrameworkElement).DataContext;
            var element = context as IncomeItemVm; // selected
            var result = new List<IncomeItemVm>();
            var temp = new IncomeItemVm();
            foreach (var item in localVm.Items)
            {
                if (item == element)
                {
                    if(element is IncomeItemWithDateVm)
                    {
                        temp = new IncomeItemWithDateVm() { 
                            Count = element.Count, 
                            Sku = element.Sku, 
                            Name = element.Name, 
                            isValid = element.isValid, 
                            TE = frameElem.Text, 
                            CatalogId = element.CatalogId,
                            Date = ((IncomeItemWithDateVm)element).Date,
                            isSelected = element.isSelected,
                            Other = element.Other
                        };

                    }
                    else if (element is IncomeItemWithBatchVm)
                    {
                        temp = new IncomeItemWithBatchVm()
                        {
                            Count = int.Parse(frameElem.Text),
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
                    else
                    {
                        temp = new IncomeItemVm() { 
                            Count = int.Parse(frameElem.Text), 
                            Sku = element.Sku, 
                            Name = element.Name, 
                            isValid = element.isValid, 
                            TE = element.TE, 
                            CatalogId = element.CatalogId };
                    }
                    
                    result.Add(temp);
                }
                else
                {
                   // var temp = new IncomeItemVm() { Count = item.Count, Sku = item.Sku, Name = item.Name, isValid = item.isValid, TE = item.TE, CatalogId = item.CatalogId };
                    result.Add(item);
                }
            }

            localVm.Items = new ObservableCollection<IncomeItemVm>(result);
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var ui = (sender as TextBox);
            //ui.Focus();
            //ui.SelectAll();

        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)// когда элемента нет в каталоге
        {
            var dialog = new AddItemWindow(new ObservableCollection<Classes.IncomeItemEntity>(localVm.CatalogItems.ToEntityList(localVm.CatalogData)), localVm.CatalogData);
            var uiItem = (sender as FrameworkElement).DataContext as IncomeItemVm;//wrong item was
            dialog.Owner = _window;

            bool? result = dialog.ShowDialog();
            IncomeItemVm data = null;
            if (result == true)
            {
                data = dialog.orderItem;
                var entityItem = localVm.CatalogData.First(inner => inner.CatalogId == data.CatalogId);
                var supplier = localVm.Suppliers.First(inner => inner.Id == entityItem.SupplierId);

                var resultCollection = new ObservableCollection<IncomeItemVm>();
                foreach (var item in localVm.Items)
                {
                    if (item == uiItem)
                    {
                        switch (supplier.SupplierType)
                        {
                            
                            case (int)SupplierTypes.Atomy:
                                resultCollection.Add(new IncomeItemWithDateVm() { 
                                    Count = uiItem.Count, 
                                    isValid = true, 
                                    Name = data.Name, 
                                    Sku = data.Sku, 
                                    TE = item.TE, 
                                    CatalogId = data.CatalogId, 
                                    isSelected = false, 
                                    Barcode = item.Barcode, 
                                    Date = (uiItem as WrongItemVm).Date,
                                    Other = item.Other 
                                });
                                break;
                            case (int)SupplierTypes.FidConsalt:
                                resultCollection.Add(new IncomeItemWithBatchVm() { 
                                    Count = uiItem.Count, 
                                    isValid = true, 
                                    Name = data.Name, 
                                    Sku = data.Sku, 
                                    TE = item.TE, 
                                    CatalogId = data.CatalogId, 
                                    Barcode= item.Barcode,
                                    Batches = (uiItem as WrongItemVm).Batches,
                                    isSelected = false,
                                    Other = item.Other
                                });
                                break;
                            default:
                                resultCollection.Add(new IncomeItemVm() { 
                                    Count = uiItem.Count, 
                                    isValid = true, 
                                    Name = data.Name, 
                                    Sku = data.Sku, 
                                    TE = item.TE, 
                                    CatalogId = data.CatalogId, 
                                    isSelected = false 
                                });
                                break;

                        }
                    }
                    else
                        resultCollection.Add(item);

                }

                localVm.Items = resultCollection;
            }
        }

        private void listItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (IncomeItemVm)listItems.SelectedItem;
            localVm.Items.Add(selectedItem);
        }

      

        private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            var frameElem = (sender as TextBox);
            var context = (sender as FrameworkElement).DataContext;
            var element = context as IncomeItemVm;
            var result = new List<IncomeItemVm>();
            foreach (var item in localVm.Items)
            {
                if (item.CatalogId == element.CatalogId &&  item.Name == element.Name && item.Sku == element.Sku)
                {
                    var temp = new IncomeItemVm() 
                    { Count = element.Count, Sku = element.Sku, Name = element.Name, isValid = element.isValid, TE = "", CatalogId = element.CatalogId };
                    result.Add(temp);
                }
                else
                {
                    var temp = new IncomeItemVm() 
                    { Count = item.Count, Sku = item.Sku, Name = item.Name, isValid = item.isValid, TE = item.TE, CatalogId = item.CatalogId };
                    result.Add(temp);
                }
            }

            localVm.Items = new ObservableCollection<IncomeItemVm>(result);
        }

      
    }
}
