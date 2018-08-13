using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class WriterInfo
    {
        private int current_id;
        private Dictionary<Note, int> created_notes;
        private List<SimplifiedMultiEffect> created_multi_effects;

        public WriterInfo()
        {
            current_id = 0;
            created_notes = new Dictionary<Note, int>();
            created_multi_effects = new List<SimplifiedMultiEffect>();
        }

        public SimplifiedMultiEffect[] getMultiEffects() { return created_multi_effects.ToArray(); }

        public void readInSimplifiedNote(SimplifiedNote s_note, Note note)
        {
            s_note.NoteId = current_id;
            created_notes.Add(note, current_id);
            current_id++;
        }

        public void createSimplifiedMultiEffect(IMultiEffect effect)
        {
            var s_effect = createEffectBase(effect);
            if (s_effect == null || !canCreateMultiEffect(effect)) { return; }

            s_effect.FirstNoteId = created_notes[effect.First];
            s_effect.SecondNoteId = created_notes[effect.Second];
            created_multi_effects.Add(s_effect);
        }

        private SimplifiedMultiEffect createEffectBase(IMultiEffect effect)
        {
            if (effect is Slide) { return new SimplifiedSlide(effect as Slide); }
            else if (effect is HOPO) { return new SimplifiedHOPO(); }
            else if (effect is Tie) { return new SimplifiedTie(); }
            return null;
        }

        private bool canCreateMultiEffect(IMultiEffect effect)
        {
            return (created_notes.ContainsKey(effect.First) && created_notes.ContainsKey(effect.Second));
        }

        public void resetInfo()
        {
            current_id = 0;
            created_notes.Clear();
            created_multi_effects.Clear();
        }
    }
}
