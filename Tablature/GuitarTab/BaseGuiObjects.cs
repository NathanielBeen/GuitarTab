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
        public IDelegate Delegate { get; set; }
        protected VisualBounds bounds;
        protected CommandSelections selections;
        protected MouseSelections mouse_selections;
        protected GuiCommandExecutor executor;

        public BaseMouseHandler(VisualBounds b, CommandSelections s, MouseSelections m, GuiCommandExecutor e, IDelegate d)
        {
            Delegate = d;
            bounds = b;
            selections = s;
            mouse_selections = m;
            executor = e;
        }

        public bool hitTest(Point point) { return bounds.containsPoint(point); }

        public void handleMouseEvent()
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
                    mouseClick();
                    return;
                case MouseSelections.DRAG_RELEASE:
                case MouseSelections.MULTIPLE_DRAG_RELEASE:
                    mouseDragRelease();
                    return;
                case MouseSelections.DRAG_SELECT:
                    mouseDragSelect();
                    return;
                default:
                    return;
            }
        }

        public abstract void mouseClick();

        public abstract void mouseDragRelease();

        public abstract void mouseDragSelect();

        public abstract void mousePositionCheck();

        public abstract void addToCommandSelections();

        public abstract void addToMouseSelections();

        public void invokeClickDelegate()
        {
            if (!mouse_selections.EventHandled) { Delegate?.invokeDelegate(); }
        }
    }

    public interface IBounded
    {
        IDelegate Delegate { get; set; }
        VisualBounds Bounds { get; set; }

        void updateBounds();
    }

    public class TabDrawingVisual : DrawingVisual
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

        public void refreshVisual()
        {
            var dc = RenderOpen();
            refreshDrawingContext(dc);
            dc.Close();
        }

        public virtual void refreshDrawingContext(DrawingContext dc) { }

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

    public class ModelBoundsPair
    {
        public VisualBounds Bounds { get; set; }
        public object Base { get; set; }

        public ModelBoundsPair(VisualBounds bounds, object base_obj)
        {
            Bounds = bounds;
            Base = base_obj;
        }
    }
}
