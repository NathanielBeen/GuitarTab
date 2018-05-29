using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Globalization;

namespace GuitarTab
{
    public class MeasureBounds : BaseBounded
    {
        public const int MIN_SPACE = 200;

        private VisualInfo info;

        private Measure measure;

        public MeasureBounds(Measure m, VisualInfo v_info, IDelegate del)
            :base(del)
        {
            measure = m;
            info = v_info;
        }

        public override VisualBounds initBounds()
        {
            return MultipleVisualBounds.createInstance(new List<VisualBounds>());
        }

        public override void updateBounds()
        {
            getInitBoundValues(out int first_left, out int lowest_bar);
            Delegate?.invokeDelegate();
            getEndingBoundValues(lowest_bar, first_left, out int highest_bar, out int last_right);

            var bounds_list = new List<VisualBounds>();
            for (int i = lowest_bar; i <= highest_bar; i++) { bounds_list.Add(genSingleMeasureBounds(first_left, last_right, lowest_bar, highest_bar, i)); }

            (Bounds as MultipleVisualBounds).updateInstance(bounds_list);
        }
        
        public void getInitBoundValues(out int first_left, out int lowest_bar)
        {
            if (info.Position.X >= info.Dimensions.BarWidth + info.Dimensions.BarMargin)
            {
                info.Position.jumpToNextBar();
            }

            first_left = info.Position.X;
            lowest_bar = info.Position.CurrentBar;
            if (!measure.MatchesPart)
            {
                info.Position.incrementXPosition(info.Dimensions.MeasureHeadWidth);
            }
        }

        public void getEndingBoundValues(int lowest_bar, int first_left, out int highest_bar, out int last_right)
        {
            highest_bar = info.Position.CurrentBar;
            last_right = info.Position.X;

            int space_taken = last_right - first_left + (highest_bar - lowest_bar) * info.Dimensions.BarWidth;

            if (space_taken < MIN_SPACE)
            {
                info.Position.incrementXPositionForMeasure(MIN_SPACE - space_taken);
                highest_bar = info.Position.CurrentBar;
                last_right = info.Position.X;
            }
        }

        public VisualBounds genSingleMeasureBounds(int first_left, int last_right, int first_bar, int last_bar, int current_bar)
        {
            int left = (current_bar == first_bar) ? first_left : info.Dimensions.BarMargin;
            int width = (current_bar == last_bar) ? last_right - left : info.Dimensions.BarWidth + info.Dimensions.BarMargin - left;
            int top = info.Dimensions.PageHeadHeight + current_bar * info.Dimensions.LineHeight + info.Dimensions.EffectHeight;
            int height = info.Dimensions.BarHeight;

            return new VisualBounds(left, top, width, height, current_bar);
        }
    }

    public class MeasureMouseHandler : BaseMouseHandler
    {
        public MeasureMouseHandler(GuiCommandExecutor e, IMouseDelegate del) :base(e, del) { }

        public override void mouseClick(StandardClick click)
        {
            if (click.matchesSelectionType(Selection.Add_Rest))
            {
                int position = performMousePositionCheck(click);
                executor.executeAddRestChordToMeasure(click, position);
            }

            invokeClickDelegate(click);

            if (click.matchesSelectionType(Selection.Add_Note))
            {
                int position = performMousePositionCheck(click);
                executor.executeAddNoteToMeasure(click, position);
            }
        }

        public override void mouseDragRelease(ReleaseClick click)
        {
            if (click.multipleChords() && !click.anyNote())
            {
                int position = performMousePositionCheck(click);
                executor.executeChangeMultipleChordPosition(click, position);
            }
            else if (click.anyChord() && !click.anyNote())
            {
                int position = performMousePositionCheck(click) - 1;
                executor.executeChangeChordPosition(click, position);
            }

            invokeClickDelegate(click);

            if (click.anyNote())
            {
                int position = performMousePositionCheck(click);
                executor.executeChangeNotePositionNewChord(click, position);
            }
        }

        public int performMousePositionCheck(MouseClick click)
        {
            var n_click = new ChordPositionClick(click.Point);
            invokeClickDelegate(n_click);
            return n_click.Position;
        }
    }

    public class MeasureDrawingVisual : TabDrawingVisual
    {
        private Measure measure;

        public MeasureDrawingVisual(Measure m, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            measure = m;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            drawMeasure(dc);
            Delegate?.invokeDelegate();
        }

        public void refreshMeasure()
        {
            var dc = RenderOpen();
            drawMeasure(dc);
            dc.Close();
        }

        public void drawMeasure(DrawingContext dc)
        {
            int right = getLastRight();
            dc.DrawLine(info.DrawingObjects.Pen, new Point(0, info.Dimensions.StringHeight / 2), new Point(0, info.Dimensions.BarHeight - info.Dimensions.StringHeight / 2));
            dc.DrawLine(info.DrawingObjects.Pen, new Point(right, Bounds.Height - info.Dimensions.BarHeight + info.Dimensions.StringHeight / 2), new Point(right, Bounds.Height - info.Dimensions.StringHeight / 2));

            if (!measure.MatchesPart)
            {
                drawMeasureBPM(measure.Bpm, dc);
                drawMeasureTimeSig(measure.TimeSignature.NumberOfBeats, measure.TimeSignature.BeatType, dc);
            }
        }

        public void drawMeasureBPM(int bpm, DrawingContext dc)
        {
            var text = new FormattedText("BPM = " + bpm.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.FontSize, info.DrawingObjects.Brush);
            dc.DrawText(text, new Point(0, -10));
        }

        public void drawMeasureTimeSig(int num_notes, NoteLength note_type, DrawingContext dc)
        {
            var text = new FormattedText(num_notes.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.LargeFontSize, info.DrawingObjects.Brush);
            var second_text = new FormattedText((note_type.getVisualNoteLength()).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.LargeFontSize, info.DrawingObjects.Brush);

            dc.DrawText(text, new Point(5, info.Dimensions.StringHeight));
            dc.DrawText(second_text, new Point(5, info.Dimensions.StringHeight * 3));
        }

        public int getLastRight()
        {
            var multi_bounds = Bounds as MultipleVisualBounds;
            if (multi_bounds is null || multi_bounds.AllBounds.Count() <= 1) { return Bounds.Width; }
            else { return multi_bounds.AllBounds.Last().Right - Bounds.Left; }
        }

        public override void boundsPropertyChange(object sender, BoundsPropertyChangedEventArgs args)
        {
            if (!sender.Equals(Bounds)) { return; }

            if (args.RequiresVisualUpdate) { refreshVisual(); }
            else
            {
                var transform = Transform as TranslateTransform;

                if (args.PropertyName == nameof(VisualBounds.Left))
                {
                    transform.X = Bounds.Left;
                    refreshMeasure();
                }
                else if (args.PropertyName == nameof(VisualBounds.Top)) { transform.Y = Bounds.Top; }
            }
        }
    }
}
