using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GuitarTab
{
    public abstract class BaseCanvasView : BaseViewModel, IRecieveDimensionUpdates
    {
        public const int TOP_MARGIN = 20;

        protected IMouseHandler handler;
        protected int scroll_amount;
        protected int current_left;

        public ICommand MouseMovedCommand { get; set; }
        public ICommand MouseUpCommand { get; set; }
        public ICommand MouseDownCommand { get; set; }
        public ICommand HeightCommand { get; set; }
        public ICommand WidthCommand { get; set; }

        public BaseCanvasView()
        {
            initCommands();
        }

        public abstract void handleMousePositionChanged(Point init_position);
        public abstract void handleMouseUp(Point init_position);
        public abstract void handleMouseDown(Point init_position);

        public void initCommands()
        {
            MouseMovedCommand = new MouseRelayCommand(handleMousePositionChanged);
            MouseUpCommand = new MouseRelayCommand(handleMouseUp);
            MouseDownCommand = new MouseRelayCommand(handleMouseDown);
            HeightCommand = new DimensionRelayCommand(handleDimensionChanged);
            WidthCommand = new DimensionRelayCommand(handleDimensionChanged);
        }

        public Point calcFinalPoint(Point init)
        {
            return new Point(init.X - current_left, init.Y + scroll_amount - TOP_MARGIN);
        }

        public void handleDimensionChanged(DimensionChangedEventArgs args)
        {
            if (args.Type == DimensionType.ScreenHeight) { HeightChanged?.Invoke(this, args); }
            else if (args.Type == DimensionType.PageWidth) { WidthChanged?.Invoke(this, args); }
        }

        public void handleDimensionUpdate(int new_val, DimensionType type)
        {
            if (type == DimensionType.ScrollAmount) { scroll_amount = new_val; }
            else if (type == DimensionType.PageWidth) { current_left = new_val; }
        }

        public event EventHandler<DimensionChangedEventArgs> WidthChanged;
        public event EventHandler<DimensionChangedEventArgs> HeightChanged;
    }

    public class EditingCanvasView : BaseCanvasView
    {
        public MouseStateView StateView { get; }
        public MouseHoverView HoverView { get; }
        public MouseSelectedView SelectedView { get; }
        public MouseDragView DragView { get; }

        public EditingCanvasView(MouseStateView state, MouseHoverView hover, MouseSelectedView selected, MouseDragView drag, IMouseHandler h)
            :base()
        {
            StateView = state;
            HoverView = hover;
            SelectedView = selected;
            DragView = drag;
            handler = h;
            scroll_amount = 0;
            current_left = 0;
        }

        public override void handleMousePositionChanged(Point init_position)
        {
            Point position = calcFinalPoint(init_position);
            StateView.changePosition(init_position);
            HoverView.setHoveredObject(handler.hoverCheck(position), scroll_amount - TOP_MARGIN, current_left);
            DragView.updateDragRect(init_position);
        }

        public override void handleMouseUp(Point init_position)
        {
            Point position = calcFinalPoint(init_position);
            DragView.clearDrag();
            handler?.mouseUp(position);
        }

        public override void handleMouseDown(Point init_position)
        {
            Point position = calcFinalPoint(init_position);
            DragView.setDownPoint(init_position, SelectedView.Selected.ToList());
            handler?.mouseDown(position);
        }
    }

    public class ViewingCanvasView : BaseCanvasView
    {
        public ViewingCanvasView(IMouseHandler h)
            :base()
        {
            handler = h;
            scroll_amount = 0;
            current_left = 0;
        }

        public override void handleMousePositionChanged(Point init_position) { }

        public override void handleMouseUp(Point init_position)
        {
            Point position = calcFinalPoint(init_position);
            handler?.mouseUp(position);
        }

        public override void handleMouseDown(Point init_position)
        {
            Point position = calcFinalPoint(init_position);
            handler?.mouseDown(position);
        }
    } 
}
