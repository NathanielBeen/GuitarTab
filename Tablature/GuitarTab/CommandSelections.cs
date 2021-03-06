﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class CommandSelections : BaseViewModel
    {
        private MouseStateConverter converter;

        private NoteLength selected_length;
        public NoteLength SelectedLength
        {
            get { return selected_length; }
            set { SetProperty(ref selected_length, value); }
        }

        private int bpm;
        public int BPM
        {
            get { return bpm; }
            set { SetProperty(ref bpm, value); }
        }

        private int num_beats;
        public int NumBeats
        {
            get { return num_beats; }
            set { SetProperty(ref num_beats, value); }
        }

        private NoteLength beat_type;
        public NoteLength BeatType
        {
            get { return beat_type; }
            set { SetProperty(ref beat_type, value); }
        }

        public EffectType SelectedEffectType
        {
            get { return converter.SelectedEffectType; }
            set { converter.SelectedEffectType = value; }
        }

        private TupletType tuplet_type;
        public TupletType TupletType
        {
            get { return tuplet_type; }
            set { SetProperty(ref tuplet_type, value); }
        }

        public Part SelectedPart { get; set; }
        public List<Measure> SelectedMeasure { get; private set; }
        public List<Chord> SelectedChord { get; private set; }
        public List<Note> SelectedNote { get; private set; }
        public IEffect SelectedEffect { get; set; }

        public int? Position { get; set; }
        public int? String { get; set; }
        public int? Fret { get; set; }

        public double? BendAmount { get; set; }

        public bool? Returns { get; set; }
        public bool? Wide { get; set; }
        public bool? Legato { get; set; }

        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public InstrumentType? Instrument { get; set; }
        public int? StringNum { get; set; }

        public CommandSelections(MouseStateConverter conv)
        {
            converter = conv;
            SelectedLength = NoteLength.None;
            SelectedEffectType = EffectType.No_Type;

            SelectedPart = null;
            SelectedMeasure = new List<Measure>();
            SelectedChord = new List<Chord>();
            SelectedNote = new List<Note>();
            SelectedEffect = null;

            Position = null;
            String = null;
            Fret = null;
            BendAmount = null;
            Returns = null;
            Wide = null;
            Legato = null;

            Name = null;
            Artist = null;
            Album = null;
            Instrument = null;
            StringNum = null;
        }

        public void Clear()
        {
            SelectedPart = null;
            SelectedMeasure.Clear();
            SelectedChord.Clear();
            SelectedNote.Clear();
            SelectedEffect = null;

            Position = null;
            String = null;
            Fret = null;
            BendAmount = null;
            Returns = null;
            Wide = null;
            Legato = null;

            Name = null;
            Artist = null;
            Album = null;
            Instrument = null;
            StringNum = null;
        }

        public void ClearModel()
        {
            SelectedPart = null;
            SelectedMeasure.Clear();
            SelectedChord.Clear();
            SelectedNote.Clear();
            SelectedEffect = null;
        }

        public Measure getFirstMeasureByPosition()
        {
            return (from measure in SelectedMeasure
                    orderby measure.Position.Index
                    select measure).FirstOrDefault();
        }
    }
}
