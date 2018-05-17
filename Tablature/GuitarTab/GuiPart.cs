using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public class PartBounds : BaseBounded
    {
        private Part part;
        private VisualInfo info;

        public PartBounds(Part p,  VisualInfo v_info, IDelegate del)
            : base(del)
        {
            part = p;
            info = v_info;
        }

        public override VisualBounds initBounds()
        {
            int width = info.Dimensions.PageWidth;
            int height = info.Dimensions.PageHeight;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public override void updateBounds()
        {
            Bounds.Left = 0;
            Bounds.Top = 0;
            Bounds.Width = info.Dimensions.PageWidth;
            Bounds.Height = info.Dimensions.PageHeight;
            Bounds.Bar = info.Position.CurrentBar;

            Delegate?.invokeDelegate();
        }
    }

    public class PartMouseHandler : BaseMouseHandler
    {
        public PartMouseHandler(GuiCommandExecutor executor, IMouseDelegate del) :base(executor, del) { }

        public override void mouseClick(StandardClick click)
        {
            if (click.matchesSelectionType(Selection.Add_Measure))
            {
                int position = performMousePositionCheck(click);
                executor.executeAddMeasureToPart(position);
            }

            invokeClickDelegate(click);

            if (click.matchesSelectionType(Selection.Add_Rest))
            {
                executor.executeAddRestChordToPart();
            }
            else if (click.matchesSelectionType(Selection.Add_Note))
            {
                executor.executeAddNoteToPart();
            }
        }

        public override void mouseDragRelease(ReleaseClick click)
        {
            if (click.multipleMeasures() && !click.anyChord())
            {
                int position = performMousePositionCheck(click);
                executor.executeChangeMultipleMeasurePosition(position);
            }
            else if (click.anyMeasure() && !click.anyChord())
            {
                int position = performMousePositionCheck(click);
                executor.executeChangeMeasurePosition(position);
            }

            invokeClickDelegate(click);

            if (click.anyNote())
            {
                executor.executeChangeNotePositionNewMeasure();
            }
            else if (click.multipleChords())
            {
                executor.executeChangeMultipleChordPositionNewMeasure();
            }
            else if (click.anyChord())
            {
                executor.executeChangeChordPositionNewMeasure();
            }
        }

        public int performMousePositionCheck(MouseClick click)
        {
            var n_click = new MeasurePositionClick(click.Point);
            invokeClickDelegate(n_click);
            return n_click.Position;
        }
    }

    public class PartDrawingVisual : TabDrawingVisual
    {
        private Part part;

        public PartDrawingVisual(Part p, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            part = p;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            //replace with variable
            int num_strings = 6;
            int y_val = info.Position.Y + info.Dimensions.EffectHeight + info.Dimensions.StringHeight / 2;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < num_strings; j++)
                {
                    dc.DrawLine(info.DrawingObjects.Pen, new Point(info.Position.X, y_val), new Point(info.Position.X + info.Dimensions.BarWidth, y_val));
                    y_val += info.Dimensions.StringHeight;
                }
                y_val += info.Dimensions.BarringHeight + info.Dimensions.EffectHeight;
            }
        }
    }
}
