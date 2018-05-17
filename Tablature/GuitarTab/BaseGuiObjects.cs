using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public abstract class BaseMouseHandler
    {
        public IMouseDelegate Delegate { get; set; }
        protected GuiCommandExecutor executor;

        public BaseMouseHandler(GuiCommandExecutor e, IMouseDelegate d)
        {
            Delegate = d;
            executor = e;
        }

        public void handleMouseEvent(MouseClick click)
        {
            if (click.matchesClickType(ClickType.Click)) { mouseClick(click as StandardClick); }
            else if (click.matchesClickType(ClickType.Release)) { mouseDragRelease(click as ReleaseClick); }
        }

        public abstract void mouseClick(StandardClick click);

        public abstract void mouseDragRelease(ReleaseClick click);

        public void invokeClickDelegate(MouseClick click) { Delegate?.invokeDelegate(click); }
    }

    public abstract class BaseBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }

        public BaseBounded(IDelegate del)
        {
            Delegate = del;
            Bounds = initBounds();
        }

        public virtual VisualBounds initBounds()
        {
            return new VisualBounds(0, 0, 0, 0, 0);
        }

        public abstract void updateBounds();

        public bool hitTest(Point point) { return Bounds.containsPoint(point); }
    }

    public abstract class TabDrawingVisual : DrawingVisual
    {
        public VisualBounds Bounds { get; set; }
        public IDelegate Delegate { get; set; }
        protected VisualInfo info;

        public TabDrawingVisual(VisualBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            Delegate = del;
            info = v_info;
            Transform = new TranslateTransform(Bounds.Left, Bounds.Top);

            Bounds.PropertyChanged += boundsPropertyChange;
        }

        public abstract void refreshDrawingContext(DrawingContext dc);

        public void refreshVisual()
        {
            var dc = RenderOpen();
            refreshDrawingContext(dc);
            dc.Close();
        }

        public void boundsPropertyChange(object sender, BoundsPropertyChangedEventArgs args)
        {
            if (!sender.Equals(Bounds)) { return; }

            if (args.RequiresVisualUpdate) { refreshVisual(); }
            else
            {
                var transform = Transform as TranslateTransform;

                if (args.PropertyName == nameof(VisualBounds.Left)) { transform.X = Bounds.Left; }
                else if (args.PropertyName == nameof(VisualBounds.Top)) { transform.Y = Bounds.Top; }
            }
        }
    }
}
