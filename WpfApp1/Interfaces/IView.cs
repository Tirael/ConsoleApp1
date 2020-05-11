using Prism.Regions;

namespace WpfApp1.Interfaces
{
    public interface IView<T> : IRegionMemberLifetime
    {
        T ViewModel { get; set; }
    }
}