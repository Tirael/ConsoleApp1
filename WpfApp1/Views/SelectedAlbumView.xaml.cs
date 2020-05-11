using WpfApp1.Interfaces;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
    /// <summary>
    /// Interaction logic for SelectedAlbumView.xaml
    /// </summary>
    public partial class SelectedAlbumView : IView<SelectedAlbumViewModel>
    {
        public SelectedAlbumView(SelectedAlbumViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

        #region Implementation of IView<out SelectedAlbumViewModel>

        public SelectedAlbumViewModel ViewModel
        {
            get => (SelectedAlbumViewModel)DataContext;
            set => DataContext = value;
        }

        public bool KeepAlive => true;

        #endregion
    }
}
