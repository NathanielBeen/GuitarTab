using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class Part : IContainModels
    {
        public SongInfo SongInfo { get; }
        public InstrumentInfo InstrumentInfo { get; }

        public int DefaultBPM { get; set; }
        public TimeSignature TimeSignature { get; set; }
        public SymbolCollection<Measure> ModelCollection { get; }

        public static Part createInstance(SongInfo song, InstrumentInfo instrument, int? bpm, int? num_beats, NoteLength? beat_type)
        {
            if (song == null || instrument == null || bpm == null || num_beats == null || beat_type == null) { return null; }

            var time_sig = TimeSignature.createInstance((int)num_beats, (NoteLength)beat_type);
            var collection = new PositionedModelCollection<Measure>();
            return new Part(song, instrument, (int)bpm, time_sig, collection);
        }

        private Part(SongInfo song, InstrumentInfo instrument, int bpm, TimeSignature time_sig, PositionedModelCollection<Measure> collection)
        {
            SongInfo = song;
            InstrumentInfo = instrument;

            DefaultBPM = bpm;
            TimeSignature = time_sig;
            ModelCollection = collection;
        }

        public event EventHandler<ObjectAddedArgs> ModelAdded;
        public event EventHandler<ObjectRemovedArgs> ModelRemoved;

        public void Add(Measure measure)
        {
            ModelCollection.Add(measure);
            ModelAdded?.Invoke(this, new ObjectAddedArgs(measure, this));
        }

        public void Remove(Measure measure)
        {
            ModelCollection.Remove(measure);
            ModelRemoved?.Invoke(this, new ObjectRemovedArgs(measure));
        }

        public List<object> getChildrenToBuild()
        {
            return new List<object>(ModelCollection.Items());
        }

        public int getLastMeasurePosition()
        {
            return ModelCollection.Last()?.Position.Index ?? 0;
        }

        public Measure getMeasureAtPosition(int pos)
        {
            return ModelCollection.getItemMatchingCondition(x => x.Position.Index == pos);
        }

        public void updateMeasureMatching()
        {
            Measure prev_measure = null;
            Measure curr_measure = null;
            foreach (Measure measure in ModelCollection.Items())
            {
                prev_measure = curr_measure;
                curr_measure = measure;
                curr_measure?.SetmatchesPrevMeasure(prev_measure);
            }
        }
    }
}
