using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp1.Interfaces
{
    public interface IImageProvider
    {
        Task<BitmapImage> GetImage(Uri uri, CancellationToken token);
    }
}