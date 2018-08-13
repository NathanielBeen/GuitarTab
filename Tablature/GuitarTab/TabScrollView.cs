using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab
{
    public class TabScrollView
    {
        public ICommand ScrollCommand { get; set; }

        public TabScrollView()
        {
            initCommands();
        }

        public void initCommands()
        {
            ScrollCommand = new DimensionRelayCommand(handleScrolled);
        }

        public void handleScrolled(DimensionChangedEventArgs args)
        {
            if (args.Type == DimensionType.ScrollAmount) { ScrollChanged?.Invoke(this, args); }
        }

        public event EventHandler<DimensionChangedEventArgs> ScrollChanged;
    }
}
