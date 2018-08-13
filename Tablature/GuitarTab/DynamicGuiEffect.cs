using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class DynamicEffectMouseHandler : DynamicMouseHandler
    {
        public DynamicEffectMouseHandler(IBounds b, GuiCommandExecutor g, IMouseDelegate del)
            :base(g, del, b) { }
    }

    public class PalmMuteDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private PalmMute palm_mute;

        public PalmMuteDrawingStrategy(PalmMute pm, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            palm_mute = pm;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            var text = new FormattedText("P.M.", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.FontSize, info.DrawingObjects.Brush);
            dc.DrawText(text, new Point(0, 0));
        }
    }

    public class BendDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Bend bend;

        public BendDrawingStrategy(Bend b, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            bend = b;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            Rect bend_rect = new Rect(0, 0, Bounds.Width / 2, Bounds.Height);
            dc.DrawLine(info.DrawingObjects.Pen, new Point(0, Bounds.Height), new Point(Bounds.Width / 4, Bounds.Height));
            dc.DrawLine(info.DrawingObjects.Pen, new Point(Bounds.Width / 4, Bounds.Height), new Point(Bounds.Width / 2, 0));


            if (bend.BendReturns)
            {
                dc.DrawLine(info.DrawingObjects.Pen, new Point(Bounds.Width / 2, 0),
                            new Point(Bounds.Width / 2, Bounds.Height));
            }

            var text = new FormattedText(bend.Amount.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.SmallFontSize, info.DrawingObjects.Brush);
            dc.DrawText(text, new Point(Bounds.Width / 2+2, 2));
        }
    }

    public class PinchHarmonicDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private PinchHarmonic pinch_harmonic;

        public PinchHarmonicDrawingStrategy(PinchHarmonic ph, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            pinch_harmonic = ph;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            var image = new BitmapImage(info.Images.getEffectImagePath(TabImages.PINCH_HARMONIC));
            var dest_rect = new Rect(Bounds.Width / 2 - image.Width / 2, Bounds.Height / 2 - image.Height / 2, image.Width, image.Height);
            dc.DrawImage(image, dest_rect);
        }
    }

    public class VibratoDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Vibrato vibrato;

        public VibratoDrawingStrategy(Vibrato v, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            vibrato = v;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            if (Bounds.Width == 0 || Bounds.Height == 0) { return; }
            var image_code = vibrato.Wide ? TabImages.WIDE_VIBRATO : TabImages.VIBRATO;
            var image = new BitmapImage(info.Images.getEffectImagePath(image_code));
            var image_rect = new Int32Rect(0, 0, Bounds.Width, Bounds.Height);
            var cropped_image = new CroppedBitmap(image, image_rect);

            dc.DrawImage(cropped_image, new Rect(image_rect.X, image_rect.Y, image_rect.Width, image_rect.Height));
        }
    }

    public class SlideDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Slide slide;

        public SlideDrawingStrategy(Slide s, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            slide = s;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            bool slides_up = slide.First.Fret < slide.Second.Fret;

            Point point_a, point_b;
            if (slides_up)
            {
                point_a = new Point(0, Bounds.Height);
                point_b = new Point(Bounds.Width, 0);
            }
            else
            {
                point_a = new Point(0, 0);
                point_b = new Point(Bounds.Width, Bounds.Height);
            }
            dc.DrawLine(info.DrawingObjects.Pen, point_a, point_b);
        }
    }

    public class HOPODrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private HOPO hopo;

        public HOPODrawingStrategy(HOPO h, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            hopo = h;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
            dc.DrawArc(info.DrawingObjects.Pen, info.DrawingObjects.Brush, rect, 200, 140);
        }
    }

    public class TieDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Tie tie;

        public TieDrawingStrategy(Tie t, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            tie = t;
        }

        public void refreshDrawingContext(DrawingContext dc)
        {
            var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
            dc.DrawArc(info.DrawingObjects.Pen, info.DrawingObjects.Brush, rect, 160, -140);
        }
    }

    public static class GuiEffectBoundDrawing
    {
        public static void updateAboveNoteEffectBounds(IBounds effect_bounds, IBounds note_bounds, Note note, VisualInfo info)
        {
            int right = Math.Min(note_bounds.Right + info.Dimensions.getLength(note.Length.NoteType) + info.Dimensions.NoteWidth / 2,
                info.Dimensions.BarMargin + info.Dimensions.BarWidth + info.Position.CurrentLeft);

            effect_bounds.Left = note_bounds.Left + info.Dimensions.NoteWidth/2;
            effect_bounds.Top = note_bounds.Top - info.Dimensions.StringHeight/3;
            effect_bounds.Width = right - (note_bounds.Left + info.Dimensions.NoteWidth/2);
            effect_bounds.Height = info.Dimensions.StringHeight;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static void updateAboveLineEffectBounds(IBounds effect_bounds, IBounds note_bounds, Note note, VisualInfo info)
        {
            int right = Math.Min(note_bounds.Left + note_bounds.Width, info.Dimensions.BarMargin + info.Dimensions.BarWidth + info.Position.CurrentLeft);

            effect_bounds.Left = note_bounds.Left;
            effect_bounds.Top = info.Dimensions.PageHeadHeight + info.Dimensions.LineHeight * note_bounds.Bar + info.Dimensions.EffectMargin + info.Dimensions.EffectHeight/4;
            effect_bounds.Width = right - note_bounds.Left;
            effect_bounds.Height = info.Dimensions.EffectHeight - info.Dimensions.EffectMargin * 2;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static void updateInLineEffectBounds(IBounds effect_bounds, IBounds note_bounds, Note note, VisualInfo info)
        {
            int right = Math.Min(note_bounds.Right + info.Dimensions.getLength(note.Length.NoteType), info.Dimensions.BarMargin + info.Dimensions.BarWidth + info.Position.CurrentLeft);

            effect_bounds.Left = note_bounds.Right;
            effect_bounds.Top = note_bounds.Top - info.Dimensions.StringHeight/2;
            effect_bounds.Width = right - note_bounds.Right;
            effect_bounds.Height = note_bounds.Height + info.Dimensions.StringHeight/2;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static void updateHalvedInLineEffectBounds(IBounds effect_bounds, IBounds note_bounds, Note note, VisualInfo info)
        {
            int right = Math.Min(note_bounds.Right + info.Dimensions.getLength(note.Length.NoteType), info.Dimensions.BarMargin + info.Dimensions.BarWidth + info.Position.CurrentLeft);

            effect_bounds.Left = note_bounds.Right;
            effect_bounds.Top = note_bounds.Top + note_bounds.Height/4;
            effect_bounds.Width = right - note_bounds.Right;
            effect_bounds.Height = note_bounds.Height/2;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static void updateBelowLineEffectBounds(IBounds effect_bounds, IBounds note_bounds, Note note, VisualInfo info)
        {
            int right = Math.Min(note_bounds.Right + info.Dimensions.getLength(note.Length.NoteType) + info.Dimensions.NoteWidth / 2,
                info.Dimensions.BarMargin + info.Dimensions.BarWidth + info.Position.CurrentLeft);

            effect_bounds.Left = note_bounds.Left + info.Dimensions.NoteWidth / 2;
            effect_bounds.Top = note_bounds.Bottom - info.Dimensions.StringHeight * 2 / 3;
            effect_bounds.Width = right - (note_bounds.Left + info.Dimensions.NoteWidth / 2);
            effect_bounds.Height = info.Dimensions.StringHeight;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static void updateBeforeNoteEffectBounds(IBounds effect_bounds, IBounds note_bounds, Note note, VisualInfo info)
        {
            effect_bounds.Left = note_bounds.Left - info.Dimensions.NoteWidth;
            effect_bounds.Top = note_bounds.Top;
            effect_bounds.Width = info.Dimensions.NoteWidth;
            effect_bounds.Height = note_bounds.Height;
            effect_bounds.Bar = note_bounds.Bar;
        }
    }
}
