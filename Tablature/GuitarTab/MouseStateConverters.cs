using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class MouseStateConverter : BaseViewModel
    {
        //mouseseelctions wraps
        private Selection selection_state;
        public Selection SelectionState
        {
            get { return selection_state; }
            set
            {
                if (selection_state != value)
                {
                    SetProperty(ref selection_state, value);
                    GuiSelectionState = selection_state.convertToAddItem();
                }
            }
        }

        //commandselections wraps
        private EffectType effect_type;
        public EffectType SelectedEffectType
        {
            get { return effect_type; }
            set
            {
                SetProperty(ref effect_type, value);
            }
        }

        //AddItemControl directly references
        private AddItem gui_selection_state;
        public AddItem GuiSelectionState
        {
            get { return gui_selection_state; }
            set
            {
                if (gui_selection_state != value)
                {
                    SetProperty(ref gui_selection_state, value);
                    SelectionState = gui_selection_state.convertToSelection();
                    SelectedEffectType = gui_selection_state.convertToEffectType();
                }
            }
        }
    }
}
