using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class ChordPositionCheck
    {
        private Point click_point;
        public ModelBoundsPair CurrentClosest { get; private set; }

        public ChordPositionCheck() { }

        public void beginPositionCheck(MouseSelections selections)
        {
            selections.PositionCheck = true;
            click_point = selections.SelectedPoint;
            CurrentClosest = null;
        }

        public void checkChord(Chord chord, VisualBounds bounds)
        {
            if (checkChordBar(bounds))
            {
                CurrentClosest = new ModelBoundsPair(bounds, chord);
            }
        }

        public bool checkChordBar(VisualBounds bounds)
        {
            if (bounds.Top <= click_point.Y && bounds.Top + bounds.Height >= click_point.Y
                && bounds.Left + bounds.Width <= click_point.X) { return checkSameBar(bounds); }
            else if (bounds.Top + bounds.Height <= click_point.Y) { return checkPrevBar(bounds); }
            return false;
        }

        public bool checkSameBar(VisualBounds bounds)
        {
            if (CurrentClosest is null) { return true; }
            return isFurtherRightThanClosest(bounds);
        }

        public bool checkPrevBar(VisualBounds bounds)
        {
            if (CurrentClosest is null) { return true; }
            if (CurrentClosest.Bounds.Bar > bounds.Bar) { return false; }
            return isFurtherRightThanClosest(bounds);
        }

        public bool isFurtherRightThanClosest(VisualBounds bounds)
        {
            return (CurrentClosest.Bounds.Left + CurrentClosest.Bounds.Width <=
                    bounds.Left + bounds.Width);
        }

        public int getPosition()
        {
            if (CurrentClosest is null) { return 0; }
            else { return ((Chord)CurrentClosest.Base).Position.Index + 1; }
        }
    }

    public class MeasurePositionCheck
    {
        private Point click_point;
        private Measure ClickedMeasure { get; set; }

        public MeasurePositionCheck() { }

        public void beginPositionCheck(MouseSelections selections)
        {
            selections.PositionCheck = true;
            click_point = selections.SelectedPoint;
        }

        public void checkMeasure(Measure measure, VisualBounds bounds)
        {
            if (bounds.containsPoint(click_point)) { ClickedMeasure = measure; }
        }

        public int getPosition()
        {
            return ClickedMeasure?.Position.Index ?? 0;
        }
    }
}
