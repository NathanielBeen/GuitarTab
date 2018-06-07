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
                MeasurePositionClick pos_click = performMousePositionCheck(click);
                executor.executeAddMeasureToPart(click, pos_click);
            }

            invokeClickDelegate(click);

            if (click.matchesSelectionType(Selection.Add_Rest))
            {
                executor.executeAddRestChordToPart(click);
            }
            else if (click.matchesSelectionType(Selection.Add_Note))
            {
                executor.executeAddNoteToPart(click);
            }
        }

        public override void mouseDragRelease(ReleaseClick click)
        {
            if (click.multipleMeasures() && !click.anyChord())
            {
                MeasurePositionClick pos_click = performMousePositionCheck(click);
                executor.executeChangeMultipleMeasurePosition(click, pos_click);
            }
            else if (click.anyMeasure() && !click.anyChord())
            {
                MeasurePositionClick pos_click = performMousePositionCheck(click);
                executor.executeChangeMeasurePosition(click, pos_click);
            }

            invokeClickDelegate(click);

            if (click.anyNote())
            {
                executor.executeChangeNotePositionNewMeasure(click);
            }
            else if (click.multipleChords())
            {
                executor.executeChangeMultipleChordPositionNewMeasure(click);
            }
            else if (click.anyChord())
            {
                executor.executeChangeChordPositionNewMeasure(click);
            }
        }

        public MeasurePositionClick performMousePositionCheck(MouseClick click)
        {
            var n_click = new MeasurePositionClick(click.Point);
            invokeClickDelegate(n_click);
            return n_click;
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
