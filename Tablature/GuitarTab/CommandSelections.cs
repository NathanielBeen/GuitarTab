using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class CommandSelections : BaseViewModel
    {
        private MouseStateConverter converter;

        private Length selected_length;
        public Length SelectedLength
        {
            get { return selected_length; }
            set { SetProperty(ref selected_length, value); }
        }

        public EffectType SelectedEffectType
        {
            get { return converter.SelectedEffectType; }
            set { converter.SelectedEffectType = value; }
        }

        public Part SelectedPart { get; set; }
        public List<Measure> SelectedMeasure { get; private set; }
        public List<Chord> SelectedChord { get; private set; }
        public List<Note> SelectedNote { get; private set; }
        public IEffect SelectedEffect { get; set; }

        public int? Position { get; set; }

        public int? NumBeats { get; set; }
        public NoteLength? BeatType { get; set; }
        public int? BPM { get; set; }

        public int? String { get; set; }
        public int? Fret { get; set; }

        public double? BendAmount { get; set; }

        public bool? Returns { get; set; }
        public bool? Wide { get; set; }
        public bool? Legato { get; set; }

        public CommandSelections(MouseStateConverter conv)
        {
            converter = conv;
            SelectedLength = null;
            SelectedEffectType = EffectType.No_Type;

            SelectedPart = null;
            SelectedMeasure = new List<Measure>();
            SelectedChord = new List<Chord>();
            SelectedNote = new List<Note>();
            SelectedEffect = null;

            Position = null;
            NumBeats = null;
            BPM = null;
            String = null;
            Fret = null;
            BendAmount = null;
            Returns = null;
            Wide = null;
            Legato = null;
        }

        public void Clear()
        {
            SelectedPart = null;
            SelectedMeasure.Clear();
            SelectedChord.Clear();
            SelectedNote.Clear();
            SelectedEffect = null;

            Position = null;
            NumBeats = null;
            BPM = null;
            String = null;
            Fret = null;
            BendAmount = null;
            Returns = null;
            Wide = null;
            Legato = null;
        }

        public Measure getFirstMeasureByPosition()
        {
            return (from measure in SelectedMeasure
                    orderby measure.Position.Index
                    select measure).FirstOrDefault();
        }
    }
}
