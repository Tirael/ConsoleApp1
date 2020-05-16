using MahApps.Metro.Controls;
using WpfApp1.Interfaces;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : MetroWindow, IView<MainWindowViewModel>
    {
        public MainWindowView(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;

            InitializeComponent();
        }

        #region Implementation of IView<out MainWindowViewModel>

        public MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel)DataContext;
            set => DataContext = value;
        }

        public bool KeepAlive => true;

        #endregion
    }
}
