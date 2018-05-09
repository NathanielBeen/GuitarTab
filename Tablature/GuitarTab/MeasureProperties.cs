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
    class MeasureProperties : BaseValidator, IPropertyMenu
    {
        private Measure measure;
        private GuiCommandExecutor executor;

        private int num_beats;
        public string NumBeats
        {
            get { return num_beats.ToString(); }
            set
            {
                if (ValidateProperty(nameof(NumBeats), value, validateNumBeats))
                {
                    SetProperty(ref num_beats, int.Parse(value));
                }
            }
        }

        private NoteLength beat_type;
        public string BeatType
        {
            get { return beat_type.getStringFromNoteLength(); }
            set
            {
                if (ValidateProperty(nameof(BeatType), value, validateBeatType))
                {
                    SetProperty(ref beat_type, NoteLengthExtensions.getNoteLengthFromVisualLength(int.Parse(value)));
                }
            }
        }

        private int bpm;
        public string BPM
        {
            get { return bpm.ToString(); }
            set
            {
                if(ValidateProperty(nameof(BPM), value, validateBPM))
                {
                    SetProperty(ref bpm, int.Parse(value));
                }
            }
        }

        public MeasureProperties(Measure m, GuiCommandExecutor exec)
        {
            measure = m;
            executor = exec;
        }

        public void resetToDefault()
        {
            NumBeats = measure.TimeSignature.NumberOfBeats.ToString();
            BeatType = measure.TimeSignature.BeatType.getVisualNoteLength().ToString();
            BPM = measure.Bpm.ToString();
        }

        public void submitChanges()
        {
            if (num_beats != measure.TimeSignature.NumberOfBeats || beat_type != measure.TimeSignature.BeatType)
            {
                executor.executeChangeMeasureTimeSigFromMenu(num_beats, beat_type);
            }
            if (bpm != measure.Bpm) { executor.executeChangeMeasureBpmFromMenu(bpm); }
        }

        //write a validator for num_beats, beat_type, and bpm
        public ICollection<string> validateNumBeats(string new_beats)
        {
            var errors = new List<string>();
            int beats = 0;

            if (int.TryParse(new_beats, out beats))
            {
                if (beats <= 0 || beats > 32) { errors.Add("must be between 1 and 32"); }
            }
            else { errors.Add("must be a number"); }
            return errors;
        }

        public ICollection<string> validateBeatType(string new_type)
        {
            var errors = new List<string>();
            int length = 0;

            if (int.TryParse(new_type, out length))
            {
                NoteLength type = NoteLengthExtensions.getNoteLengthFromVisualLength(length);
                if (type == NoteLength.None) { errors.Add("must be a valid beat type"); }
            }
            else { errors.Add("must be a number"); }
            return errors;
        }

        public ICollection<string> validateBPM(string new_bpm)
        {
            var errors = new List<string>();
            int n_bpm = 0;

            if (int.TryParse(new_bpm, out n_bpm))
            {
                if (n_bpm < 20 || n_bpm > 250) { errors.Add("must be between 20 and 250"); }
            }
            else { errors.Add("must be a number"); }
            return errors;
        }
    }
}
