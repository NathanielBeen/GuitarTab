using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class DimensionUpdater
    {
        protected List<IRecieveDimensionUpdates> recievers;
        protected int prev_value;
        protected DimensionType dim_type;

        public DimensionUpdater(DimensionType type)
        {
            recievers = new List<IRecieveDimensionUpdates>();
            dim_type = type;
        }

        public void addReciever(IRecieveDimensionUpdates reciever)
        {
            if (reciever != null) { recievers.Add(reciever); }
        }

        public void removeReciever(IRecieveDimensionUpdates reciever)
        {
            if (reciever != null) { recievers.Remove(reciever); }
        }

        public void handleRecieverChanged(object sender, ReceiverChangedEventArgs args)
        {
            addReciever(args.NewReciever);
            removeReciever(args.OldReciever);
        }

        public virtual void handleDimensionChanged(object sender, DimensionChangedEventArgs args)
        {
            changeDimension(args.NewValue);
        }

        public void changeDimension(int new_value)
        {
            if (new_value == prev_value) { return; }
            foreach (var rec in recievers) { rec.handleDimensionUpdate(new_value, dim_type); }
            prev_value = new_value;
        }
    }

    public class PageHeightUpdater : DimensionUpdater, IRecieveDimensionUpdates
    {
        private int screen_height;

        public PageHeightUpdater(int init_height) : base(DimensionType.PageHeight) { screen_height = init_height; }

        public override void handleDimensionChanged(object sender, DimensionChangedEventArgs args)
        {
            int new_value = Math.Max(screen_height, args.NewValue);
            changeDimension(new_value);
        }

        public void handleDimensionUpdate(int new_height, DimensionType type)
        {
            if (type != DimensionType.ScreenHeight) { return; }
            if (new_height > screen_height) { changeDimension(Math.Max(new_height, prev_value)); }
            screen_height = new_height;
        }
    }

    public class PageWidthUpdater : DimensionUpdater
    {
        private int render_width;

        public PageWidthUpdater(int render) : base(DimensionType.PageWidth) { render_width = render; }

        public override void handleDimensionChanged(object sender, DimensionChangedEventArgs args)
        {
            int left = Math.Max(0, (args.NewValue - render_width) / 2);
            changeDimension(left);
        }
    }

    public enum DimensionType
    {
        ScreenHeight,
        PageHeight,
        PageWidth,
        ScrollAmount
    }

    public interface IRecieveDimensionUpdates
    {
        void handleDimensionUpdate(int new_val, DimensionType type);
    }

    public class DimensionChangedEventArgs : EventArgs
    {
        public int NewValue { get; }
        public DimensionType Type { get; }

        public DimensionChangedEventArgs(int new_value, DimensionType type) 
            : base()
        {
            NewValue = new_value;
            Type = type;
        }
    }

    public class ReceiverChangedEventArgs : EventArgs
    {
        public IRecieveDimensionUpdates OldReciever { get; }
        public IRecieveDimensionUpdates NewReciever { get; }

        public ReceiverChangedEventArgs(IRecieveDimensionUpdates old_r, IRecieveDimensionUpdates new_r)
            :base()
        {
            OldReciever = old_r;
            NewReciever = new_r;
        }
    }
}
