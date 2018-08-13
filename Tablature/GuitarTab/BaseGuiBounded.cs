using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public interface IBounded
    {
        IBounds Bounds { get; set; }
        IDelegate BoundsDelegate { get; set; }
        IBoundedStrategy Strategy { get; }

        IBounds initBounds();
        bool hitTest(Point point);
        void updateBounds();
    }

    public interface IBoundedStrategy
    {
        IBounds Bounds { get; set; }
        IDelegate BoundsDelegate { get; set; }

        void updateBounds();
    }

    public class StaticSingleBounded : IBounded
    {
        public IBounds Bounds { get => Strategy.Bounds; set => Strategy.Bounds = value; }
        public IDelegate BoundsDelegate { get => Strategy.BoundsDelegate; set => Strategy.BoundsDelegate = value; }
        public IBoundedStrategy Strategy { get; }

        public StaticSingleBounded(IBoundedStrategy str)
        {
            Strategy = str;
            Bounds = initBounds();
        }

        public void updateBounds() { Strategy.updateBounds(); }

        public bool hitTest(Point point) { return Bounds.containsPoint(point); }

        public virtual IBounds initBounds() { return new SingleBounds(0, 0, 0, 0, 0); }
    }

    public class StaticMultiBounded : StaticSingleBounded
    {
        public StaticMultiBounded(IBoundedStrategy str) : base(str) { }

        public override IBounds initBounds() { return new MultipleBounds(new List<IBounds>()); }
    }

    public class DynamicSingleBounded : StaticSingleBounded
    {
        public DynamicSingleBounded(IBoundedStrategy str) : base(str) { }

        public override IBounds initBounds() { return new DynamicBounds(new SingleBounds(0, 0, 0, 0, 0)); }
    }

    public class DynamicMultiBounded : StaticSingleBounded
    {
        public DynamicMultiBounded(IBoundedStrategy str) : base(str) { }

        public override IBounds initBounds() { return new DynamicBounds(new MultipleBounds(new List<IBounds>())); }
    }
}
