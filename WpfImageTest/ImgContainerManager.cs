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
        public Point Origin { get; set; } = new Point(300, 100);    // 原点
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

        public void InitContainerPos()
        {
            Containers.ForEach( tc => tc.InitPos(Origin) );
        }

        public void InitContainerGrid()
        {
            Containers.ForEach( tc => tc.InitGrid(2, 2) );
        }

        public async Task InitAllContainerImage(int index)
        {
            ImagePool.InitIndex(index);
            InitContainerPos();
            InitContainerGrid();
            await PickImageToContainer(Containers[2], false);
            await PickImageToContainer(Containers[3], false);
            await PickImageToContainer(Containers[4], false);
        }

        public async Task PickImageToContainer(ImgContainer container, bool isBackward)
        {
            foreach( var child in container.MainGrid.Children )
            {
                Image image = child as Image;
                if(image != null )
                {
                    // 前方向
                    if(!isBackward) image.Source = await ImagePool.PickForward();

                    // 巻き戻し方向
                    //else image.Source = await ImagePool.PickBackward();
                }
            }

        }
    }
}
