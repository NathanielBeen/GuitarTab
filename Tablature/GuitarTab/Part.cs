using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class Part : IContainModels
    {
        //some kind of instrumenttype thing
        public int DefaultBPM { get; set; }
        public TimeSignature TimeSignature { get; set; }
        public ModelCollection<Measure> ModelCollection { get; }

        public static Part createInstance(int? bpm, int? num_beats, NoteLength? beat_type)
        {
            if (bpm == null || num_beats == null || beat_type == null) { return null; }

            var time_sig = TimeSignature.createInstance((int)num_beats, (NoteLength)beat_type);
            var collection = new PositionedModelCollection<Measure>();
            return new Part((int)bpm, time_sig, collection);
        }

        private Part(int bpm, TimeSignature time_sig, PositionedModelCollection<Measure> collection)
        {
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

        public List<object> getGenericModelList()
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

        public Note getPreviousNote(Note note)
        {
            MultiPosition note_pos = note.getPosition();
            if (note_pos == null) { return null; }

            Measure measure = getMeasureAtPosition(note_pos.getPreviousMeasurePosition());
            if (measure == null) { return null; }

            int prev_pos = note_pos.getPreviousPosition(measure.ModelCollection.Count());
            NoteChord chord = measure.getChordAtPosition(prev_pos) as NoteChord;
            if (chord == null) { return null; }

            return chord.ModelCollection.getItemMatchingCondition(n => n.String == note.String);
        }

        public Note getNextNote(Note note)
        {
            MultiPosition note_pos = note.getPosition();
            if (note_pos == null) { return null; }

            Measure measure = getMeasureAtPosition(note_pos.getNextMeasurePosition());
            if (measure == null) { return null; }

            NoteChord chord = measure.getChordAtPosition(note_pos.getNextPosition()) as NoteChord;
            if (chord == null) { return null; }

            return chord.ModelCollection.getItemMatchingCondition(n => n.String == note.String);
        }
    }
}
