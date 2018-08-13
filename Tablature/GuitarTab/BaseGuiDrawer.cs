using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuitarTab
{
    public interface IDraw
    {
        IDelegate DrawDelegate { get; set; }

        void refreshVisual();
        DrawingVisual getVisual();
    }

    public interface IDrawStrategy
    {
        IBounds Bounds { get; set; }
        IDelegate DrawDelegate { get; set; }

        void refreshDrawingContext(DrawingContext dc);
    }

    public class StaticDrawingVisual : DrawingVisual, IDraw
    {
        public IDelegate DrawDelegate
        {
            get { return strategy.DrawDelegate; }
            set { strategy.DrawDelegate = value; }
        }
        protected IDrawStrategy strategy;

        public StaticDrawingVisual(IDrawStrategy str)
        {
            strategy = str;
            Transform = new TranslateTransform(strategy.Bounds.Left, strategy.Bounds.Top);
        }

        public virtual void refreshVisual()
        {
            var dc = RenderOpen();
            strategy.refreshDrawingContext(dc);
            dc.Close();
            Transform = new TranslateTransform(strategy.Bounds.Left, strategy.Bounds.Top);
            DrawDelegate?.invokeDelegate();
        }

        public DrawingVisual getVisual() { return this; }
    }

    public class DynamicDrawingVisual : StaticDrawingVisual
    {
        public DynamicDrawingVisual(IDrawStrategy strategy)
            : base(strategy)
        {
            if (strategy.Bounds is DynamicBounds)
            {
                (strategy.Bounds as DynamicBounds).PropertyChanged += boundsPropertyChange;
            }
        }

        public override void refreshVisual()
        {
            var dc = RenderOpen();
            strategy.refreshDrawingContext(dc);
            dc.Close();
        }

        public virtual void boundsPropertyChange(object sender, BoundsPropertyChangedEventArgs args)
        {
            if (!sender.Equals(strategy.Bounds)) { return; }

            if (args.RequiresVisualUpdate) { refreshVisual(); }
            else
            {
                var transform = Transform as TranslateTransform;

                if (args.PropertyName == nameof(IBounds.Left)) { transform.X = strategy.Bounds.Left; }
                else if (args.PropertyName == nameof(IBounds.Top)) { transform.Y = strategy.Bounds.Top; }
            }
        }
    }
}
