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
        public ImgContainerManager ImgContainerManager = new ImgContainerManager();
        public static Profile TempProfile = new Profile();

        public MainWindow()
        {
            InitializeComponent();

            ImgContainerManager.Initialize();
            ImgContainerManager.Containers.ForEach( tc => {
                MainGrid.Children.Add(tc);
            });
            ImgContainerManager.InitContainerPos();


            this.Drop += (s, e) =>
            {
                string[] folder = e.Data.GetData(DataFormats.FileDrop) as string[];
                var files = Directory.GetFiles( folder[0], "*.*", SearchOption.AllDirectories );

                ImgContainerManager.ImagePool.Initialize(files);
                ImgContainerManager.InitAllContainerImage(0);
            };

            this.KeyDown += (s, e) =>
            {
                switch( e.Key )
                {
                    case Key.Left:
                        TempProfile.SlideDirection = SlideDirection.Left;
                        TextBox1.Text = "スライド方向：左";
                        ImgContainerManager.InitAllContainerImage(0);
                        break;
                    case Key.Up:
                        TempProfile.SlideDirection = SlideDirection.Top;
                        TextBox1.Text = "スライド方向：上";
                        ImgContainerManager.InitAllContainerImage(0);
                        break;
                    case Key.Right:
                        TempProfile.SlideDirection = SlideDirection.Right;
                        TextBox1.Text = "スライド方向：右";
                        ImgContainerManager.InitAllContainerImage(0);
                        break;
                    case Key.Down:
                        TempProfile.SlideDirection = SlideDirection.Bottom;
                        TextBox1.Text = "スライド方向：下";
                        ImgContainerManager.InitAllContainerImage(0);
                        break;
                    case Key.Return:
                        ImgContainerManager.SlideToForward();
                        break;
                    case Key.Back:
                        ImgContainerManager.SlideToBackward();
                        break;
                }
            };
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ImgContainerManager.SlideToForward();
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ImgContainerManager.SlideToBackward();
        }
    }
}
