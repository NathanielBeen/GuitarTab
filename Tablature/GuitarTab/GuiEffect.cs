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
    public class EffectMouseHandler : BaseMouseHandler
    {
        public EffectMouseHandler(VisualBounds b, CommandSelections c, MouseSelections m, GuiCommandExecutor g, IDelegate del)
            :base(b, c, m, g, del) { }

        public override void mouseClick() { }
        public override void mouseDragRelease() { }
        public override void mouseDragSelect() { }
        public override void addToCommandSelections() { }
        public override void addToMouseSelections() { }
        public override void mousePositionCheck() { }
    }

    public class PalmMuteBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private PalmMute palm_mute;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public PalmMuteBounds(PalmMute pm, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            palm_mute = pm;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createAboveLineEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateAboveLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class PalmMuteDrawingVisual : TabDrawingVisual
    {
        private PalmMute palm_mute;

        public PalmMuteDrawingVisual(PalmMute pm, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            palm_mute = pm;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var text = new FormattedText("P.H.", CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.FontSize, info.DrawingObjects.Brush);
            dc.DrawText(text, new Point(0, 0));
        }
    }

    public class BendBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private Bend bend;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public BendBounds(Bend b, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            bend = b;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createInLineEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateInLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class BendDrawingVisual : TabDrawingVisual
    {
        private Bend bend;

        public BendDrawingVisual(Bend b, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            bend = b;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            Rect bend_rect = new Rect(0, 0, Bounds.Width / 2, Bounds.Height);
            dc.DrawArc(info.DrawingObjects.Pen, info.DrawingObjects.Brush, bend_rect, 0, 90);

            if (bend.BendReturns)
            {
                dc.DrawLine(info.DrawingObjects.Pen, new Point(Bounds.Width / 2, 0),
                            new Point(Bounds.Width / 2, Bounds.Height));
            }

            var text = new FormattedText(bend.Amount.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.FontSize, info.DrawingObjects.Brush);
            dc.DrawText(text, new Point(Bounds.Width / 2, 0));
        }
    }

    public class PinchHarmonicBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private PinchHarmonic pinch_harmonic;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public PinchHarmonicBounds(PinchHarmonic ph, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            pinch_harmonic = ph;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createAboveLineEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateAboveLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class PinchHarmonicDrawingVisual : TabDrawingVisual
    {
        private PinchHarmonic pinch_harmonic;

        public PinchHarmonicDrawingVisual(PinchHarmonic ph, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            pinch_harmonic = ph;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var image = new BitmapImage(info.Images.getEffectImagePath(TabImages.PINCH_HARMONIC));
            var dest_rect = new Rect(Bounds.Width / 2 - image.Width / 2, Bounds.Height / 2 - image.Height / 2, image.Width, image.Height);
            dc.DrawImage(image, dest_rect);
        }
    }

    public class VibratoBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private Vibrato vibrato;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public VibratoBounds(Vibrato v, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            vibrato = v;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createBeforeNoteEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateBeforeNoteEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class VibratoDrawingVisual : TabDrawingVisual
    {
        private Vibrato vibrato;

        public VibratoDrawingVisual(Vibrato v, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            vibrato = v;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var image_code = vibrato.Wide ? TabImages.WIDE_VIBRATO : TabImages.VIBRATO;
            var image = new BitmapImage(info.Images.getEffectImagePath(image_code));
            var image_rect = new Int32Rect(0, 0, Bounds.Width, Bounds.Height);
            var cropped_image = new CroppedBitmap(image, image_rect);

            dc.DrawImage(cropped_image, new Rect(image_rect.X, image_rect.Y, image_rect.Width, image_rect.Height));
        }
    }

    public class SlideBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private Slide slide;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public SlideBounds(Slide s, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            slide = s;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createInLineEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateInLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class SlideDrawingVisual : TabDrawingVisual
    {
        private Slide slide;

        public SlideDrawingVisual(Slide s, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            slide = s;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
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

    public class HOPOBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private HOPO hopo;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public HOPOBounds(HOPO h, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            hopo = h;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createAboveNoteEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateAboveNoteEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class HOPODrawingVisual : TabDrawingVisual
    {
        private HOPO hopo;

        public HOPODrawingVisual(HOPO h, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            hopo = h;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
            dc.DrawArc(info.DrawingObjects.Pen, info.DrawingObjects.Brush, rect, 45, -90);
        }
    }

    public class TieBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private Tie tie;
        private Note note;
        private VisualBounds note_bounds;
        private VisualInfo info;

        public TieBounds(Tie t, Note n, VisualBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            tie = t;
            note = n;
            note_bounds = n_bounds;
            info = v_info;

            Bounds = genBounds();
        }

        public VisualBounds genBounds() { return GuiEffectBoundDrawing.createInLineEffectBounds(note_bounds, note, info); }

        public void updateBounds() { GuiEffectBoundDrawing.updateInLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class TieDrawingVisual : TabDrawingVisual
    {
        private Tie tie;

        public TieDrawingVisual(Tie t, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            tie = t;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var rect = new Rect(0, 0, Bounds.Width, Bounds.Height);
            dc.DrawArc(info.DrawingObjects.Pen, info.DrawingObjects.Brush, rect, -45, 90);
        }
    }

    public static class GuiEffectBoundDrawing
    {

        public static VisualBounds createAboveNoteEffectBounds(VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int width = info.Dimensions.getLength(note.Length.NoteType) + info.Dimensions.NoteWidth;
            int height = info.Dimensions.StringHeight;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public static void updateAboveNoteEffectBounds(VisualBounds effect_bounds, VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int right = note_bounds.Left + info.Dimensions.getLength(note.Length.NoteType) + info.Dimensions.NoteWidth;
            right = info.Position.truncateHorizontalLengthIfNeeded(right);

            effect_bounds.Left = note_bounds.Left;
            effect_bounds.Top = note_bounds.Top - info.Dimensions.StringHeight;
            effect_bounds.Width = right - note_bounds.Left;
            effect_bounds.Height = info.Dimensions.StringHeight;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static VisualBounds createAboveLineEffectBounds(VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int width = note_bounds.Width;
            int height = info.Dimensions.EffectHeight - info.Dimensions.EffectMargin * 2;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public static void updateAboveLineEffectBounds(VisualBounds effect_bounds, VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int right = note_bounds.Left + note_bounds.Width;
            right = info.Position.truncateHorizontalLengthIfNeeded(right);

            effect_bounds.Left = note_bounds.Left;
            effect_bounds.Top = info.Dimensions.PageHeadHeight + info.Dimensions.LineHeight * note_bounds.Bar + info.Dimensions.EffectMargin;
            effect_bounds.Width = right - note_bounds.Left;
            effect_bounds.Height = info.Dimensions.EffectHeight - info.Dimensions.EffectMargin * 2;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static VisualBounds createInLineEffectBounds(VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int width = note_bounds.Left + note_bounds.Height + info.Dimensions.getLength(note.Length.NoteType);
            int height = note_bounds.Height;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public static void updateInLineEffectBounds(VisualBounds effect_bounds, VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int right = Math.Min(note_bounds.Left + note_bounds.Width + info.Dimensions.getLength(note.Length.NoteType),
                info.Dimensions.BarMargin + info.Dimensions.BarWidth);

            effect_bounds.Left = note_bounds.Left + note_bounds.Height;
            effect_bounds.Top = note_bounds.Top;
            effect_bounds.Width = right - (note_bounds.Left + note_bounds.Height);
            effect_bounds.Height = note_bounds.Height;
            effect_bounds.Bar = note_bounds.Bar;
        }

        public static VisualBounds createBeforeNoteEffectBounds(VisualBounds note_bounds, Note note, VisualInfo info)
        {
            int width = info.Dimensions.NoteWidth;
            int height = note_bounds.Height;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public static void updateBeforeNoteEffectBounds(VisualBounds effect_bounds, VisualBounds note_bounds, Note note, VisualInfo info)
        {
            effect_bounds.Left = note_bounds.Left - info.Dimensions.NoteWidth;
            effect_bounds.Top = note_bounds.Top;
            effect_bounds.Width = info.Dimensions.NoteWidth;
            effect_bounds.Height = note_bounds.Height;
            effect_bounds.Bar = note_bounds.Bar;
        }
    }
}
