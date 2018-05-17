using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class Measure : IPosition, IContainModels
    {
        public int Bpm { get; set; }
        public TimeSignature TimeSignature { get; }
        //replace with a method instead of a property
        public bool MatchesPart { get; set; }

        public Position Position { get; set; }

        public SymbolCollection<Chord> ModelCollection { get; }
        public event EventHandler<ObjectAddedArgs> ModelAdded;
        public event EventHandler<ObjectRemovedArgs> ModelRemoved;

        public static Measure createInstance(int? bpm, int? num_beats, NoteLength? beat_type, TimeSignature part_sig, int? part_bpm, int? pos)
        {
            if (bpm == null || num_beats == null || beat_type == null || part_sig == null || part_bpm == null || pos == null) { return null; }

            var this_sig = TimeSignature.createInstance((int)num_beats, (NoteLength)beat_type);
            bool matches_part = this_sig.matchesSignature(part_sig) && bpm == part_bpm;
            var position = new Position((int)pos, false);
            var collection = new ChordCollection(this_sig);

            return new Measure((int)bpm, this_sig, matches_part, position, collection);
        }

        private Measure(int bpm, TimeSignature time_sig, bool matches_part, Position position, ChordCollection collection)
        {
            Bpm = bpm;
            TimeSignature = time_sig;
            MatchesPart = matches_part;

            Position = position;
            ModelCollection = collection;
        }

        public void Add(Chord chord)
        {
            ModelCollection.Add(chord);
            ModelAdded?.Invoke(this, new ObjectAddedArgs(chord, this));
        }

        public void Remove(Chord chord)
        {
            ModelCollection.Remove(chord);
            ModelRemoved?.Invoke(this, new ObjectRemovedArgs(chord));
        }

        public List<object> getGenericModelList()
        {
            return new List<object>(ModelCollection.Items());
        }

        public int getTotalSpace()
        {
            return (ModelCollection as ChordCollection)?.MeasureLength.TotalSpace ?? 0;
        }

        public double getSpaceTaken()
        {
            return (ModelCollection as ChordCollection)?.MeasureLength.SpaceTaken ?? 0;
        }

        public Chord getChordAtPosition(int pos)
        {
            return (ModelCollection.getItemMatchingCondition(x => x.Position.Index == pos));
        }

        public int getLastChordPosition()
        {
            return ModelCollection.Last()?.Position.Index ?? 0;
        }

        public void breakEffectsWithPrevMeasure()
        {
            ModelCollection.First()?.breakEffectsAtPosition(EffectPosition.Into);
        }

        public void breakCrossMeasureEffects()
        {
            ModelCollection.First()?.breakEffectsAtPosition(EffectPosition.Into);
            ModelCollection.Last()?.breakEffectsAtPosition(EffectPosition.After);
        }
    }
}
