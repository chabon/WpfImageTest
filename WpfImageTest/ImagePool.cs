using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;


namespace WpfImageTest
{
    public class ImagePool
    {
        public List<ImageFileContext> ImageFileContextList = new List<ImageFileContext>();
        public int ForwardIndex  { get; private set; } = 0;
        public int BackwardIndex { get; private set; } = 0;



        public void Initialize(string[] pathes)
        {
            ImageFileContextList.Clear();

            foreach( string path in pathes )
            {
                ImageFileContextList.Add( new ImageFileContext(path) );
            }

            InitIndex(0);
        }

        public void InitIndex(int index)
        {
            // 前方向
            ForwardIndex = index;
            if(ForwardIndex < 0 || ImageFileContextList.Count <= ForwardIndex )
            {
                ForwardIndex = 0;
            }

            // 巻き戻し方向
            BackwardIndex = ForwardIndex - 1;
            if( BackwardIndex < 0 )
            {
                BackwardIndex = ImageFileContextList.Count - 1;
            }
        }

        public async Task<BitmapImage> PickForward()
        {
            ImageFileContext context = ImageFileContextList[ForwardIndex];
            ForwardIndex++;
            if(ForwardIndex >= ImageFileContextList.Count )
            {
                ForwardIndex = 0;
            }

            if( context.BitmapImage == null )
            {
                context.BitmapImage = await context.GetImage();
                return context.BitmapImage;
            }
            else
            {
                return context.BitmapImage;
            }
        } 
    }
}
