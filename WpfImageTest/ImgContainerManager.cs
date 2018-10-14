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
    public enum SlideShowState
    {
        Stop, Continuous, Interval
    }

    public class ImgContainerManager
    {
        /* ---------------------------------------------------- */
        //     フィールド
        /* ---------------------------------------------------- */
        private int numofBackwardContainer  = 2;    // 巻き戻し方向のコンテナの数
        private int numofForwardContainer   = 2;    // 進む方向のコンテナの数
        private Point wrapPoint     = new Point();  // 前方向スライド後、コンテナが末尾に戻る座標
        private Point wrapPointRev  = new Point();  // 後方向スライド後、コンテナが先頭に戻る座標

        /* ---------------------------------------------------- */
        //     プロパティ
        /* ---------------------------------------------------- */
        public List<ImgContainer> Containers { get; set; } = new List<ImgContainer>();
        public SlideShowState SlideShowState { get; set; } = SlideShowState.Stop;
        public ImagePool ImagePool { get; set; } = new ImagePool();
        public int CurrentImageIndex
        {
            get
            {
                ImgContainer ic = Containers.FirstOrDefault(c => c.CurrentIndex == 0);
                if( ic == null ) return 0;
                else
                {
                    ImageFileContext context = ic.ImageFileContextMapList.FirstOrDefault(ct => !ct.IsDummy);
                    if( context == null ) return 0;
                    else
                    {
                        return ImagePool.ImageFileContextList.IndexOf(context);
                    }
                }
            }
        }
        public ImgContainer CurrentContainer
        {
            get
            {
                return Containers.FirstOrDefault(c => c.CurrentIndex == 0);
            }
        }
        public Size CurrentContainerGridSize
        {
            get
            {
                ImgContainer c = CurrentContainer;
                return new Size(c.Width / c.NumofCol, c.Height / c.NumofRow);
            }
        }
        public int ContainerWidth
        {
            get { return (int)Containers[0].Width; }
        }
        public int ContainerHeight
        {
        get { return (int)Containers[0].Height; }
        }
        public Point ContinuousSlideReturnPoint
        {
            get
            {
                Point returnPoint = new Point();
                returnPoint.X = 0; returnPoint.Y = 0;
                SlideDirection dir = MainWindow.TempProfile.SlideDirection;

                switch( dir )
                {
                case SlideDirection.Left:
                    returnPoint.X   = numofForwardContainer * ContainerWidth;
                    break;
                case SlideDirection.Top:
                    returnPoint.Y   = numofForwardContainer * ContainerHeight;
                    break;
                case SlideDirection.Right:
                    returnPoint.X   = - numofForwardContainer * ContainerWidth;
                    break;
                case SlideDirection.Bottom:
                    returnPoint.Y   = - numofForwardContainer * ContainerHeight;
                    break;
                }

                return returnPoint;
            }
        }

          
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
            Containers.ForEach( tc => tc.InitPos(MainWindow.TempProfile.SlideDirection) );
        }

        public void InitWrapPoint(SlideDirection dir)
        {
            wrapPoint.X = 0;    wrapPoint.Y = 0;
            wrapPointRev.X = 0; wrapPointRev.Y = 0;
            switch( dir )
            {
            case SlideDirection.Left:
                wrapPoint.X     = -(numofBackwardContainer + 1) * ContainerWidth;
                wrapPointRev.X  = (numofForwardContainer + 1) * ContainerWidth;
                break;
            case SlideDirection.Top:
                wrapPoint.Y     = -(numofBackwardContainer + 1) * ContainerHeight;
                wrapPointRev.Y  = (numofForwardContainer + 1) * ContainerHeight;
                break;
            case SlideDirection.Right:
                wrapPoint.X     = (numofBackwardContainer + 1) * ContainerWidth;
                wrapPointRev.X  = -(numofForwardContainer + 1) * ContainerWidth;
                break;
            case SlideDirection.Bottom:
                wrapPoint.Y     = (numofBackwardContainer + 1) * ContainerHeight;
                wrapPointRev.Y  = -(numofForwardContainer + 1) * ContainerHeight;
                break;
            }
        }

        public void InitContainerGrid(int numofCol, int numofRow)
        {
            Containers.ForEach( tc => tc.InitGrid(numofCol, numofRow) );
        }

        public void SetImageElementToContainerGrid()
        {
            Containers.ForEach( tc => tc.SetImageElementToGrid(MainWindow.TempProfile.SlideDirection) );
        }


        public async Task InitAllContainer(int index)
        {
            StopSlideShow();
            ImagePool.InitIndex(index);
            ImagePool.InitImageFileContextRefCount();
            InitContainerIndex();
            InitContainerPos();
            InitWrapPoint(MainWindow.TempProfile.SlideDirection);

            InitContainerGrid(2, 2);
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

            bool bReachEnd = false;

            for(int i=0; i < container.NumofGrid; i++ )
            {
                // 前方向
                if( !isBackward )
                {
                    if( bReachEnd ) container.ImageFileContextMapList.Add(ImagePool.DummyImageFileContext);
                    else container.ImageFileContextMapList.Add(ImagePool.PickForward());

                    if( !bReachEnd && ImagePool.ForwardIndex == 0 ) bReachEnd = true;
                }

                // 巻き戻し方向
                else
                {
                    if( bReachEnd ) container.ImageFileContextMapList.Insert(0, ImagePool.DummyImageFileContext);
                    else container.ImageFileContextMapList.Insert( 0, ImagePool.PickBackward() );

                    if( !bReachEnd && ImagePool.BackwardIndex == ImagePool.ImageFileContextList.Count - 1 ) bReachEnd = true;
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


        public void ActiveSlideToForward(bool slideBySizeOfOneImage)
        {
            if( SlideShowState != SlideShowState.Stop ) StopSlideShow();

            foreach( ImgContainer container in Containers )
            {
                container.Animation.OnStoryboardCompleted = async (s, e) =>
                {
                    // 座標を確定させる
                    container.Margin = new Thickness(container.Animation.ActiveSlideEndPoint.X, container.Animation.ActiveSlideEndPoint.Y, 0, 0);

                    // リターンするコンテナ
                    if( container.Pos == wrapPoint )
                    {
                        Containers.ForEach(c => c.CurrentIndex -= 1);
                        container.CurrentIndex = numofForwardContainer;
                        container.InitPos(MainWindow.TempProfile.SlideDirection);
                        ReleaseContainerImage(container);
                        ImagePool.ShiftBackwardIndex( container.NumofImage );
                        MapImageFileContextToContainer(container, false);
                        await container.LoadImage();
                    }
                };
                container.Animation.BeginActiveSlideAnimation(slideBySizeOfOneImage, false, 300, CurrentContainerGridSize);
            }
        }


        public void ActiveSlideToBackward(bool slideBySizeOfOneImage)
        {
            if( SlideShowState != SlideShowState.Stop ) StopSlideShow();

            foreach( ImgContainer container in Containers )
            {
                container.Animation.OnStoryboardCompleted = async (s, e) =>
                {
                    // 座標を確定させる
                    container.Margin = new Thickness(container.Animation.ActiveSlideEndPoint.X, container.Animation.ActiveSlideEndPoint.Y, 0, 0);

                    // リターンするコンテナ
                    if( container.Pos == wrapPointRev )
                    {
                        Containers.ForEach(c => c.CurrentIndex += 1);
                        container.CurrentIndex = -numofBackwardContainer;
                        container.InitPos(MainWindow.TempProfile.SlideDirection);
                        ReleaseContainerImage(container);
                        ImagePool.ShiftForwardIndex( - container.NumofImage );
                        MapImageFileContextToContainer(container, true);
                        await container.LoadImage();
                    }
                };
                container.Animation.BeginActiveSlideAnimation(slideBySizeOfOneImage, true, 300, CurrentContainerGridSize);
            }
        }

        public void StartContinuousSlideShow()
        {
            if( SlideShowState == SlideShowState.Continuous ) return;
            if( SlideShowState == SlideShowState.Interval ) StopSlideShow();

            foreach( ImgContainer container in Containers )
            {
                // 折り返し時
                container.Animation.OnStoryboardCompleted = async (s, e) =>
                {
                    Containers.ForEach(c => c.CurrentIndex -= 1);
                    container.CurrentIndex = numofForwardContainer;
                    container.InitPos(MainWindow.TempProfile.SlideDirection);
                    ReleaseContainerImage(container);
                    ImagePool.ShiftBackwardIndex(container.NumofImage);
                    MapImageFileContextToContainer(container, false);
                    container.Animation.BeginContinuousSlideAnimation(ContinuousSlideReturnPoint, wrapPoint, 3000);
                    await container.LoadImage();
                };

                // アニメーションスタート
                Point ptFrom = new Point(container.Margin.Left, container.Margin.Top);
                container.Animation.BeginContinuousSlideAnimation(ptFrom, wrapPoint, 3000);
            }

            SlideShowState = SlideShowState.Continuous;
        }

        public void StopSlideShow()
        {
            foreach( ImgContainer container in Containers )
            {
                container.Animation.StopSlideAnimation();
            }

            SlideShowState = SlideShowState.Stop;
        }

        public void ToggleSlideShow()
        {
            if(SlideShowState == SlideShowState.Stop )
            {
                StartContinuousSlideShow();
            }
            else StopSlideShow();
        }
    }
}
