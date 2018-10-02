using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Controls;
using System.Windows;

namespace WpfImageTest
{
    public class ImgContainerManager
    {
        /* ---------------------------------------------------- */
        //     フィールド
        /* ---------------------------------------------------- */
        private int numofBackwardContainer  = 2;    // 巻き戻し方向のコンテナの数
        private int numofForwardContainer   = 2;    // 進む方向のコンテナの数

        /* ---------------------------------------------------- */
        //     プロパティ
        /* ---------------------------------------------------- */
        public Point Origin { get; set; } = new Point(450, 400);    // 原点
        public List<ImgContainer> Containers { get; set; } = new List<ImgContainer>();
        public ImagePool ImagePool { get; set; } = new ImagePool();

          
        /* ---------------------------------------------------- */
        //     コンストラクタ
        /* ---------------------------------------------------- */
        public ImgContainerManager()
        {
        }


        /* ---------------------------------------------------- */
        //     メソッド
        /* ---------------------------------------------------- */
        public void Initialize()
        {
            for(int i = -numofBackwardContainer; i <= numofForwardContainer; i++ )
            {
                Containers.Add( new ImgContainer(i) );
            }
        }

        public void InitContainerIndex()
        {
            Containers.ForEach( tc => tc.InitIndex() );
        }

        public void InitContainerPos()
        {
            Containers.ForEach( tc => tc.InitPos(Origin, MainWindow.TempProfile.SlideDirection) );
        }

        public void InitContainerGrid()
        {
            Containers.ForEach( tc => tc.InitGrid(2, 2) );
        }

        public void SetImageElementToContainerGrid()
        {
            Containers.ForEach( tc => tc.SetImageElementToGrid(MainWindow.TempProfile.SlideDirection) );
        }


        public async Task InitAllContainerImage(int index)
        {
            ImagePool.InitIndex(index);
            ImagePool.InitImageFileContextRefCount();
            InitContainerIndex();
            InitContainerPos();
            InitContainerGrid();
            SetImageElementToContainerGrid();

            // 前方向マッピング
            MapImageFileContextToContainer(Containers[2], false);
            MapImageFileContextToContainer(Containers[3], false);
            MapImageFileContextToContainer(Containers[4], false);

            // 巻き戻し方向マッピング
            MapImageFileContextToContainer(Containers[1], true);
            MapImageFileContextToContainer(Containers[0], true);

            // 画像のロード
            await Containers[2].LoadImage();
            await Containers[3].LoadImage();
            await Containers[1].LoadImage();
            await Containers[4].LoadImage();
            await Containers[0].LoadImage();

            // 使われていないBitmapImageの開放
            ImagePool.ReleaseBitmapImageOutofRefarence();
        }


        public void MapImageFileContextToContainer(ImgContainer container, bool isBackward)
        {
            container.ImageFileContextMapList.Clear();

            for(int i=0; i < container.NumofGrid; i++ )
            {
                // 前方向
                if( !isBackward )
                {
                    container.ImageFileContextMapList.Add( ImagePool.PickForward() );
                }

                // 巻き戻し方向
                else
                {
                    container.ImageFileContextMapList.Insert( 0, ImagePool.PickBackward() );
                }
            }
        }


        public void ReleaseContainerImage(ImgContainer container)
        {
            foreach( var child in container.MainGrid.Children )
            {
                Image image = child as Image;
                if(image != null )
                {
                    image.Source = null;
                }
            }

            container.ImageFileContextMapList.ForEach( context => 
            {
                context.RefCount--;
                if(context.RefCount <= 0 )
                {
                    context.RefCount = 0;
                    context.BitmapImage = null;
                }
            });
        }


        public async Task SlideToForward()
        {
            ImgContainer returnConteiner = null;

            foreach(ImgContainer c in Containers)
            {
                c.CurrentIndex -= 1;
                if(c.CurrentIndex < -numofBackwardContainer)
                {
                    c.CurrentIndex = numofForwardContainer;
                    returnConteiner = c;
                }
            }

            InitContainerPos();

            if( returnConteiner != null )
            {
                ReleaseContainerImage(returnConteiner);
                ImagePool.ShiftBackwardIndex( returnConteiner.ImageFileContextMapList.Count );
                MapImageFileContextToContainer(returnConteiner, false);
                await returnConteiner.LoadImage();
            }
        }


        public async Task SlideToBackward()
        {
            ImgContainer returnConteiner = null;

            foreach(ImgContainer c in Containers)
            {
                c.CurrentIndex += 1;
                if(c.CurrentIndex > numofForwardContainer)
                {
                    c.CurrentIndex = -numofBackwardContainer;
                    returnConteiner = c;
                }
            }

            InitContainerPos();

            if( returnConteiner != null )
            {
                ReleaseContainerImage(returnConteiner);
                ImagePool.ShiftForwardIndex( -returnConteiner.ImageFileContextMapList.Count );
                MapImageFileContextToContainer(returnConteiner, true);
                await returnConteiner.LoadImage();
            }

        }

    }
}
