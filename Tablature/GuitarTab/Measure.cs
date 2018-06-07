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
        public bool MatchesPrevMeasure { get; set; }
        public bool MatchUpdated { get; set; }

        public Position Position { get; set; }

        public SymbolCollection<Chord> ModelCollection { get; }
        public event EventHandler<ObjectAddedArgs> ModelAdded;
        public event EventHandler<ObjectRemovedArgs> ModelRemoved;

        public static Measure createInstance(int? bpm, int? num_beats, NoteLength? beat_type, int? pos)
        {
            if (bpm == null || num_beats == null || beat_type == null || pos == null) { return null; }

            var this_sig = TimeSignature.createInstance((int)num_beats, (NoteLength)beat_type);
            bool matches_prev_measure = false;
            var position = new Position((int)pos, false);
            var collection = new ChordCollection(this_sig);

            return new Measure((int)bpm, this_sig, matches_prev_measure, position, collection);
        }

        private Measure(int bpm, TimeSignature time_sig, bool matches_prev_measure, Position position, ChordCollection collection)
        {
            Bpm = bpm;
            TimeSignature = time_sig;
            MatchesPrevMeasure = matches_prev_measure;
            MatchUpdated = false;

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

        public int getSpaceTaken()
        {
            return (ModelCollection as ChordCollection)?.MeasureLength.SpaceTaken ?? 0;
        }

        public void updateSpaceTaken()
        {
            (ModelCollection as ChordCollection)?.MeasureLength.updateSpaceTaken(ModelCollection.Items());
        }

        public Chord getChordAtPosition(int pos)
        {
            return (ModelCollection.getItemMatchingCondition(x => x.Position.Index == pos));
        }

        public int getLastChordPosition()
        {
            return ModelCollection.Last()?.Position.Index ?? 0;
        }

        public void breakCrossMeasureEffectsAtPosition(EffectPosition position)
        {
            if (position == EffectPosition.Into) { ModelCollection.First()?.breakMultiEffectsAtPosition(position); }
            else { ModelCollection.Last()?.breakMultiEffectsAtPosition(position); }
        }

        public void breakCrossMeasureEffects()
        {
            ModelCollection.First()?.breakMultiEffectsAtPosition(EffectPosition.Into);
            ModelCollection.Last()?.breakMultiEffectsAtPosition(EffectPosition.After);
        }

        public void SetmatchesPrevMeasure(Measure other)
        {
            bool new_matches = (other != null && other.Bpm == Bpm && other.TimeSignature.matchesSignature(TimeSignature));
            if (new_matches != MatchesPrevMeasure)
            {
                MatchesPrevMeasure = new_matches;
                MatchUpdated = true;
            }
        }
    }
}
