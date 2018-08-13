using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IHandleMouseEvents
    {
        IMouseDelegate MouseDelegate { get; set; }

        void handleMouseEvent(MouseClick click);
    }

    public class StaticMouseHandler : IHandleMouseEvents
    {
        public IMouseDelegate MouseDelegate { get; set; }
        protected IBounds bounds;

        public StaticMouseHandler(IMouseDelegate d, IBounds b)
        {
            MouseDelegate = d;
            bounds = b;
        }

        public virtual void handleMouseEvent(MouseClick click) { }

        public void invokeClickDelegate(MouseClick click)
        {
            bool handled = (click as NodeClick)?.Handled ?? false;
            if (!handled) { MouseDelegate?.invokeDelegate(click); }
        }
    }

    public class DynamicMouseHandler : IHandleMouseEvents
    {
        public IMouseDelegate MouseDelegate { get; set; }
        protected GuiCommandExecutor executor;
        protected IBounds bounds;

        public DynamicMouseHandler(GuiCommandExecutor e, IMouseDelegate d, IBounds b)
        {
            MouseDelegate = d;
            executor = e;
            bounds = b;
        }

        public void handleMouseEvent(MouseClick click)
        {
            if (click.matchesClickType(ClickType.Position)) { handlePositionClick(click as PositionClick); }
            else if (click.matchesClickType(ClickType.Select) && bounds.containedInRectangle((click as SelectClick).Rectangle))
            {
                handleSelectClick(click as SelectClick);
            }
            else if (bounds.containsPoint(click.Point))
            {
                (click as NodeClick)?.acceptPropAdd();

                if (click.matchesClickType(ClickType.Click)) { mouseClick(click as StandardClick); }
                else if (click.matchesClickType(ClickType.Bounds)) { handleBoundsClick(click as BoundsClick); }
                else if (click.matchesClickType(ClickType.Release)) { mouseDragRelease(click as ReleaseClick); }
                else if (click.matchesClickType(ClickType.NoteSelect)) { MouseDelegate?.invokeDelegate(click); }
            }
        }

        public void handleSelectClick(SelectClick click)
        {
            click.acceptPropAdd();
            click?.setContainsRect(bounds);
            if (click?.ContainsRect ?? false) { invokeClickDelegate(click); }
        }

        public virtual void handleBoundsClick(BoundsClick click)
        {
            click.DeepestBounds = bounds;
            invokeClickDelegate(click);
        }

        public virtual void handlePositionClick(PositionClick click) { }

        public virtual void mouseClick(StandardClick click) { }

        public virtual void mouseDragRelease(ReleaseClick click) { }

        public void invokeClickDelegate(MouseClick click)
        {
            bool handled = (click as NodeClick)?.Handled ?? false;
            if (!handled) { MouseDelegate?.invokeDelegate(click); }
        }
    }
}
