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

namespace WpfImageTest
{
    /// <summary>
    /// TileContainer.xaml の相互作用ロジック
    /// </summary>
    public partial class ImgContainer : UserControl
    {
        /* ---------------------------------------------------- */
        //     プロパティ
        /* ---------------------------------------------------- */
        public int DefaultIndex { get; set; }   // コンテナの並び順(デフォルト)
        public int CurrentIndex { get; set; }   // コンテナの並び順(現在の)
        public int NumofGrid
        {
            get { return MainGrid.ColumnDefinitions.Count * MainGrid.RowDefinitions.Count; }
        }




        public ImgContainer(int defaultIndex)
        {
            InitializeComponent();
            DefaultIndex = defaultIndex;
            CurrentIndex = defaultIndex;
        }

        public void InitPos(Point origin)
        {
            double left = origin.X + this.DefaultIndex * this.Width;
            //double top  = origin.Y + this.DefaultIndex * this.Height;
            Margin = new Thickness(left, Margin.Top, Margin.Right, Margin.Bottom);
        }

        public void InitGrid(int numofCol, int numofRow)
        {
            MainGrid.Children.Clear();

            for(int i=0; i< numofCol; i++)
            {
                ColumnDefinition c = new ColumnDefinition();
                MainGrid.ColumnDefinitions.Add(c);
            }

            for(int i=0; i< numofRow; i++)
            {
                RowDefinition r = new RowDefinition();
                MainGrid.RowDefinitions.Add(r);
            }

            for(int i=0; i< numofRow; i++)
            {
                for(int j=0; j< numofCol; j++)
                {
                    Image image = new Image();
                    MainGrid.Children.Add(image);
                    Grid.SetColumn(image, j);
                    Grid.SetRow(image, i);
                }
            }
        }
    }
}
