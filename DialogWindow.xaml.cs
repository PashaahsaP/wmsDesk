using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WmsDesk.ViewModels;

namespace WmsDesk
{
    
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        private object Sender{ get; set; }
        private List<CatalogItemBase> Items { get; set; }
        CreateIncomeSessionViewModel MainViewModel { get; set; }
        ObservableCollection<IncomeItemVm> UiItems { get; set; }
        internal DialogWindow(object sender, List<CatalogItemBase> items,  System.Collections.ObjectModel.ObservableCollection<IncomeItemVm> uiItems)
        {
            InitializeComponent();
            Sender = sender;
            Items = items;
            UiItems = uiItems;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (listItems != null)
            {
                var content = textBox.Text.ToString();
                var filtered = Items.Where(item => item.Name != null && item.Name.ToLower().Contains(content)).ToList();
                listItems.ItemsSource = null;
                listItems.ItemsSource = filtered;
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
           
            
        }
    }
}
