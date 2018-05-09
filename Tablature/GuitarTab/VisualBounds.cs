using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace GuitarTab
{
    public class VisualBounds
    {
        private int left;
        public int Left
        {
            get { return left; }
            set
            {
                if (value != left)
                {
                    left = value;
                    onBoundsPropertyChanged(nameof(Left), false);
                }
            }
        }

        private int top;
        public int Top
        {
            get { return top; }
            set
            {
                if (value != top)
                {
                    top = value;
                    onBoundsPropertyChanged(nameof(Top), false);
                }
            }
        }

        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                if (value != width)
                {
                    width = value;
                    onBoundsPropertyChanged(nameof(Width), true);
                }
            }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set
            {
                if (value != height)
                {
                    height = value;
                    onBoundsPropertyChanged(nameof(Height), true);
                }
            }
        }

        public int Bar { get; set; }
        public event EventHandler<BoundsPropertyChangedEventArgs> PropertyChanged;

        public VisualBounds(int left, int top, int width, int height, int bar)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Bar = bar;
        }

        public Rect genRectangleFromBounds() { return new Rect(Left, Top, Width, Height); }

        public virtual bool containsPoint(Point point)
        {
            return (point.X >= Left && point.X <= Left + Width && point.Y >= Top && point.Y <= Top + Height) ;
        }

        public virtual bool containedInRectangle(Rect rect)
        {
            return (rect.Left < Left + Width && rect.Right > Left && rect.Top < Top + Height && rect.Bottom > Top);
        }

        public virtual bool containsRectangle(Rect rect)
        {
            return (rect.Left >= Left && rect.Right <= Left + Width && rect.Top >= Top && rect.Bottom <= Top + Height);
        }

        protected void onBoundsPropertyChanged(string name, bool update)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new BoundsPropertyChangedEventArgs(name, update));
        }
    }

    //something needs to be done about this. the inheritence doesnt make sense and is not very useful
    public class MultipleVisualBounds : VisualBounds
    {
        public List<VisualBounds> AllBounds { get; set; }
        public int EndBar { get; set; }

        private MultipleVisualBounds(List<VisualBounds> bounds, int left, int right, int top, int bottom, int bar, int end_bar)
            :base(left, right, top, bottom, bar)
        {
            AllBounds = bounds;
            EndBar = end_bar;
        }

        public static MultipleVisualBounds createInstance(List<VisualBounds> bounds)
        {
            int left = bounds.FirstOrDefault()?.Left ?? 0;
            int width = bounds.LastOrDefault()?.Width ?? 0;
            int top = bounds.FirstOrDefault()?.Top ?? 0;
            int height = bounds.LastOrDefault()?.Top + bounds.LastOrDefault()?.Height - top ?? 0;
            int bar = bounds.FirstOrDefault()?.Bar ?? 0;
            int end_bar = bounds.LastOrDefault()?.Bar ?? 0;

            return new MultipleVisualBounds(bounds, left, width, top, height, bar, end_bar);
        }

        public void updateInstance(List<VisualBounds> bounds)
        {
            Left = bounds.LastOrDefault()?.Left ?? 0;
            Width = bounds.LastOrDefault()?.Width ?? 0;
            Top = bounds.FirstOrDefault()?.Top ?? 0;
            Height = bounds.LastOrDefault()?.Top + bounds.LastOrDefault()?.Height - Top ?? 0;
            Bar = bounds.FirstOrDefault()?.Bar ?? 0;
            EndBar = bounds.LastOrDefault()?.Bar ?? 0;

            AllBounds = bounds;
        }

        public override bool containsPoint(Point point)
        {
            foreach (VisualBounds Bound in AllBounds)
            {
                if (Bound.containsPoint(point)) { return true; }
            }
            return false;
        }

        public override bool containedInRectangle(Rect rect)
        {
            foreach (VisualBounds Bound in AllBounds)
            {
                if (Bound.containedInRectangle(rect)) { return true; }
            }
            return false;
        }
    }

    public class BoundsPropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; }
        public bool RequiresVisualUpdate { get; }

        public BoundsPropertyChangedEventArgs(string name, bool update)
            :base()
        {
            PropertyName = name;
            RequiresVisualUpdate = update;
        }
    }
}
