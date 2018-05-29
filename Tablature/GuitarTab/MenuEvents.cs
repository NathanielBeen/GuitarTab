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
        public NodeClick Click { get; }

        public PropertyMenuEventArgs(NodeClick click)
            :base()
        {
            Click = click;
        }
    }

    public class FretMenuEventArgs : EventArgs
    {
        public NodeClick Click { get; }
        public ContinueCommandDelegate Command { get; }

        public FretMenuEventArgs(NodeClick click, Action<NodeClick, int> command)
        {
            Click = click;
            Command = (c,p) => command(c,p);
        }
    }

    public class NoteSelectLaunchEventArgs : EventArgs
    {
        public NodeClick Click { get; }
        public ContinueNoteSelectDelegate Command { get; }

        public NoteSelectLaunchEventArgs(NodeClick click, Action<NodeClick> command)
        {
            Click = click;
            Command = (c) => command(c);
        }
    }

    public class NoteSelectEndEventArgs : EventArgs
    {
        public NodeClick Click { get; }

        public NoteSelectEndEventArgs(NodeClick click)
        {
            Click = click;
        }
    }

    public delegate void ContinueCommandDelegate(NodeClick click, int fret);

    public delegate void ContinueNoteSelectDelegate(NodeClick click);

    public delegate void CancelDialogueDelegate();
}
