using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    //create an interface called IContainChildren or something that 
    //part, measure, chord, note will inherit.
    //will give access to the itemadded/itemremoved events, add, remove, ect methods.

    //make a more general symbolcollection base class that the sortedsymbolcollection
    //and the effectcollection can then inherit from.
    //it will include:
    // a list of items (with get and clear methods)
    // raiseobjectadded and raiseobjectremoved methods (effectcollection needs to have an add and remove
    // method that takes an integer position as well)
    // perform action and items matching condition

    //make note effects a type of symbolCollection

    //what is the difference between this list and the sorted version? (spoiler: there is none) Remove the redundancy.

    public interface IContainModels
    {
        List<object> getGenericModelList();
        event EventHandler<ObjectAddedArgs> ModelAdded;
        event EventHandler<ObjectRemovedArgs> ModelRemoved;
    }

    public interface IContainModels<T> : IContainModels
    {
        ModelCollection<T> ModelCollection { get; }

        void Add(T item);
        void Remove(T item);
    }

    public class ModelCollection<T>
    {
        protected SortedSet<T> items;

        public event EventHandler<ObjectAddedArgs> ModelAdded;
        public event EventHandler<ObjectRemovedArgs> ModelRemoved;

        /*
        * Constructor
        */
        public ModelCollection()
        {
            items = createItemList();
        }

        public virtual SortedSet<T> createItemList()
        {
            return new SortedSet<T>(Comparer<T>.Default);
        }

        /*
        * Add/Remove Methods
        */

        public virtual void Add(T item) { items.Add(item); }

        public virtual void Remove(T item) { items.Remove(item); }

        public virtual void Clear() { items = createItemList(); }

        public bool Contains(T item) { return (items.Contains(item)); }

        public T First() { return items.FirstOrDefault(); }

        public T Last() { return items.LastOrDefault(); }

        public List<T> Items() { return items.ToList(); }

        public int Count() { return items.Count; }

        /*
        * Search Methods
        */
        public T getItemMatchingCondition(Func<T, bool> condition)
        {
            return (from item in items
                    where condition(item)
                    select item).FirstOrDefault();
        }

        public List<T> getItemsMatchingCondition(Func<T, bool> condition)
        {
            return (from item in items
                    where condition(item)
                    select item).ToList();
        }

        public void performActionOnAllItems(Action<T> operation)
        {
            foreach (T item in items) { operation(item); }
        }

        public void performActionOnSpecificItems(Func<T, bool> condition, Action<T> operation)
        {
            foreach (T item in items)
            {
                if (condition(item)) { operation(item); }
            }
        }
    }

    public class PositionedModelCollection<T> : ModelCollection<T>
        where T : IPosition
    {
        private T last;

        /*
        * Constructor
        */
        public PositionedModelCollection()
            :base()
        {
            last = default(T);
        }

        public override SortedSet<T> createItemList()
        {
            return new SortedSet<T>(new PositionComparer<T>());
        }

        /*
        * Update Methods
        */
        public void updateLastItem()
        {
            T new_last = items.Last();
            if (last == null)
            {
                last = new_last;
            }

            else if (!last.Equals(new_last))
            {
                last.Position.IsLast = false;
                new_last.Position.IsLast = true;
                last = new_last;
            }
        }

        /*
        * Add/Remove Methods
        */

        public override void Add(T item)
        {
            base.Add(item);
            updateLastItem();
        }

        public override void Remove(T item)
        {
            base.Remove(item);
            updateLastItem();
        }

        public override void Clear()
        {
            base.Clear();
            last = default(T);
        }
    }

    public class ChordCollection : PositionedModelCollection<Chord>
    {
        public MeasureLength MeasureLength;

        public ChordCollection(TimeSignature time_sig) : base()
        {
            MeasureLength = new MeasureLength(time_sig);
        }

        public override void Add(Chord item)
        {
            base.Add(item);
            MeasureLength.updateSpaceTaken(items);
        }

        public override void Remove(Chord item)
        {
            base.Remove(item);
            MeasureLength.updateSpaceTaken(items);
        }

        public override void Clear()
        {
            base.Clear();
            MeasureLength.updateSpaceTaken(items);
        }
    }
}
