using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WmsDesk.Pages;

namespace WmsDesk.ViewModels
{
    internal class HomePageViewModel : INotifyPropertyChanged, IState
    {
        #region ui
        private int _widthSideBar = 150;
        private Page _page;
        public int WidthSideBar
        {
            get
            {
                return _widthSideBar;
            }
            set
            {
                _widthSideBar = value;
            }
        }
        #endregion region
        #region ui command
        public ICommand closePage { get; set; }
        public ICommand callAddingPage { get; set; }
        public ICommand callCreateIncomeSessionPage { get; set; }
        public ICommand callCreateAssemblySessionPage { get; set; }

        public PageStates PageState { get => PageStates.HomePage; }
        #endregion
        #region ctor
        public HomePageViewModel(MainViewModel vm, Window window)
        {
            closePage = new RelayCommand((o) =>
            {
                vm.HomePage = null;
            });
            callAddingPage = new RelayCommand(async (o) =>
            {
                var catalogVm = await AddingCatalogViewModel.CreateAsync();
                var newMenuItem = new MenuItem
                {
                    IsSelected = true,
                    Title = "Добавить наименование",
                    Page = new AddingCatalogsPage(vm, catalogVm),
                    State = catalogVm
                };

                AppendPage(newMenuItem, vm);
            });
            callCreateIncomeSessionPage = new RelayCommand(async (o) =>
            {
                var sessionVm = await CreateIncomeSessionViewModel.CreateAsync(window);
                var newMenuItem = new MenuItem
                {
                    IsSelected = true,
                    Title = "Создать заявку на приход",
                    Page = new CreateIncomeSessionPage(vm, window, sessionVm),
                    State = sessionVm

                };
                AppendPage(newMenuItem, vm);
            });
            callCreateAssemblySessionPage = new RelayCommand(async (o) =>
            {
                var sessionVm = await CreateAssemblySessionViewModel.CreateAsync(window);
                var newMenuItem = new MenuItem
                {
                    IsSelected = true,
                    Title = "Создать заявку на сборку",
                    Page = new CreateAssemblySessionPage(vm, window, sessionVm),
                    State = sessionVm

                };
                AppendPage(newMenuItem, vm);
            });



        }
        #region helper methods
        private void AppendPage(MenuItem menuItem, MainViewModel vm)
        {
            foreach (var item in vm.LeftMenuItems)// сброс выделения
            {
                item.IsSelected = false;
            }
            vm.LeftMenuItems.Add(menuItem); // добавить в панель
            vm.LeftCurrentPage = menuItem.Page; // Выделенный элемент на панели
            vm.HomePage = null; //закрыть боковую панель
        }
        #endregion
        #endregion



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
