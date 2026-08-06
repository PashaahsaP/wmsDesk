using ExcelFileParser;
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
using WmsDesk.ViewModels;

namespace WmsDesk.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateSessionByExcelFile.xaml
    /// </summary>
    public partial class CreateSessionByExcelFile : Window
    {
        public List<IncomeItemVm> Result = new List<IncomeItemVm>();
        public CreateSessionByExcelFile(CreateSessionByExcelFileViewModel vm, FileReader reader)
        {
            
                foreach (var item in reader.filesInfo[0].SessionField)
                {
                    // в вм добавить коллецию из коллеций
                    vm.Data.Add(new AddingExcelFile() { FieldName = item, FileField = reader.filesInfo[0].FileField });
                    vm.Dialog = this;
                    vm.Reader = reader;
                    vm.TablesList = reader.TablesList;
                }
            
            DataContext = vm;
            InitializeComponent();
        }
    }
}
