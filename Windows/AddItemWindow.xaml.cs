using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using WmsDesk.Classes;
using WmsDesk.Converter;
using WmsDesk.ViewModels;

namespace WmsDesk.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddItemWindow.xaml
    /// </summary>
    public partial class AddItemWindow : Window
    {
        public AddItemViewModel localVm = null;
        public IncomeItemVm orderItem = null;
        public AddItemWindow(System.Collections.ObjectModel.ObservableCollection<IncomeItemEntity> catalogItems, List<IncomeItemEntity> catalogData)
        {
            InitializeComponent();
            localVm = new AddItemViewModel(new System.Collections.ObjectModel.ObservableCollection<IncomeItemVm>(catalogItems.ToVmList()),catalogData);
            DataContext = localVm;
            
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            orderItem = localVm.SelectedItem;
            DialogResult = true;
        }
    }
}
