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
    public class MeasureBounds : IBounded
    {
        public const int MIN_SPACE = 300;

        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private VisualInfo info;

        private Measure measure;

        public MeasureBounds(Measure m, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            measure = m;
            info = v_info;
            Bounds = genBounds();
        }

        public VisualBounds genBounds()
        {
            return MultipleVisualBounds.createInstance(new List<VisualBounds>());
        }

        public Measure getMeasure() { return measure; }

        public void updateBounds()
        {
            getInitBoundValues(out int first_left, out int lowest_bar);
            Delegate?.invokeDelegate();
            getEndingBoundValues(lowest_bar, first_left, out int highest_bar, out int last_right);

            var bounds_list = new List<VisualBounds>();
            for (int i = lowest_bar; i <= highest_bar; i++) { bounds_list.Add(genSingleMeasureBounds(first_left, last_right, lowest_bar, highest_bar, i)); }

            (Bounds as MultipleVisualBounds)?.updateInstance(bounds_list);
        }
        
        public void getInitBoundValues(out int first_left, out int lowest_bar)
        {
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
        private Measure measure;
        private ChordPositionCheck chord_position;
        private MeasurePositionCheck measure_position;


        public MeasureMouseHandler(VisualBounds b, CommandSelections s, MouseSelections ms, GuiCommandExecutor e, Measure m, ChordPositionCheck cpos, MeasurePositionCheck mpos, IDelegate del)
            :base(b, s, ms, e, del)
        {
            measure = m;
            chord_position = cpos;
            measure_position = mpos;
        }

        public override void mouseClick()
        {
            addToCommandSelections();
            addToMouseSelections();

            if (mouse_selections.checkSelectionState(Selection.Add_Rest))
            {
                int position = performMousePositionCheck();
                executor.executeAddRestChordToMeasure(position);
            }

            invokeClickDelegate();

            if (mouse_selections.checkSelectionState(Selection.Add_Note))
            {
                int position = performMousePositionCheck();
                executor.executeAddNoteToMeasure(position);
            }
        }

        public override void mouseDragRelease()
        {
            addToCommandSelections();

            if (selections.SelectedChord.Count > 1 && !selections.SelectedNote.Any())
            {
                int position = performMousePositionCheck();
                executor.executeChangeMultipleChordPosition(position);
            }
            else if (selections.SelectedChord.Count == 1 && !selections.SelectedNote.Any())
            {
                int position = performMousePositionCheck();
                executor.executeChangeChordPosition(position);
            }

            invokeClickDelegate();

            if (selections.SelectedNote.Count == 1)
            {
                int position = performMousePositionCheck();
                executor.executeChangeNotePositionNewChord(position);
            }
        }

        public override void mouseDragSelect()
        {
            addToCommandSelections();

            if (bounds.containsRectangle(mouse_selections.SelectedRectangle)) { invokeClickDelegate(); }
            else { addToMouseSelections(); }
        }

        public override void addToCommandSelections() { selections.SelectedMeasure.Add(measure); }

        public override void addToMouseSelections() { mouse_selections.setToSingleSelectedObject(new ModelBoundsPair(bounds, measure)); }

        public int performMousePositionCheck()
        {
            chord_position.beginPositionCheck(mouse_selections);
            invokeClickDelegate();
            mouse_selections.PositionCheck = false;
            return chord_position.getPosition();
        }

        public override void mousePositionCheck()
        {
            measure_position.checkMeasure(measure, bounds);
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
            dc.DrawLine(info.DrawingObjects.Pen, new Point(0, 0), new Point(0, info.Dimensions.BarHeight));
            dc.DrawLine(info.DrawingObjects.Pen, new Point(Bounds.Width, Bounds.Height - info.Dimensions.BarHeight), new Point(Bounds.Width, Bounds.Height));

            if (!measure.MatchesPart)
            {
                drawMeasureBPM(measure.Bpm, dc);
                drawMeasureTimeSig(measure.TimeSignature.NumberOfBeats, measure.TimeSignature.BeatType, dc);
            }

            Delegate?.invokeDelegate();
        }

        public void drawMeasureBPM(int bpm, DrawingContext dc)
        {
            var text = new FormattedText("BPM = " + bpm.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.FontSize, info.DrawingObjects.Brush);
            dc.DrawText(text, new Point(info.Dimensions.MeasureHeadMargin, 0));
        }

        public void drawMeasureTimeSig(int num_notes, NoteLength note_type, DrawingContext dc)
        {
            var text = new FormattedText(num_notes.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.LargeFontSize, info.DrawingObjects.Brush);
            var second_text = new FormattedText(((int)note_type).ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.LargeFontSize, info.DrawingObjects.Brush);

            dc.DrawText(text, new Point(info.Dimensions.MeasureHeadMargin, info.Dimensions.StringHeight));
            dc.DrawText(text, new Point(info.Dimensions.MeasureHeadMargin, info.Dimensions.StringHeight * 3));
        }
    }
}
