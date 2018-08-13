using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GuitarTab
{
    class MeasureProperties : BasePropertyMenu
    {
        public const int NUM_BEATS_MIN = 1;
        public const int NUM_BEATS_MAX = 32;
        public const int BPM_MIN = 20;
        public const int BPM_MAX = 300;

        private Measure measure;

        private string num_beats;
        public string NumBeats
        {
            get { return num_beats.ToString(); }
            set
            {
                string error = NumBeatsError;
                setIntProperty(ref num_beats, value, NUM_BEATS_MIN, NUM_BEATS_MAX, ref error);
                NumBeatsError = error;
            }
        }

        private string num_beats_error;
        public string NumBeatsError
        {
            get { return num_beats_error; }
            set { SetProperty(ref num_beats_error, value); }
        }

        private string beat_type;
        public string BeatType
        {
            get { return beat_type; }
            set
            {
                string error = BeatTypeError;
                setTimeSigNoteLengthProperty(ref beat_type, value, ref error);
                BeatTypeError = error;
            }
        }

        private string beat_type_error;
        public string BeatTypeError
        {
            get { return beat_type_error; }
            set { SetProperty(ref beat_type_error, value); }
        }

        private string bpm;
        public string BPM
        {
            get { return bpm.ToString(); }
            set
            {
                string error = BpmError;
                setIntProperty(ref bpm, value, BPM_MIN, BPM_MAX, ref error);
                BpmError = error;
            }
        }

        private string bpm_error;
        public string BpmError
        {
            get { return bpm_error; }
            set { SetProperty(ref bpm_error, value); }
        }

        public MeasureProperties(MeasureTreeNode m, GuiCommandExecutor exec, NodeClick c)
            :base(c, exec)
        {
            measure = m.getMeasure();
            resetToDefault();
        }

        public override void resetToDefault()
        {
            NumBeats = measure.TimeSignature.NumberOfBeats.ToString();
            BeatType = measure.TimeSignature.BeatType.getVisualNoteLength().ToString();
            BPM = measure.Bpm.ToString();
        }

        public override void submitChanges()
        {
            int beat_type_i = (Int32.TryParse(BeatType, out int beat)) ? beat : 0;
            NoteLength beat_type_n = NoteLengthExtensions.getNoteLengthFromVisualLength(beat_type_i);
            if (NumBeatsError == String.Empty || BeatTypeError == String.Empty || BpmError == String.Empty || !Int32.TryParse(NumBeats, out int num_beats_i)
                || beat_type_n == NoteLength.None || !Int32.TryParse(BPM, out int bpm_i)) { return; }

            if (num_beats_i != measure.TimeSignature.NumberOfBeats || beat_type_n != measure.TimeSignature.BeatType)
            {
                executor.executeChangeMeasureTimeSigFromMenu(getClickCopy(), num_beats_i, beat_type_n);
            }
            if (bpm_i != measure.Bpm) { executor.executeChangeMeasureBpmFromMenu(getClickCopy(), bpm_i); }
        }
    }
}
