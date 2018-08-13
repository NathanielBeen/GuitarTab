using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    //make IContainModels only about the collection, add IAddModels and IRemoveModels as well>
    public class Note : IPosition, IContainModels<IEffect>, IComparable<Note>
    {
        public int Fret { get; set; }
        public int String { get; set; }

        public Position Position { get; set; }
        public Length Length { get; set; }

        public SymbolCollection<IEffect> ModelCollection { get; }
        public event EventHandler<ObjectAddedArgs> ModelAdded;
        public event EventHandler<ObjectRemovedArgs> ModelRemoved;

        public static Note createInstance(int? fret, int? this_string, MultiPosition position, Length len)
        {
            if (fret == null || this_string == null || position == null || len == null) { return null; }

            var collection = new ModelCollection<IEffect>();
            return new Note((int)fret, (int)this_string, position, len, collection);
        }

        private Note(int fret, int this_string, MultiPosition position, Length len, ModelCollection<IEffect> collection)
        {
            Fret = fret;
            String = this_string;

            Position = position;
            Length = len;

            ModelCollection = collection;
        }

        public void Add(IEffect effect)
        {
            if (effect == null) { return; }
            IEffect prev = getEffectAtPosition(effect.getPosition(this));
            if (prev != null) { Remove(prev); }

            ModelCollection.Add(effect);
            if (effect is IMultiEffect && effect.getPosition(this) == EffectPosition.Into) { return; }
            ModelAdded?.Invoke(this, new ObjectAddedArgs(effect, this));
        }

        public void Remove(IEffect effect)
        {
            if (effect == null) { return; }
            bool remove = !(effect is IMultiEffect && effect.getPosition(this) == EffectPosition.Into);
            effect.breakEffect(this);

            ModelCollection.Remove(effect);
            if (remove) { ModelRemoved?.Invoke(this, new ObjectRemovedArgs(effect)); }
        }

        public List<object> getChildrenToBuild()
        {
            return new List<object>(ModelCollection.getItemsMatchingCondition(e => !(e is IMultiEffect && e.getPosition(this) == EffectPosition.Into)));
        }

        public MultiPosition getPosition() { return Position as MultiPosition; }

        public IEffect getEffectAtPosition(EffectPosition position) { return ModelCollection.getItemMatchingCondition(e => e.getPosition(this) == position); }

        public void removeEffectAtPosition(EffectPosition position)
        {
            IEffect effect = getEffectAtPosition(position);
            Remove(effect);
        }

        public List<IEffect> getMultiEffects() { return ModelCollection.getItemsMatchingCondition(e => e is IMultiEffect); }

        public void removeMultiEffects()
        {
            List<IEffect> effects = getMultiEffects();
            foreach (IEffect effect in effects) { Remove(effect); }
        }

        public void removeMultiEffectsAtPosition(EffectPosition position)
        {
            IEffect effect = ModelCollection.getItemMatchingCondition(e => e is IMultiEffect && e.getPosition(this) == position);
            Remove(effect);
        }

        public int CompareTo(Note other)
        {
            return String - other.String;
        }
    }
}
