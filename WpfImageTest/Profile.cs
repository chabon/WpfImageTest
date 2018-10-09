using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfImageTest
{
    public enum SlideDirection
    {
        Left, Top, Right, Bottom
    }

    public enum TileOrigin
    {
        TopLeft, TopRight, BottomRight, BottomLeft
    }

    public enum TileOrientation
    {
        Horizontal, Vertical
    }


    public class Profile
    {

        // メンバ
        public SlideDirection SlideDirection = SlideDirection.Left;

        // プロパティ
        public bool IsHorizontalSlide
        {
            get
            {
                if( SlideDirection == SlideDirection.Left || SlideDirection == SlideDirection.Right ) return true;
                else return false;
            }
        }

        // メソッド
        public static SlideDirection GetReversedSlideDirection(SlideDirection dir)
        {
            switch( dir )
            {
                default:
                case SlideDirection.Left:
                    return SlideDirection.Right;
                case SlideDirection.Top:
                    return SlideDirection.Bottom;
                case SlideDirection.Right:
                    return SlideDirection.Left;
                case SlideDirection.Bottom:
                    return SlideDirection.Top;
            }
        }
    }
}
