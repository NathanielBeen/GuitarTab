using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class ReaderInfo
    {
        private Dictionary<int, Note> created_notes;
        private List<IMultiEffect> created_effects;

        public ReaderInfo()
        {
            created_notes = new Dictionary<int, Note>();
            created_effects = new List<IMultiEffect>();
        }

        public void readInNote(SimplifiedNote s_note, Note note)
        {
            created_notes.Add(s_note.NoteId, note);
        }

        public void createMultiEffects()
        {
            foreach (var effect in created_effects)
            {
                effect.First.Add(effect);
                effect.Second.Add(effect);
            }
        }

        public void readInMultiEffect(SimplifiedMultiEffect effect)
        {
            if (effect == null || !canReadInMultiEffect(effect)) { return; }
            Note first = created_notes[effect.FirstNoteId];
            Note second = created_notes[effect.SecondNoteId];
            
            if (effect is SimplifiedSlide)
            {
                created_effects.Add(Slide.createInstance(first, second, (effect as SimplifiedSlide).Legato));
            }
            else if (effect is SimplifiedHOPO)
            {
                created_effects.Add(HOPO.createInstance(first, second));
            }
            else if (effect is SimplifiedTie)
            {
                created_effects.Add(Tie.createInstance(first, second));
            }
        }

        public bool canReadInMultiEffect(SimplifiedMultiEffect effect)
        {
            return (created_notes.ContainsKey(effect.FirstNoteId) &&
                    created_notes.ContainsKey(effect.SecondNoteId));
        }

        public void resetInfo()
        {
            created_notes.Clear();
            created_effects.Clear();
        }
    }
}
