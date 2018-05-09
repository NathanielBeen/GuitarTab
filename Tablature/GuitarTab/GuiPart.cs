using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public class PartBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private Part part;
        private VisualInfo info;

        public PartBounds(Part p,  VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            part = p;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds()
        {
            int width = info.Dimensions.PageWidth;
            int height = info.Dimensions.PageHeight;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public void updateBounds()
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
        private Part part;
        private MeasurePositionCheck position;

        public PartMouseHandler(VisualBounds bounds, CommandSelections command, MouseSelections mouse, GuiCommandExecutor executor, Part pa, MeasurePositionCheck pos, IDelegate del)
            :base(bounds, command, mouse, executor, del)
        {
            part = pa;
            position = pos;
        }

        public override void mouseClick()
        {
            addToCommandSelections();
            addToMouseSelections();

            if (mouse_selections.checkSelectionState(Selection.Add_Measure))
            {
                executor.executeAddMeasureToPart(performMousePositionCheck());
            }

            invokeClickDelegate();

            if (mouse_selections.checkSelectionState(Selection.Add_Rest))
            {
                executor.executeAddRestChordToPart();
            }
            else if (mouse_selections.checkSelectionState(Selection.Add_Note))
            {
                executor.executeAddNoteToPart();
            }
        }

        public override void mouseDragRelease()
        {
            addToCommandSelections();

            if (selections.SelectedMeasure.Count > 1 && !selections.SelectedChord.Any())
            {
                int position = performMousePositionCheck();
                executor.executeChangeMultipleMeasurePosition(position);
            }
            else if (selections.SelectedMeasure.Count == 1 && !selections.SelectedChord.Any())
            {
                int position = performMousePositionCheck();
                executor.executeChangeMeasurePosition(position);
            }

            invokeClickDelegate();

            if (selections.SelectedNote.Count == 1)
            {
                executor.executeChangeNotePositionNewMeasure();
            }
            else if (selections.SelectedChord.Count == 1)
            {
                executor.executeChangeChordPositionNewMeasure();
            }
            else if (selections.SelectedChord.Count > 1)
            {
                executor.executeChangeMultipleChordPositionNewMeasure();
            }
        }

        public override void mouseDragSelect()
        {
            if (bounds.containsRectangle(mouse_selections.SelectedRectangle)) { invokeClickDelegate(); }
        }

        public override void addToCommandSelections() { selections.SelectedPart = part; }

        public override void addToMouseSelections() { mouse_selections.setToSingleSelectedObject(new ModelBoundsPair(bounds, part)); }

        public override void mousePositionCheck() { }

        public int performMousePositionCheck()
        {
            position.beginPositionCheck(mouse_selections);
            invokeClickDelegate();
            mouse_selections.PositionCheck = false;
            return position.getPosition();
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
            int y_val = info.Position.Y + info.Dimensions.EffectHeight;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j < num_strings; j++)
                {
                    dc.DrawLine(info.DrawingObjects.Pen, new Point(info.Position.X, y_val), new Point(info.Position.X + Bounds.Width, y_val));
                    y_val += info.Dimensions.StringHeight;
                }
                y_val += info.Dimensions.BarringHeight + info.Dimensions.EffectHeight - info.Dimensions.StringHeight;
            }
        }
    }
}
