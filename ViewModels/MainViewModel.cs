using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WmsDesk
{
    internal class AtomyItem : IUiItem
    {
        private string _name;
        private int _count;
        private int _amount;
        private string _TE;
        private string _barcode;
        private OrderItem _catalog;
        public string Name { get => _name; set => _name = value; }
        public int Count { get => _count; set => _count = value; }
        public string TE { get => _TE; set => _TE = value; }
        public string Barcode { get => _barcode; set => _barcode = value; }
        public OrderItem Catalog { get => _catalog; set => _catalog = value; }
        public int Amount { get => _amount; set => _amount = value; }
        public string Left { get => _name; }
        public string Center { get => TE; }
        public string Right { get => $"{_count}/{_amount}"; }
    }
    internal class BorkItem : IUiItem
    {
        private string _name;
        private int _count;
        private int _amount;
        private string _art;
        private string _barcode;
        private OrderItem _catalog;
        public string Name { get => _name; set => _name = value; }
        public int Count { get => _count; set => _count = value; }
        public string Art { get => _art; set => _art = value; }
        public string Barcode { get => _barcode; set => _barcode = value; }
        public OrderItem Catalog { get => _catalog; set => _catalog = value; }
        public int Amount { get => _amount; set => _amount = value; }
        public string Left { get => _name; }
        public string Center { get => _barcode; }
        public string Right { get => $"{_count}/{_amount}"; }

    }
    public class MenuItem : INotifyPropertyChanged
    {
        private bool _isSelected;
        private Page _page;
        public string Title { get; set; }
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public Page Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Page));
            }
        }

        public IState State { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public class MainViewModel : INotifyPropertyChanged
    {
        #region bisiness member

        public string ip { get; set; }

        private ObservableCollection<MenuItem> _leftMenuItems = new ObservableCollection<MenuItem>();
        private ObservableCollection<MenuItem> _rightMenuItems = new ObservableCollection<MenuItem>();
        private GridLength _leftWidth = new GridLength(1, GridUnitType.Star);
        private GridLength _centerWidth = new GridLength(0, GridUnitType.Star);
        private GridLength _rightWidth = new GridLength(0, GridUnitType.Star);

        public ObservableCollection<MenuItem> LeftMenuItems
        {
            get => _leftMenuItems;
            set
            {
                _leftMenuItems = value;
                OnPropertyChanged(nameof(LeftMenuItems));
            }
        }
        public ObservableCollection<MenuItem> RightMenuItems
        {
            get => _rightMenuItems;
            set
            {
                _rightMenuItems = value;

                OnPropertyChanged(nameof(RightMenuItems));

                if (RightMenuItems.Count != 0)
                {
                    LeftWidth = new GridLength(0.5, GridUnitType.Star);
                    CenterWidth = new GridLength(0.01, GridUnitType.Star);
                    RightWidth = new GridLength(0.5, GridUnitType.Star);
                }
                else
                {
                    LeftWidth = new GridLength(1, GridUnitType.Star);
                    CenterWidth = new GridLength(0, GridUnitType.Star);
                    RightWidth = new GridLength(0, GridUnitType.Star);
                }
                OnPropertyChanged(nameof(LeftWidth));
                OnPropertyChanged(nameof(CenterWidth));
                OnPropertyChanged(nameof(RightWidth));

            }
        }
        public MainWindow MainWindow { get; set; }



        #endregion
        #region Display Window
        private Window _window;

        /// <summary>
        /// padding need for correct displaing in maximazed state of buttons
        /// </summary>
        private Thickness _titlePadding = new Thickness(10);
        public Thickness TitlePadding
        {
            get
            {
                return _window.WindowState == WindowState.Maximized ? _titlePadding : new Thickness(0);
            }
            set
            {
                _titlePadding = value;
            }
        }
        /// <summary>
        /// need for correct displaing menu items(second line)
        /// </summary>
        private Thickness _titleMenuPadding = new Thickness(8, 0, 8, 0);
        public Thickness TitleMenuPadding
        {
            get
            {
                return _window.WindowState == WindowState.Maximized ? _titleMenuPadding : new Thickness(0);
            }
            set
            {
                _titleMenuPadding = value;
            }
        }
        /// <summary>
        /// need for correct displaing button in top right
        /// </summary>
        private int _titleHeight = 30;
        public int TitleHeight
        {
            get
            {
                return _window.WindowState == WindowState.Maximized ? 50 : _titleHeight;
            }

            set
            {
                _titleHeight = value;
            }
        }
        public int MenuTitleHeight { get; set; } = 30;
        private Page _HomePage;
        public Page HomePage
        {
            get => _HomePage;
            set
            {
                if (_HomePage != value)
                {
                    _HomePage = value;
                    OnPropertyChanged(nameof(HomePage));
                }
            }
        }
        private Page _leftCurrentPage;
        private Page _rightCurrentPage;
        public Page LeftCurrentPage
        {
            get => _leftCurrentPage;
            set
            {


                if (_leftCurrentPage != value)
                {
                    _leftCurrentPage = value;
                    OnPropertyChanged(nameof(LeftCurrentPage));
                }
            }
        }
        public Page RightCurrentPage
        {
            get => _rightCurrentPage;
            set
            {

                if (_rightCurrentPage != value)
                {
                    _rightCurrentPage = value;
                    OnPropertyChanged(nameof(RightCurrentPage));
                }
            }
        }

        public GridLength LeftWidth
        {
            get => _leftWidth;
            set
            {
                _leftWidth = value;
                OnPropertyChanged(nameof(LeftWidth));
            }
        }
        public GridLength CenterWidth
        {
            get => _centerWidth;
            set
            {
                _centerWidth = value;
                OnPropertyChanged(nameof(CenterWidth));
            }
        }
        public GridLength RightWidth
        {
            get => _rightWidth;
            set
            {
                _rightWidth = value;
                OnPropertyChanged(nameof(RightWidth));
            }
        }
        #region commands
        public ICommand collapseWindow { get; set; }
        public ICommand expandWindow { get; set; }
        public ICommand closeWindow { get; set; }
        public ICommand selectLeftMenuItem { get; set; }
        public ICommand selectRightMenuItem { get; set; }
        public ICommand callHomeWindow { get; set; }
        public ICommand moveToLeft { get; set; }
        public ICommand moveToRight { get; set; }

        /// <summary>
        /// Event for closing button on press X on menu item(еще есть закрытие на среднию кнопку и это события cs коде)
        /// </summary>
        public ICommand closeMenuItem { get; set; }
        #endregion

        #endregion

        public MainViewModel(Window window)
        {
            #region Display Window
            _window = window;
            collapseWindow = new RelayCommand(o => _window.WindowState = WindowState.Minimized);
            expandWindow = new RelayCommand(o => _window.WindowState = WindowState.Maximized);
            closeWindow = new RelayCommand(o => _window.Close());
            selectLeftMenuItem = new RelayCommand((o) =>
            {
                foreach (var item in LeftMenuItems)
                {
                    item.IsSelected = false;
                }
                MenuItem menuItem = o as MenuItem;
                menuItem.IsSelected = true;
                LeftCurrentPage = menuItem.Page;

            });
            selectRightMenuItem = new RelayCommand((o) =>
            {
                foreach (var item in RightMenuItems)
                {
                    item.IsSelected = false;
                }
                MenuItem menuItem = o as MenuItem;
                menuItem.IsSelected = true;
                RightCurrentPage = menuItem.Page;

            });
            _window.StateChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TitlePadding));
                OnPropertyChanged(nameof(TitleHeight));
                OnPropertyChanged(nameof(TitleMenuPadding));
                OnPropertyChanged(nameof(MenuTitleHeight));
            };
            callHomeWindow = new RelayCommand(o =>
            {
                HomePage = new HomePage(this, window);
            });
            closeMenuItem = new RelayCommand(o =>
            {
                var item = o as MenuItem;
                if (item == null)
                    return;
                if (LeftMenuItems.Contains(item))
                {
                    if (item.IsSelected == true)
                    {
                        LeftMenuItems.Remove(item);
                        if (LeftMenuItems.Count != 0)
                        {
                            LeftMenuItems[LeftMenuItems.Count - 1].IsSelected = true;
                            LeftCurrentPage = LeftMenuItems[LeftMenuItems.Count - 1].Page;
                        }
                        else
                        {
                            LeftCurrentPage = null;
                        }
                    }
                    //LeftMenuItems.Remove(item);
                }
                else
                {
                    if (item.IsSelected == true)
                    {
                        RightMenuItems.Remove(item);
                        if (RightMenuItems.Count != 0)
                        {
                            RightMenuItems[RightMenuItems.Count - 1].IsSelected = true;
                            RightCurrentPage = RightMenuItems[RightMenuItems.Count - 1].Page;
                        }
                        else
                        {
                            RightCurrentPage = null;
                        }
                    }
                    var temp = new ObservableCollection<MenuItem>();
                    foreach (var item1 in RightMenuItems)
                    {
                        temp.Add(item1);
                    }
                    RightMenuItems = temp;
                    //RightMenuItems.Remove(item);
                }
            });
            moveToLeft = new RelayCommand(o =>
            {
                if (RightMenuItems.Count != 0)
                {
                    LeftCurrentPage = null;
                    RightCurrentPage = null;
                    var leftResult = new ObservableCollection<MenuItem>();
                    var rightResult = new ObservableCollection<MenuItem>();

                    foreach (var item in LeftMenuItems)
                    {
                        item.IsSelected = false;
                        leftResult.Add(item);
                    }

                    foreach (var item in RightMenuItems)
                    {
                        if (item.IsSelected)
                        {
                            leftResult.Add(item);
                            LeftCurrentPage = item.Page;
                        }
                        else
                        {
                            rightResult.Add(item);

                        }
                    }
                    if (rightResult.Count != 0)
                    {
                        var last = rightResult.Last();
                        last.IsSelected = true;
                        RightCurrentPage = last.Page;
                    }
                    RightMenuItems = rightResult;
                    LeftMenuItems = leftResult;

                }
            });
            moveToRight = new RelayCommand(o =>
            {
                if (LeftMenuItems.Count != 0)
                {
                    LeftCurrentPage = null;
                    RightCurrentPage = null;
                    var leftResult = new ObservableCollection<MenuItem>();
                    var rightResult = new ObservableCollection<MenuItem>();

                    foreach (var item in RightMenuItems)
                    {
                        item.IsSelected = false;
                        rightResult.Add(item);
                    }

                    foreach (var item in LeftMenuItems)
                    {
                        if (item.IsSelected)
                        {
                            rightResult.Add(item);
                            RightCurrentPage = item.Page;
                        }
                        else
                        {
                            leftResult.Add(item);

                        }
                    }
                    if (leftResult.Count != 0)
                    {
                        var last = leftResult.Last();
                        last.IsSelected = true;
                        LeftCurrentPage = last.Page;
                    }
                    LeftMenuItems = leftResult;
                    RightMenuItems = rightResult;
                }
            });
            
            #endregion
        }
        public void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
