using Prism.Events;
using WpfApp1.ViewModels;

namespace WpfApp1.Events
{
    public class AlbumSelectedEvent : PubSubEvent<AlbumLookupEntryViewModel>
    {
    }
}