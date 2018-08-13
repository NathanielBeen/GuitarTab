using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class UpdatingVisualBounds : BaseViewModel
    {
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

        public UpdatingVisualBounds(IBounds base_bounds, int current_left, int top_margin)
        {
            Left = base_bounds.Left + current_left;
            Top = base_bounds.Top + top_margin;
            Width = base_bounds.Width;
            Height = base_bounds.Height;
        }

        public bool containsPoint(Point point)
        {
            return (point.X >= Left && point.X <= Left + Width && point.Y >= Top && point.Y <= Top + Height);
        }
    }
}
