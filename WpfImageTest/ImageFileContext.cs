using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;

namespace WpfImageTest
{
    public class ImageFileContext
    {
        public string        Path { get; set; }
        public BitmapImage   BitmapImage { get; set; }
        public ImageFileInfo Info { get; set; }
        public bool          Archiver { get; set; } 
        public int           RefCount { get; set; } = 0; // 参照カウンタ(コンテナからの)

        public ImageFileContext(string path)
        {
            this.Path = path;
        }

        public Stream GetStream()
        {
            return File.OpenRead(this.Path);
        }

        // @ref https://chitoku.jp/programming/wpf-lazy-image-behavior
        public Task<BitmapImage> GetImage()
        {
            return Task.Run(() =>
            {
                Stream st = GetStream();
                var source = new BitmapImage();

                try
                {
                    source.BeginInit();
                    source.CacheOption   = BitmapCacheOption.OnLoad;
                    source.CreateOptions = BitmapCreateOptions.None;

                    source.DecodePixelWidth  = 1920;
                    source.DecodePixelHeight = 1920;

                    source.StreamSource = st;
                    source.EndInit();
                    source.Freeze();
                    return source;
                }
                catch (IOException) { }
                catch (InvalidOperationException) { }
                finally { }
                return null;
            });
        }
    }
}
