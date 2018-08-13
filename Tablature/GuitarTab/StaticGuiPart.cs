using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public interface IPartBounds : IBoundedStrategy, IRecieveDimensionUpdates { }

    public class PartBounds : IPartBounds
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        protected Part part;
        protected VisualInfo info;

        public PartBounds(Part p, VisualInfo v_info, IDelegate del)
        {
            BoundsDelegate = del;
            part = p;
            info = v_info;
        }

        public void updateBounds()
        {
            Bounds.Left = info.Position.CurrentLeft;
            Bounds.Top = 0;
            Bounds.Width = info.Dimensions.PageWidth;
            Bounds.Bar = 0;

            BoundsDelegate?.invokeDelegate();
        }

        public virtual void handleDimensionUpdate(int value, DimensionType type)
        {
            if (type == DimensionType.PageHeight) { Bounds.Height = value; }
            else if (type == DimensionType.PageWidth)
            {
                info.Position.setCurrentLeftAndResetPosition(value);
                updateBounds();
            }
        }
    }

    public class StaticPartMouseHandler : StaticMouseHandler
    {
        public StaticPartMouseHandler(IBounds b, IMouseDelegate del) :base(del, b) { }

        public override void handleMouseEvent(MouseClick click)
        {
            invokeClickDelegate(click);
        }
    }
}
