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
        public int DefaultIndex { get; private set; }   // コンテナの並び順(デフォルト)
        public int CurrentIndex { get; set; }           // コンテナの並び順(現在の)
        public List<ImageFileContext> ImageFileContextMapList { get; set; } // 画像ファイル付随情報へのマップ(順序はGridと同期的) 

        public int NumofGrid
        {
            get { return MainGrid.ColumnDefinitions.Count * MainGrid.RowDefinitions.Count; }
        }

        public int NumofImage
        {
            get { return ImageFileContextMapList.Count(c => !c.IsDummy); }
        }

        /* ---------------------------------------------------- */
        //     コンストラクタ
        /* ---------------------------------------------------- */
        public ImgContainer(int defaultIndex)
        {
            InitializeComponent();
            DefaultIndex = defaultIndex;
            CurrentIndex = defaultIndex;
            ImageFileContextMapList = new List<ImageFileContext>();
        }

        /* ---------------------------------------------------- */
        //     メソッド
        /* ---------------------------------------------------- */
        public void InitIndex()
        {
            CurrentIndex = DefaultIndex;
        }

        public void InitPos(Point origin, SlideDirection slideDirection)
        {
            double left, top;
            switch( slideDirection )
            {
                default:
                case SlideDirection.Left:
                    left = origin.X + CurrentIndex * Width;
                    top  = origin.Y;
                    break;
                case SlideDirection.Top:
                    left = origin.X;
                    top  = origin.Y + CurrentIndex * Height;
                    break;
                case SlideDirection.Right:
                    left = origin.X - (CurrentIndex * Width);
                    top  = origin.Y;
                    break;
                case SlideDirection.Bottom:
                    left = origin.X;
                    top  = origin.Y - (CurrentIndex * Height);
                    break;
            }
            Margin = new Thickness(left, top, Margin.Right, Margin.Bottom);
        }

        public void InitGrid(int numofCol, int numofRow)
        {
            MainGrid.Children.Clear();
            MainGrid.ColumnDefinitions.Clear();
            MainGrid.RowDefinitions.Clear();

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
        }

        public void SetImageElementToGrid(SlideDirection slideDirection)
        {
            TileOrientation orientaition;
            TileOrigin      origin;

            int numofRow = MainGrid.RowDefinitions.Count;
            int numofCol = MainGrid.ColumnDefinitions.Count;

            switch( slideDirection )
            {
                default:
                case SlideDirection.Left:
                    orientaition    = TileOrientation.Vertical;
                    origin          = TileOrigin.TopLeft;
                    break;
                case SlideDirection.Top:
                    orientaition    = TileOrientation.Horizontal;
                    origin          = TileOrigin.TopLeft;
                    break;
                case SlideDirection.Right:
                    orientaition    = TileOrientation.Vertical;
                    origin          = TileOrigin.TopRight;
                    break;
                case SlideDirection.Bottom:
                    orientaition    = TileOrientation.Horizontal;
                    origin          = TileOrigin.BottomRight;
                    break;
            }

            Action<int, int> setToGrid = (i, j) =>
            {
                Image image = new Image();
                MainGrid.Children.Add(image);
                Grid.SetRow(image, i);
                Grid.SetColumn(image, j);
            };

            if(orientaition == TileOrientation.Horizontal )
            {
                switch( origin )
                {
                default:
                case TileOrigin.TopLeft:
                    for(int i=0; i < numofRow; i++) for(int j=0; j< numofCol; j++ ) { setToGrid(i, j); }
                    break;
                case TileOrigin.TopRight:
                    for (int i = 0; i < numofRow; i++) for (int j = numofCol -1; j >= 0; j-- ) { setToGrid(i, j); }
                    break;
                case TileOrigin.BottomRight:
                    for (int i = numofRow -1; i >=0; i--) for (int j = numofCol -1; j >=0; j-- ) { setToGrid(i, j); }
                    break;
                case TileOrigin.BottomLeft:
                    for (int i = numofRow -1; i >=0; i--) for (int j = 0; j < numofCol; j++ ) { setToGrid(i, j); }
                    break;
                }
            }
            else
            {
                switch( origin )
                {
                default:
                case TileOrigin.TopLeft:
                    for (int i = 0; i < numofCol; i++) for (int j = 0; j < numofRow; j++ ) { setToGrid(j, i); }
                    break;
                case TileOrigin.TopRight:
                    for (int i = numofCol -1; i >=0; i--) for (int j = 0; j < numofRow; j++ ) { setToGrid(j, i); }
                    break;
                case TileOrigin.BottomRight:
                    for (int i = numofCol -1; i >=0; i--) for (int j = numofRow -1; j >=0; j-- ) { setToGrid(j, i); }
                    break;
                case TileOrigin.BottomLeft:
                    for (int i = 0; i < numofCol; i++) for (int j = numofRow -1; j >=0; j-- ) { setToGrid(j, i); }
                    break;
                }
            }

        }

        public async Task LoadImage()
        {
            for(int i=0; i < MainGrid.Children.Count; i++ )
            {
                var child = MainGrid.Children[i];
                Image image = child as Image;

                if(image != null && i < ImageFileContextMapList.Count && ImageFileContextMapList[i] != null)
                {
                    if(ImageFileContextMapList[i].BitmapImage == null )
                    {
                        image.Source = await ImageFileContextMapList[i].GetImage();
                    }
                    else
                    {
                        image.Source = ImageFileContextMapList[i].BitmapImage;
                    }
                }
            }
        }

        public void Move(double x, double y)
        {
            this.Margin = new Thickness( Margin.Left + x, Margin.Top + y, Margin.Right, Margin.Bottom );
        }

    }
}
