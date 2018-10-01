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
        public SlideDirection SlideDirection = SlideDirection.Left;
    }
}
