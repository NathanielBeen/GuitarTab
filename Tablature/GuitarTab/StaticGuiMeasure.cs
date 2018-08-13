using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class MeasureBounds : IBoundedStrategy
    {
        public const int MIN_SPACE = 200;
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        protected VisualInfo info;

        protected Measure measure;

        public MeasureBounds(Measure m, VisualInfo v_info, IDelegate del)
        {
            BoundsDelegate = del;
            measure = m;
            info = v_info;
        }

        public void updateBounds()
        {
            getInitBoundValues(out int first_left, out int lowest_bar);
            BoundsDelegate?.invokeDelegate();
            getEndingBoundValues(lowest_bar, first_left, out int highest_bar, out int last_right);

            var bounds_list = new List<IBounds>();
            for (int i = lowest_bar; i <= highest_bar; i++) { bounds_list.Add(genSingleMeasureBounds(first_left, last_right, lowest_bar, highest_bar, i)); }

            updateMeasureBounds(bounds_list);
            if (measure.Position.IsLast) { info.Position.heightChanged(Bounds.Top + Bounds.Height + 20); }
        }

        public void getInitBoundValues(out int first_left, out int lowest_bar)
        {
            if (info.Position.X >= info.Dimensions.BarWidth + info.Dimensions.BarMargin + info.Position.CurrentLeft)
            {
                info.Position.jumpToNextBar();
            }

            first_left = info.Position.X;
            lowest_bar = info.Position.CurrentBar;
            if (!measure.MatchesPrevMeasure)
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

        public IBounds genSingleMeasureBounds(int first_left, int last_right, int first_bar, int last_bar, int current_bar)
        {
            int left = (current_bar == first_bar) ? first_left : info.Dimensions.BarMargin + info.Position.CurrentLeft;
            int width = (current_bar == last_bar) ? last_right - left : info.Dimensions.BarWidth + info.Dimensions.BarMargin + info.Position.CurrentLeft - left;
            int top = info.Dimensions.PageHeadHeight + current_bar * info.Dimensions.LineHeight + info.Dimensions.EffectHeight;
            int height = info.Dimensions.BarHeight;

            return new SingleBounds(left, top, width, height, current_bar);
        }

        public void updateMeasureBounds(List<IBounds> bounds)
        {
            Bounds.setBoundsList(bounds);

            Bounds.Left = bounds.FirstOrDefault()?.Left ?? 0;
            Bounds.Top = bounds.FirstOrDefault()?.Top ?? 0;
            Bounds.Width = bounds.LastOrDefault()?.Width ?? 0;
            Bounds.Height = bounds.LastOrDefault()?.Top + bounds.LastOrDefault()?.Height - Bounds.Top ?? 0;
            Bounds.Bar = bounds.LastOrDefault()?.Bar ?? 0;
        }
    }

    public class StaticMeasureMouseHandler : StaticMouseHandler
    {
        public StaticMeasureMouseHandler(IBounds b, IMouseDelegate del) :base(del, b) { }

        public override void handleMouseEvent(MouseClick click)
        {
            invokeClickDelegate(click);
        }
    }
}
