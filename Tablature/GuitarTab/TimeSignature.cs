using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class TimeSignature
    {
        public int NumberOfBeats { get; private set; }
        public NoteLength BeatType { get; private set; }
        public int MeasureLength { get { return NumberOfBeats * (int)BeatType; } }

        public static TimeSignature createInstance(int? num_beats, NoteLength? beat_type)
        {
            if (num_beats == null || beat_type == null) { return null; }
            if (!canSetTimeSignature((int)num_beats, (NoteLength)beat_type)) { return null; }
            return new TimeSignature((int)num_beats, (NoteLength)beat_type);
        }

        private TimeSignature(int num_beats, NoteLength beat_type)
        {
            NumberOfBeats = num_beats;
            BeatType = beat_type;
        }

        public static bool canSetTimeSignature(int num_beats, NoteLength beat_type)
        {
            return (num_beats >= 0 && num_beats <= 32 && beat_type != NoteLength.None && !beat_type.isDotted());
        }

        public void setTimeSignature(int num_beats, NoteLength beat_type)
        {
            if (canSetTimeSignature(num_beats, beat_type))
            {
                NumberOfBeats = num_beats;
                BeatType = beat_type;
            }
        }

        public bool matchesSignature(TimeSignature other)
        {
            if (other is null) { return false; }
            return (other.NumberOfBeats == NumberOfBeats && other.BeatType == BeatType);
        }
    }
}
