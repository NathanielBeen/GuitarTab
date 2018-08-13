using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GuitarTab
{
    public class PartSettingsMenuView : BaseViewModel
    {
        private GuiObjectTree tree;

        public int Height { get; set; }
        public int Width { get; set; }

        public ICommand OpenPartPropertyMenu { get; private set; }
        public ICommand OpenInstrumentPropertyMenu { get; private set; }

        public event EventHandler<PartMenuEventArgs> PartMenuLaunched;

        public PartSettingsMenuView(GuiObjectTree t)
        {
            tree = t;
            initCommands();
        }

        public void initCommands()
        {
            OpenPartPropertyMenu = new RelayCommand(handlePartPropertyMenu);
            OpenInstrumentPropertyMenu = new RelayCommand(handleInstrumentPropertyMenu);
        }

        public void handlePartPropertyMenu()
        {
            var click = new NodeClick(new Point(0, 0));
            click.PartNode = tree.Root as PartTreeNode;
            var args = new PartMenuEventArgs(click, false, Width, Height);

            PartMenuLaunched?.Invoke(this, args);
        }

        public void handleInstrumentPropertyMenu()
        {
            var click = new NodeClick(new Point(0, 0));
            click.PartNode = tree.Root as PartTreeNode;
            var args = new PartMenuEventArgs(click, true, Width, Height);

            PartMenuLaunched?.Invoke(this, args);
        }
    }
}
