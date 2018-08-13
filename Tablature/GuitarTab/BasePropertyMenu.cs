using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public abstract class BasePropertyMenu : BaseInputViewModel
    {
        protected NodeClick ref_click;
        protected GuiCommandExecutor executor;

        public BasePropertyMenu(NodeClick click, GuiCommandExecutor exec)
        {
            ref_click = click;
            executor = exec;
        }

        public abstract void resetToDefault();
        public abstract void submitChanges();

        public NodeClick getClickCopy()
        {
            NodeClick new_click = new NodeClick(ref_click.Point);
            new_click.PartNode = ref_click.PartNode;
            new_click.MeasureNodes.AddRange(ref_click.MeasureNodes);
            new_click.ChordNodes.AddRange(ref_click.ChordNodes);
            new_click.NoteNodes.AddRange(ref_click.NoteNodes);
            new_click.EffectNode = ref_click.EffectNode;
            new_click.Selected = ref_click.Selected;

            return new_click;
        }
    }
}
