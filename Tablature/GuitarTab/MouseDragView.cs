using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class MouseDragView : BaseViewModel
    {
        private Point down_point;

        private int width;
        public int Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        private int left;
        public int Left
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        private int top;
        public int Top
        {
            get { return top; }
            set { SetProperty(ref top, value); }
        }

        public MouseDragView()
        {
            down_point = default(Point);
        }

        public void setDownPoint(Point down, List<UpdatingVisualBounds> selected)
        {
            foreach (UpdatingVisualBounds bounds in selected)
            {
                if (bounds.containsPoint(down)) { return; }
            }
            down_point = down;
        }

        public void updateDragRect(Point curr)
        {
            if (down_point != default(Point))
            {
                Left = (int)Math.Min(curr.X, down_point.X);
                Top = (int)Math.Min(curr.Y, down_point.Y);
                Width = (int)Math.Abs(curr.X - down_point.X);
                Height = (int)Math.Abs(curr.Y - down_point.Y);
            }
        }

        public void clearDrag()
        {
            down_point = default(Point);
            Width = 0;
            Height = 0;
            Left = 0;
            Top = 0;
        }
    }
}
