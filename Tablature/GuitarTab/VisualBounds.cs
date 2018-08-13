using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace GuitarTab
{
    public interface IBounds
    {
        int Left { get; set; }
        int Top { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        int Bottom { get; }
        int Right { get; }
        int Bar { get; set; }

        Rect genRectangleFromBounds();
        bool containsPoint(Point point);
        bool containedInRectangle(Rect rect);
        bool containsRectangle(Rect rect);
        List<IBounds> getBoundsList();
        void setBoundsList(List<IBounds> bounds);
    }

    public interface IDynamicBounds
    {
        event EventHandler<BoundsPropertyChangedEventArgs> PropertyChanged;
    }


    public class SingleBounds : IBounds
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Bottom { get { return Top + Height; } }
        public int Right { get { return Left + Width; } }
        public int Bar { get; set; }

        public SingleBounds(int left, int top, int width, int height, int bar)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Bar = bar;
        }

        public Rect genRectangleFromBounds() { return new Rect(Left, Top, Width, Height); }

        public bool containsPoint(Point point)
        {
            return (point.X >= Left && point.X <= Left + Width && point.Y >= Top && point.Y <= Top + Height);
        }

        public bool containedInRectangle(Rect rect)
        {
            return (rect.Left < Left + Width && rect.Right > Left && rect.Top < Top + Height && rect.Bottom > Top);
        }

        public bool containsRectangle(Rect rect)
        {
            return (rect.Left >= Left && rect.Right <= Left + Width && rect.Top >= Top && rect.Bottom <= Top + Height);
        }

        public virtual List<IBounds> getBoundsList() { return new List<IBounds>() { this }; }

        public void setBoundsList(List<IBounds> bounds) { }
    }

    public class MultipleBounds : IBounds
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Bottom { get { return Top + Height; } }
        public int Right { get { return Left + Width; } }
        public int Bar { get; set; }

        private List<IBounds> all_bounds;
        private int start_bar;

        public MultipleBounds(List<IBounds> bounds)
        {
            all_bounds = bounds;

            Left = bounds.FirstOrDefault()?.Left ?? 0;
            Top = bounds.FirstOrDefault()?.Top ?? 0;
            Width = bounds.LastOrDefault()?.Width ?? 0;
            Height = bounds.LastOrDefault()?.Top + bounds.LastOrDefault()?.Height - Top ?? 0;
            start_bar = bounds.FirstOrDefault()?.Bar ?? 0;
            Bar = bounds.LastOrDefault()?.Bar ?? 0;
        }

        public void updateInstance(List<IBounds> bounds)
        {
            all_bounds = bounds;

            Left = bounds.FirstOrDefault()?.Left ?? 0;
            Top = bounds.FirstOrDefault()?.Top ?? 0;
            Width = bounds.LastOrDefault()?.Width ?? 0;
            Height = bounds.LastOrDefault()?.Top + bounds.LastOrDefault()?.Height - Top ?? 0;
            start_bar = bounds.FirstOrDefault()?.Bar ?? 0;
            Bar = bounds.LastOrDefault()?.Bar ?? 0;
        }

        public Rect genRectangleFromBounds() { return new Rect(Left, Top, Width, Height); }

        public bool containsRectangle(Rect rect)
        {
            return (rect.Left >= Left && rect.Right <= Left + Width && rect.Top >= Top && rect.Bottom <= Top + Height);
        }

        public bool containsPoint(Point point)
        {
            foreach (IBounds Bound in all_bounds)
            {
                if (Bound.containsPoint(point)) { return true; }
            }
            return false;
        }

        public bool containedInRectangle(Rect rect)
        {
            foreach (IBounds Bound in all_bounds)
            {
                if (Bound.containedInRectangle(rect)) { return true; }
            }
            return false;
        }

        public List<IBounds> getBoundsList() { return all_bounds; }

        public void setBoundsList(List<IBounds> bounds) { all_bounds = bounds; }
    }

    public class DynamicBounds : IBounds
    {
        private IBounds bounds;

        public int Left
        {
            get { return bounds.Left; }
            set
            {
                if (value != bounds.Left)
                {
                    bounds.Left = value;
                    onBoundsPropertyChanged(nameof(IBounds.Left), false);
                }
            }
        }

        public int Top
        {
            get { return bounds.Top; }
            set
            {
                if (value != bounds.Top)
                {
                    bounds.Top = value;
                    onBoundsPropertyChanged(nameof(IBounds.Top), false);
                }
            }
        }

        public int Width
        {
            get { return bounds.Width; }
            set
            {
                if (value != bounds.Width)
                {
                    bounds.Width = value;
                    onBoundsPropertyChanged(nameof(IBounds.Width), true);
                }
            }
        }

        public int Height
        {
            get { return bounds.Height; }
            set
            {
                if (value != bounds.Height)
                {
                    bounds.Height = value;
                    onBoundsPropertyChanged(nameof(IBounds.Height), true);
                }
            }
        }

        public int Bottom => bounds.Bottom;

        public int Right => bounds.Right;

        public int Bar { get => bounds.Bar; set => bounds.Bar = value; }

        public event EventHandler<BoundsPropertyChangedEventArgs> PropertyChanged;

        public DynamicBounds(IBounds b)
        {
            int left = b.Left;
            int top = b.Top;
            int width = b.Width;
            int height = b.Height;
            b.Left = 0;
            b.Top = 0;
            b.Width = 0;
            b.Height = 0;
            bounds = b;

            b.Left = left;
            b.Top = top;
            b.Width = width;
            b.Height = height;
        }

        protected void onBoundsPropertyChanged(string name, bool update)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new BoundsPropertyChangedEventArgs(name, update));
        }

        public Rect genRectangleFromBounds() { return bounds.genRectangleFromBounds(); }

        public bool containsPoint(Point point) { return bounds.containsPoint(point); }

        public bool containedInRectangle(Rect rect) { return bounds.containedInRectangle(rect); }

        public bool containsRectangle(Rect rect) { return bounds.containsRectangle(rect); }

        public List<IBounds> getBoundsList() { return bounds.getBoundsList(); }

        public void setBoundsList(List<IBounds> bounds) { this.bounds.setBoundsList(bounds); }
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
