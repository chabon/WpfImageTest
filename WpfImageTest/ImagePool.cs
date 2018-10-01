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
            int maxIdx = ImageFileContextList.Count - 1;

            // 前方向
            ForwardIndex = index;
            if(ForwardIndex < 0 || maxIdx < ForwardIndex )
            {
                ForwardIndex = 0;
            }

            // 巻き戻し方向
            BackwardIndex = ForwardIndex - 1;
            if( BackwardIndex < 0 )
            {
                BackwardIndex = maxIdx;
            }
        }

        public void ShiftForwardIndex(int vari)
        {
            ForwardIndex += vari;
            int maxIdx = ImageFileContextList.Count - 1;
            if( ForwardIndex > maxIdx )
            {
                ForwardIndex = ForwardIndex % maxIdx;
            }
            else if( ForwardIndex < 0)
            {
                int p = ForwardIndex % maxIdx;
                ForwardIndex = maxIdx + p;
            }
        }

        public void ShiftBackwardIndex(int vari)
        {
            BackwardIndex += vari;
            int maxIdx = ImageFileContextList.Count - 1;
            if( BackwardIndex > maxIdx )
            {
                BackwardIndex = BackwardIndex % maxIdx;
            }
            else if( BackwardIndex < 0)
            {
                int p = BackwardIndex % maxIdx;
                BackwardIndex = maxIdx + p;
            }
        }

        public ImageFileContext PickForward()
        {
            ImageFileContext context = ImageFileContextList[ForwardIndex];
            ForwardIndex++;
            if(ForwardIndex >= ImageFileContextList.Count )
            {
                ForwardIndex = 0;
            }

            return context;
        } 

        public ImageFileContext PickBackward()
        {
            ImageFileContext context = ImageFileContextList[BackwardIndex];
            BackwardIndex--;
            if(BackwardIndex < 0 )
            {
                BackwardIndex = ImageFileContextList.Count - 1;
            }

            return context;
        } 

    }
}
