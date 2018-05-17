using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{

    public class PropertyMenuEventArgs : EventArgs
    {
        public ModelBoundsPair Selected { get; }

        public PropertyMenuEventArgs(ModelBoundsPair select)
            :base()
        {
            Selected = select;
        }
    }

    public class FretMenuEventArgs : EventArgs
    {
        public Point CurrentPosition { get; }
        public ContinueCommandDelegate Command { get; }

        public FretMenuEventArgs(Point position, Action<NodeClick, int> command)
        {
            CurrentPosition = position;
            Command = (c,p) => command(c,p);
        }
    }

    public delegate void ContinueCommandDelegate(NodeClick click, int fret);

    public delegate void CancelDialogueDelegate();
}
