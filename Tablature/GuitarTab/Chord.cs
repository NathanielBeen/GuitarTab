using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class Chord : IPosition
    {
        public Position Position { get; set; }

        public Length Length { get; private set; }

        public static Chord createInstance(int? pos, Position measure_position, Length length)
        {
            if (pos == null || measure_position == null || length == null) { return null; }

            var position = new MultiPosition(measure_position, (int)pos, false);
            return new Chord(position, length);
        }

        protected Chord(MultiPosition position, Length length)
        {
            Position = position;
            Length = length;
        }

        public virtual void setLength(Length new_length) { Length = new_length;  }
        public virtual void breakEffectsAtPosition(EffectPosition position) { }
        public virtual void breakMultiEffects() { }
        public virtual void breakMultiEffectsAtPosition(EffectPosition position) { }
    }

    public class NoteChord : Chord, IContainModels
    {
        public ModelCollection<Note> ModelCollection { get; }
        public event EventHandler<ObjectAddedArgs> ModelAdded;
        public event EventHandler<ObjectRemovedArgs> ModelRemoved;

        new public static NoteChord createInstance(int? pos, Position measure_position, Length length)
        {
            if (pos == null || measure_position == null || length == null) { return null; }

            var position = new MultiPosition(measure_position, (int)pos, false);
            return new NoteChord(position, length, new ModelCollection<Note>());
        }

        private NoteChord(MultiPosition position, Length length, ModelCollection<Note> collection)
            : base(position, length)
        {
            ModelCollection = collection;
        }

        public override void setLength(Length new_length)
        {
            base.setLength(new_length);
            foreach (Note note in ModelCollection.Items())
            {
                note.Length = new_length;
            }
        }

        public override void breakEffectsAtPosition(EffectPosition position)
        {
            ModelCollection.performActionOnAllItems(n => n.removeEffectAtPosition(position));
        }

        public override void breakMultiEffects()
        {
           ModelCollection.performActionOnAllItems(n => n.removeMultiEffects());
        }

        public override void breakMultiEffectsAtPosition(EffectPosition position)
        {
            ModelCollection.performActionOnAllItems(n => n.removeMultiEffectsAtPosition(position));
        }

        public void Add(Note note)
        {
            ModelCollection.Add(note);
            ModelAdded?.Invoke(this, new ObjectAddedArgs(note, this));
        }

        public void Remove(Note note)
        {
            ModelCollection.Remove(note);
            ModelRemoved?.Invoke(this, new ObjectRemovedArgs(note));
        }

        public List<object> getChildrenToBuild()
        {
            return new List<object>(ModelCollection.Items());
        }
    }
}
