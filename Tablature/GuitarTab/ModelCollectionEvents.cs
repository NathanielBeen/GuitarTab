using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class ObjectRemovedArgs : EventArgs
    {
        public object Removed { get; }

        public ObjectRemovedArgs(object removed)
            :base()
        {
            Removed = removed;
        }
    }

    public class ObjectAddedArgs : EventArgs
    {
        public object Added { get; }
        public object Parent { get; }

        public ObjectAddedArgs(object added, object parent)
        {
            Added = added;
            Parent = parent;
        }
    }

    public class VisualsAddedArgs : EventArgs
    {
        public List<TabDrawingVisual> Added { get; }

        public VisualsAddedArgs(List<TabDrawingVisual> visuals)
        {
            Added = visuals;
        }
    }

    public class VisualsRemovedArgs : EventArgs
    {
        public List<TabDrawingVisual> Removed { get; }

        public VisualsRemovedArgs(List<TabDrawingVisual> visuals)
        {
            Removed = visuals;
        }
    }
}
