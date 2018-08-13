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
    public class DynamicPartBounds : PartBounds
    {
        public DynamicPartBounds(Part p,  VisualInfo v_info, IDelegate del) : base(p, v_info, del) { }

        public override void handleDimensionUpdate(int value, DimensionType type)
        {
            if (type == DimensionType.PageHeight) { Bounds.Height = value; }
            else if (type == DimensionType.PageWidth)
            {
                info.Position.setCurrentLeftAndResetPosition(value);
                updateBounds();
            }
        }
    }

    public class DynamicPartMouseHandler : DynamicMouseHandler
    {
        public DynamicPartMouseHandler(IBounds b, GuiCommandExecutor executor, IMouseDelegate del) :base(executor, del, b) { }

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

        public override void handleBoundsClick(BoundsClick click)
        {
            if (bounds.containsPoint(click.Point)) { invokeClickDelegate(click); }
        }

        public MeasurePositionClick performMousePositionCheck(MouseClick click)
        {
            var n_click = new MeasurePositionClick(click.Point);
            invokeClickDelegate(n_click);
            return n_click;
        }
    }

    public class PartDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Part part;

        public PartDrawingStrategy(Part p, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            part = p;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            drawBars(dc);
            drawSongInfo(dc);
        }

        public void drawBars(DrawingContext dc)
        {
            int num_strings = 6;
            int y_val = info.Dimensions.PageHeadHeight + info.Dimensions.EffectHeight + info.Dimensions.StringHeight / 2;
            while (y_val < Bounds.Height)
            {
                for (int j = 0; j < num_strings; j++)
                {
                    dc.DrawLine(info.DrawingObjects.Pen, new Point(info.Dimensions.BarMargin, y_val), new Point(info.Dimensions.BarMargin + info.Dimensions.BarWidth, y_val));
                    y_val += info.Dimensions.StringHeight;
                }
                y_val += info.Dimensions.BarringHeight + info.Dimensions.EffectHeight;
            }
        }

        public void drawSongInfo(DrawingContext dc)
        {
            var song_text = new FormattedText(part.SongInfo.SongName, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.LargeFontSize, info.DrawingObjects.Brush);
            var artist_text = new FormattedText("By "+part.SongInfo.ArtistName, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.LargeFontSize, info.DrawingObjects.Brush);

            dc.DrawText(song_text, new Point((info.Dimensions.PageWidth - song_text.Width) / 2, 20));
            dc.DrawText(artist_text, new Point((info.Dimensions.PageWidth - artist_text.Width) / 2, 30 + song_text.Height));
        }
    }
}
