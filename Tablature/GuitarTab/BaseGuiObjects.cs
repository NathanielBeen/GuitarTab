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
        protected VisualBounds bounds;
        protected CommandSelections selections;
        protected MouseSelections mouse_selections;
        protected GuiCommandExecutor executor;

        public BaseMouseHandler(VisualBounds b, CommandSelections s, MouseSelections m, GuiCommandExecutor e, IMouseDelegate d)
        {
            Delegate = d;
            bounds = b;
            selections = s;
            mouse_selections = m;
            executor = e;
        }

        public void handleMouseEvent(MouseClick click)
        {
            if (mouse_selections.PositionCheck)
            {
                mousePositionCheck();
                return;
            }
            if (!hitTest(mouse_selections.SelectedPoint)) { return; }

            switch (mouse_selections.EventType)
            {
                case MouseSelections.CLICK:
                    mouseClick(click);
                    return;
                case MouseSelections.DRAG_RELEASE:
                case MouseSelections.MULTIPLE_DRAG_RELEASE:
                    mouseDragRelease(click);
                    return;
                case MouseSelections.DRAG_SELECT:
                    mouseDragSelect(click);
                    return;
                default:
                    return;
            }
        }

        public abstract void mouseClick(MouseClick click);

        public abstract void mouseDragRelease(MouseClick click);

        public abstract void mouseDragSelect(MouseClick click);

        public abstract void mousePositionCheck();

        public abstract void addToCommandSelections();

        public abstract void addToMouseSelections();

        public void invokeClickDelegate(MouseClick click)
        {
            if (!mouse_selections.EventHandled) { Delegate?.invokeDelegate(click); }
        }
    }

    public abstract class BaseBounded
    {
        IDelegate Delegate { get; set; }
        VisualBounds Bounds { get; set; }

        public BaseBounded(IDelegate del, VisualBounds bounds)
        {
            Delegate = del;
            Bounds = bounds;
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
