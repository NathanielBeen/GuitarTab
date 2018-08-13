using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GuitarTab
{
    /// <summary>
    /// Interaction logic for MouseCanvas.xaml
    /// </summary>
    public partial class MouseCanvas : UserControl
    {
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.Register("MouseMoveCommand", typeof(MouseRelayCommand),
                typeof(MouseCanvas));

        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.Register("MouseUpCommand", typeof(MouseRelayCommand),
                typeof(MouseCanvas));

        public static readonly DependencyProperty MouseDownCommandProperty =
            DependencyProperty.Register("MouseDownCommand", typeof(MouseRelayCommand),
                typeof(MouseCanvas));

        public static readonly DependencyProperty HeightCommandProperty =
            DependencyProperty.Register("HeightCommand", typeof(DimensionRelayCommand),
                typeof(MouseCanvas), new PropertyMetadata(default(DimensionRelayCommand), onHeightAdded));

        public static readonly DependencyProperty WidthCommandProperty =
            DependencyProperty.Register("WidthCommand", typeof(DimensionRelayCommand),
                typeof(MouseCanvas), new PropertyMetadata(default(DimensionRelayCommand), onWidthAdded));

        public MouseRelayCommand MouseMoveCommand
        {
            get { return (MouseRelayCommand)GetValue(MouseMoveCommandProperty); }
            set { SetValue(MouseMoveCommandProperty, value);  }
        }

        public MouseRelayCommand MouseUpCommand
        {
            get { return (MouseRelayCommand)GetValue(MouseUpCommandProperty); }
            set { SetValue(MouseUpCommandProperty, value); }
        }

        public MouseRelayCommand MouseDownCommand
        {
            get { return (MouseRelayCommand)GetValue(MouseDownCommandProperty); }
            set { SetValue(MouseDownCommandProperty, value); }
        }

        public DimensionRelayCommand HeightCommand
        {
            get { return (DimensionRelayCommand)GetValue(HeightCommandProperty); }
            set { SetValue(HeightCommandProperty, value); }
        }

        private int height;
        private int width;

        public DimensionRelayCommand WidthCommand
        {
            get { return (DimensionRelayCommand)GetValue(WidthCommandProperty); }
            set { SetValue(WidthCommandProperty, value); }
        }

        public MouseCanvas()
        {
            InitializeComponent();
            SizeChanged += handleSizeChanged;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            MouseMoveCommand?.Execute(e.GetPosition(this));
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownCommand?.Execute(e.GetPosition(this));
        }

        private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseUpCommand?.Execute(e.GetPosition(this));
        }

        public void handleSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.HeightChanged)
            {
                HeightCommand?.Execute(new DimensionChangedEventArgs((int)args.NewSize.Height, DimensionType.ScreenHeight));
                height = (int)args.NewSize.Height;
            }
            if (args.WidthChanged)
            {
                WidthCommand?.Execute(new DimensionChangedEventArgs((int)args.NewSize.Width, DimensionType.PageWidth));
                width = (int)args.NewSize.Width;
            }
        }

        private static void onHeightAdded(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MouseCanvas canvas = obj as MouseCanvas;
            canvas.onHeightAdded(args);
        }

        private void onHeightAdded(DependencyPropertyChangedEventArgs args)
        {
            HeightCommand?.Execute(new DimensionChangedEventArgs(height, DimensionType.ScreenHeight));
        }

        private static void onWidthAdded(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MouseCanvas canvas = obj as MouseCanvas;
            canvas.onWidthAdded(args);
        }

        private void onWidthAdded(DependencyPropertyChangedEventArgs args)
        {
            WidthCommand?.Execute(new DimensionChangedEventArgs(width, DimensionType.PageWidth));
        }
    }
}
