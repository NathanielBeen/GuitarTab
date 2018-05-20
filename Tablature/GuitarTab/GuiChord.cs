﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class ChordBounds : BaseBounded
    {
        public ChordBar ChordBar { get; set; }

        private Chord chord;
        private VisualInfo info;

        public ChordBounds(Chord c, VisualInfo v_info, IDelegate del)
            :base(del)
        {
            ChordBar = new ChordBar(c.Length.NoteType);

            chord = c;
            info = v_info;
        }

        public override void updateBounds()
        {
            Bounds.Left = info.Position.X;
            Bounds.Top = info.Position.Y + info.Dimensions.EffectHeight;
            Bounds.Width = info.Dimensions.NoteWidth;
            Bounds.Height = info.Dimensions.BarHeight;
            Bounds.Bar = info.Position.CurrentBar;

            Delegate?.invokeDelegate();

            info.Position.incrementXPosition(info.Dimensions.NoteWidth + info.Dimensions.getLength(chord.Length.NoteType));
        }

        public Chord getChord() { return chord; }
    }

    public class ChordMouseHandler : BaseMouseHandler
    {
        private Chord chord;

        public ChordMouseHandler(Chord c, GuiCommandExecutor e, IMouseDelegate del)
            : base(e, del)
        {
            chord = c;
        }

        public override void mouseClick(StandardClick click)
        {
            if (click.matchesSelectionType(Selection.Set_Length))
            {
                executor.executeChangeChordLength(click);
            }
            else if (click.matchesSelectionType(Selection.Add_Note))
            { 
                executor.executeAddNoteToChord(click);
            }

            invokeClickDelegate(click);
        }

        public override void mouseDragRelease(ReleaseClick click)
        {
            if (click.anyNote() && click.chordMatchesFirst(chord))
            {
                executor.executeChangeNoteStringFromPosition(click);
            }
            else if (click.anyNote() && !click.chordMatchesFirst(chord))
            {
                executor.executeChangeNotePosition(click);
            }

            invokeClickDelegate(click);
        }
    }

    public class ChordDrawingVisual : TabDrawingVisual
    {
        protected Chord chord;
        protected ChordBar bar;
        
        public ChordDrawingVisual(Chord c, ChordBar b, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            chord = c;
            bar = b;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            int center_x = Bounds.Width / 2;
            int top_y = Bounds.Height + info.Dimensions.BarringMargin;
            int curr_y = top_y + info.Dimensions.BarringHeight - 2 * info.Dimensions.BarringMargin;
            int note_width = info.Dimensions.getLength(chord.Length.NoteType);

            drawChordVerticalLine(dc, center_x, top_y, curr_y);
            drawAllChordBars(dc, center_x, note_width, ref curr_y);
            drawChordDot(dc, center_x, curr_y);
        }
        public void drawChordVerticalLine(DrawingContext dc, int center_x, int top_y, int bottom_y)
        {
            Point top_point = new Point(center_x, top_y);
            Point bottom_point = new Point(center_x, bottom_y);
            dc.DrawLine(info.DrawingObjects.Pen, top_point, bottom_point);
        }

        public void drawAllChordBars(DrawingContext dc, int center_x, int note_width, ref int curr_y)
        {
            int right_conn_x = center_x + info.Dimensions.NoteWidth + note_width;
            int right_single_x = center_x + info.Dimensions.SingleBarLength;
            int left_single_x = center_x - info.Dimensions.SingleBarLength;
            int bottom_y = curr_y;

            for (int i = 0; i < bar.RightConnected; i++)
            {
                drawChordBar(dc, center_x, right_conn_x, ref curr_y);
            }
            for (int i = 0; i < bar.RightSingle; i++)
            {
                drawChordBar(dc, center_x, right_single_x, ref curr_y);
            }

            curr_y = bottom_y - info.Dimensions.BarSpacing * bar.LeftConnected;

            for (int i = 0; i < bar.LeftSingle; i++)
            {
                drawChordBar(dc, center_x, left_single_x, ref curr_y);
            }
        }

        public void drawChordBar(DrawingContext dc, int center_x, int other_x, ref int curr_y)
        {
            Point left_point = new Point(center_x, curr_y);
            Point right_point = new Point(other_x, curr_y);
            dc.DrawLine(info.DrawingObjects.BarringPen, left_point, right_point);
            curr_y -= info.Dimensions.BarSpacing;
        }

        public void drawChordDot(DrawingContext dc, int center_x, int y_val)
        {
            if (!bar.LeftDot && !bar.RightDot) { return; }
            int x = (bar.LeftDot) ? center_x - info.Dimensions.DotMargin - info.Dimensions.DotSize : center_x + info.Dimensions.DotMargin + info.Dimensions.DotSize;
            int y = y_val - info.Dimensions.DotSize;

            dc.DrawEllipse(info.DrawingObjects.Brush, info.DrawingObjects.Pen, new Point(x, y), info.Dimensions.DotSize, info.Dimensions.DotSize);
        }
    }

    public class RestChordDrawingVisual : ChordDrawingVisual
    {
        public RestChordDrawingVisual(Chord c, ChordBar b, VisualBounds bounds, VisualInfo v_info, IDelegate del) : base(c, b, bounds, v_info, del) { }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var length = chord.Length.NoteType;
            var image = new BitmapImage(info.Images.getRestImagePath(length));
            int top = getTopRestPosition((int)image.Height);
            var dest_rect = new Rect(Bounds.Width/2 - image.Width/2, top, image.Width, image.Height);

            dc.DrawImage(image, dest_rect);
            base.refreshDrawingContext(dc);
        }

        public int getTopRestPosition(int image_height)
        {
            if (chord.Length.NoteType == NoteLength.Whole || chord.Length.NoteType == NoteLength.DottedWhole)
            {
                return info.Dimensions.StringHeight * 2;
            }
            else if (chord.Length.NoteType == NoteLength.Half || chord.Length.NoteType == NoteLength.DottedHalf)
            {
                return info.Dimensions.StringHeight * 3 - image_height;
            }
            else { return Bounds.Height / 2 - image_height / 2; }
        }
    }
}