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
    public class BenchMarker
    {
        public List<Image>          ImageList       { get; set; } = new List<Image>();
        public List<BitmapImage>    BitmapImageList { get; set; } = new List<BitmapImage>();

        public int DexodePixelWidth  { get; set; } = 160;
        public int DexodePixelHeight { get; set; } = 160;

        public BenchMarker()
        {
        }

        public void LoadImage(string[] pathes)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            /* ---------------------------------------------------- */

            ImageList.Clear();
            BitmapImageList.Clear();

            foreach(string path in pathes )
            {
                Stream st = File.OpenRead(path);
                var source = new BitmapImage();

                source.BeginInit();
                source.CacheOption   = BitmapCacheOption.OnLoad;
                source.CreateOptions = BitmapCreateOptions.None;

                source.DecodePixelWidth = this.DexodePixelWidth;
                source.DecodePixelHeight = this.DexodePixelHeight;

                source.StreamSource = st;
                source.EndInit();
                source.Freeze();

                BitmapImageList.Add(source);

                var image = new Image();
                image.Source = source;
                ImageList.Add(image);

                Debug.WriteLine("bitmap load from stream: " + source.PixelWidth + "x" + source.PixelHeight + "  path: " + path);
            }


            /* ---------------------------------------------------- */
            sw.Stop();
            Debug.WriteLine("-----------------------------------------------------");
            Debug.WriteLine(" load finished:  time: " + sw.Elapsed);
            Debug.WriteLine("-----------------------------------------------------");
        }



        public async Task LoadImageAsync(string[] pathes)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            /* ---------------------------------------------------- */

            ImageList.Clear();
            BitmapImageList.Clear();

            foreach(string path in pathes )
            {
                BitmapImage source = await GetImage(path);

                BitmapImageList.Add(source);
                Debug.WriteLine("bitmap load from stream: " + source.PixelWidth + "x" + source.PixelHeight + "  path: " + path);
            }


            /* ---------------------------------------------------- */
            sw.Stop();
            Debug.WriteLine("-----------------------------------------------------");
            Debug.WriteLine(" load finished:  time: " + sw.Elapsed);
            Debug.WriteLine("-----------------------------------------------------");
        }


        // @ref https://chitoku.jp/programming/wpf-lazy-image-behavior
        public Task<BitmapImage> GetImage(string path)
        {
            return Task.Run(() =>
            {
                Stream st = File.OpenRead(path);
                var source = new BitmapImage();

                try
                {
                    source.BeginInit();
                    source.CacheOption   = BitmapCacheOption.OnLoad;
                    source.CreateOptions = BitmapCreateOptions.None;

                    source.DecodePixelWidth  = this.DexodePixelWidth;
                    source.DecodePixelHeight = this.DexodePixelHeight;

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
