using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GuitarTab
{
    class TabScrollViewer : ScrollViewer
    {
        public static readonly DependencyProperty ScrollCommandProperty =
            DependencyProperty.Register("ScrollCommand", typeof(DimensionRelayCommand),
                typeof(TabScrollViewer));

        public DimensionRelayCommand ScrollCommand
        {
            get { return (DimensionRelayCommand)GetValue(ScrollCommandProperty); }
            set { SetValue(ScrollCommandProperty, value); }
        }

        public TabScrollViewer() : base()
        {
            ScrollChanged += handleScroll;
        }

        public void handleScroll(object sender, ScrollChangedEventArgs args)
        {
            if (args.VerticalChange != 0) { ScrollCommand?.Execute(new DimensionChangedEventArgs((int)args.VerticalOffset, DimensionType.ScrollAmount)); }
        }
    }
}
