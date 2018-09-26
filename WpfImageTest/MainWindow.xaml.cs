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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;

namespace WpfImageTest
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public BenchMarker BenchMarder = new BenchMarker();
        public ImgContainerManager TileContainerManager = new ImgContainerManager();

        public MainWindow()
        {
            InitializeComponent();

            TileContainerManager.Initialize();
            TileContainerManager.Containers.ForEach( tc => {
                MainGrid.Children.Add(tc);
            });
            TileContainerManager.InitContainerPos();


            this.Drop += (s, e) =>
            {
                string[] folder = e.Data.GetData(DataFormats.FileDrop) as string[];
                var files = Directory.GetFiles( folder[0], "*.*", SearchOption.AllDirectories );

                TileContainerManager.ImagePool.Initialize(files);
                TileContainerManager.InitAllContainerImage(0);

                // Async
                //foreach( string path in files )
                //{
                //    BitmapImage source = await BenchMarder.GetImage(path);
                //    Image image = new Image();
                //    image.Source = source;
                //}

                //TextBox1.Text += "image file loaded";
            };

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
