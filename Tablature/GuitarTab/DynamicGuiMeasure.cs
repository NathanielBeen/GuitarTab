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
    public class DynamicMeasureMouseHandler : DynamicMouseHandler
    {
        private Measure measure;

        public DynamicMeasureMouseHandler(Measure m, DynamicBounds b, GuiCommandExecutor e, IMouseDelegate del) :base(e, del, b)
        {
            measure = m;
        }

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

        public override void handlePositionClick(PositionClick click) { click?.checkItem(measure.Position.Index, bounds); }

        public int performMousePositionCheck(MouseClick click)
        {
            var n_click = new ChordPositionClick(click.Point);
            invokeClickDelegate(n_click);
            return n_click.Position;
        }
    }

    public class DynamicMeasureDrawingVisual : DynamicDrawingVisual
    {
        public DynamicMeasureDrawingVisual(MeasureDrawingStrategy strategy) : base(strategy) { }

        public void refreshMeasure()
        {
            var dc = RenderOpen();
            (strategy as MeasureDrawingStrategy).drawMeasure(dc);
            dc.Close();
        }

        public override void boundsPropertyChange(object sender, BoundsPropertyChangedEventArgs args)
        {
            if (!sender.Equals(strategy.Bounds)) { return; }

            if (args.RequiresVisualUpdate) { refreshVisual(); }
            else
            {
                var transform = Transform as TranslateTransform;

                if (args.PropertyName == nameof(IBounds.Left))
                {
                    transform.X = strategy.Bounds.Left;
                    refreshMeasure();
                }
                else if (args.PropertyName == nameof(IBounds.Top)) { transform.Y = strategy.Bounds.Top; }
            }
        }
    }

    public class MeasureDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Measure measure;

        public MeasureDrawingStrategy(Measure m, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            measure = m;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            drawMeasure(dc);
           DrawDelegate?.invokeDelegate();
        }

        public void drawMeasure(DrawingContext dc)
        {
            int right = getLastRight();
            dc.DrawLine(info.DrawingObjects.Pen, new Point(0, info.Dimensions.StringHeight / 2), new Point(0, info.Dimensions.BarHeight - info.Dimensions.StringHeight / 2));
            dc.DrawLine(info.DrawingObjects.Pen, new Point(right, Bounds.Height - info.Dimensions.BarHeight + info.Dimensions.StringHeight / 2), new Point(right, Bounds.Height - info.Dimensions.StringHeight / 2));

            if (!measure.MatchesPrevMeasure)
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
            if (Bounds.getBoundsList().Count() <= 1) { return Bounds.Width; }
            else { return Bounds.getBoundsList().Last().Right - Bounds.Left; }
        }
    }
}
