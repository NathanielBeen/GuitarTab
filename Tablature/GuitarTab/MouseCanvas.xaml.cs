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
        public MouseCanvasView View { get; set; }

        public MouseCanvas()
        {
            InitializeComponent();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);
            View?.handleMousePositionChanged(position);
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            View?.handleMouseDown(e.GetPosition(this));
        }

        private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            View?.handleMouseUp(e.GetPosition(this));
        }
    }
}
