using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IActionBuilder
    {
        IActionValidator buildValidator();
        IActionCommand buildCommand();
    }

    public class InitPartBld : IActionBuilder
    {
        private InitPartAtr attributes;

        public InitPartBld(CommandSelections selection)
        {
            attributes = new InitPartAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new InitPartVal();
        }

        public IActionCommand buildCommand()
        {
            return null;
        }
    }

    public class AddChordToMeasureBld : IActionBuilder
    {
        private AddNoteChordToMeasureAtr attributes;

        public AddChordToMeasureBld(CommandSelections selection)
        {
            attributes = new AddNoteChordToMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddChordToMeasureVal(attributes.Chord, attributes.Measure);
        }

        public IActionCommand buildCommand()
        {
            return new AddChordToMeasureCom(attributes.Measure, attributes.Chord);
        }
    }

    public class AddRestChordToMeasureBld : IActionBuilder
    {
        private AddRestChordToMeasureAtr attributes;

        public AddRestChordToMeasureBld(CommandSelections selection)
        {
            attributes = new AddRestChordToMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddChordToMeasureVal(attributes.Chord, attributes.Measure);
        }

        public IActionCommand buildCommand()
        {
            return new AddChordToMeasureCom(attributes.Measure, attributes.Chord);
        }
    }

    public class AddRestChordToPartBld : IActionBuilder
    {
        private AddRestChordToPartAtr attributes;
        
        public AddRestChordToPartBld(CommandSelections selection)
        {
            attributes = new AddRestChordToPartAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddChordToPartVal(attributes.Part, attributes.Measure, attributes.Chord);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new AddMeasureToPartCom(attributes.Part, attributes.Measure));
            command.AddCommand(new AddChordToMeasureCom(attributes.Measure, attributes.Chord));
            return command;
        }
    }

    public class AddNoteToChordBld : IActionBuilder
    {
        private AddNoteToChordAtr attributes;

        public AddNoteToChordBld(CommandSelections selection)
        {
            attributes = new AddNoteToChordAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddNoteToChordVal(attributes.Chord, attributes.Note);
        }

        public IActionCommand buildCommand()
        {
            return new AddNoteToChordCom(attributes.Chord, attributes.Note);
        }
    }

    public class AddNoteToMeasureBld : IActionBuilder
    {
        private AddNoteToMeasureAtr attributes;

        public AddNoteToMeasureBld(CommandSelections selection)
        {
            attributes = new AddNoteToMeasureAtr(selection);
        }
        
        public IActionValidator buildValidator()
        {
            var validator = new MultipleActionValidator();
            validator.AddValidator(new AddChordToMeasureVal(attributes.Chord, attributes.Measure));
            validator.AddValidator(new AddNoteToChordVal(attributes.Chord, attributes.Note));
            return validator;
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new AddChordToMeasureCom(attributes.Measure, attributes.Chord));
            command.AddCommand(new AddNoteToChordCom(attributes.Chord, attributes.Note));
            return command;
        }
    }

    public class AddNoteToPartBld : IActionBuilder
    {
        private AddNoteToPartAtr attributes;

        public AddNoteToPartBld(CommandSelections selection)
        {
            attributes = new AddNoteToPartAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddNoteToPartVal(attributes.Part, attributes.Measure, attributes.Chord, attributes.Note);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new AddMeasureToPartCom(attributes.Part, attributes.Measure));
            command.AddCommand(new AddChordToMeasureCom(attributes.Measure, attributes.Chord));
            command.AddCommand(new AddNoteToChordCom(attributes.Chord, attributes.Note));
            return command;
        }
    }

    public class RemoveNoteFromChordBld : IActionBuilder
    {
        private RemoveNoteFromChordAtr attributes;

        public RemoveNoteFromChordBld(CommandSelections selection)
        {
            attributes = new RemoveNoteFromChordAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new RemoveNoteFromChordVal(attributes.Measure, attributes.Chord, attributes.Note);
        }

        public IActionCommand buildCommand()
        {
            if (attributes.Chord.ModelCollection.Count() == 1)
            {
                var command = new MultipleActionCommand();
                command.AddCommand(new RemoveNoteFromChordCom(attributes.Chord, attributes.Note));
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.Measure, attributes.Chord));
                return command;
            }

            return new RemoveNoteFromChordCom(attributes.Chord, attributes.Note);
        }
    }

    public class RemoveMultipleNotesFromChordBld : IActionBuilder
    {
        private RemoveMultipleNotesFromChordAtr attributes;

        public RemoveMultipleNotesFromChordBld(CommandSelections selection)
        {
            attributes = new RemoveMultipleNotesFromChordAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            var validator = new MultipleActionValidator();
            foreach (Note note in attributes.Notes)
            {
                validator.AddValidator(new RemoveNoteFromChordVal(attributes.Measure, attributes.Chord, note));
            }
            return validator;
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            foreach (Note note in attributes.Notes)
            {
                command.AddCommand(new RemoveNoteFromChordCom(attributes.Chord, note));
            }
            if (attributes.Chord.ModelCollection.Count() == attributes.Notes.Count)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.Measure, attributes.Chord));
            }
            return command;
        }
    }

    public class RemoveChordFromMeasureBld : IActionBuilder
    {
        private RemoveChordFromMeasureAtr attributes;

        public RemoveChordFromMeasureBld(CommandSelections selection)
        {
            attributes = new RemoveChordFromMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new RemoveChordFromMeasureVal(attributes.Measure, attributes.Chord);
        }

        public IActionCommand buildCommand()
        {
            return new RemoveChordFromMeasureCom(attributes.Measure, attributes.Chord);
        }
    }

    public class RemoveMultipleChordsFromMeasureBld : IActionBuilder
    {
        private RemoveMultipleChordsFromMeasureAtr attributes;

        public RemoveMultipleChordsFromMeasureBld(CommandSelections selection)
        {
            attributes = new RemoveMultipleChordsFromMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            var validator = new MultipleActionValidator();
            foreach (Chord chord in attributes.Chords)
            {
                validator.AddValidator(new RemoveChordFromMeasureVal(attributes.Measure, chord));
            }
            return validator;
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            foreach (Chord chord in attributes.Chords)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.Measure, chord));
            }
            return command;
        }
    }

    public class ChangeNotePositionBld : IActionBuilder
    {
        private ChangeNotePositionAtr attributes;

        public ChangeNotePositionBld(CommandSelections selection)
        {
            attributes = new ChangeNotePositionAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeNotePositionVal(attributes.Measure, attributes.FirstChord, attributes.SecondChord, attributes.Note, attributes.NoteString);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new RemoveNoteFromChordCom(attributes.FirstChord, attributes.Note));
            if (attributes.FirstChord.ModelCollection.Count() == 1)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.Measure, attributes.FirstChord));
            }
            command.AddCommand(new ChangeNoteStringCom(attributes.Note, (int)attributes.NoteString));
            command.AddCommand(new AddNoteToChordCom(attributes.SecondChord, attributes.Note));

            return command;
        }
    }

    public class ChangeNotePositionNewChordBld : IActionBuilder
    {
        private ChangeNotePositionNewChordAtr attributes;

        public ChangeNotePositionNewChordBld(CommandSelections selection)
        {
            attributes = new ChangeNotePositionNewChordAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeNotePositionNewChordVal(attributes.FirstMeasure, attributes.SecondMeasure, attributes.FirstChord, attributes.SecondChord, attributes.Note, attributes.NoteString);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new RemoveNoteFromChordCom(attributes.FirstChord, attributes.Note));
            if (attributes.FirstChord.ModelCollection.Count() == 1)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.FirstMeasure, attributes.FirstChord));
            }
            command.AddCommand(new ChangeNoteStringCom(attributes.Note, (int)attributes.NoteString));
            command.AddCommand(new AddChordToMeasureCom(attributes.SecondMeasure, attributes.SecondChord));
            command.AddCommand(new AddNoteToChordCom(attributes.SecondChord, attributes.Note));
            return command;
        }
    }

    public class ChangeNotePositionNewMeasureBld : IActionBuilder
    {
        private ChangeNotePositionNewMeasureAtr attributes;

        public ChangeNotePositionNewMeasureBld(CommandSelections selection)
        {
            attributes = new ChangeNotePositionNewMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeNotePositionNewMeasureVal(attributes.FirstMeasure, attributes.SecondMeasure, attributes.FirstChord, attributes.SecondChord, attributes.Note, attributes.NoteString);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new RemoveNoteFromChordCom(attributes.FirstChord, attributes.Note));
            if (attributes.FirstChord.ModelCollection.Count() == 1)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.FirstMeasure, attributes.FirstChord));
            }
            command.AddCommand(new ChangeNoteStringCom(attributes.Note, (int)attributes.NoteString));
            command.AddCommand(new AddMeasureToPartCom(attributes.Part, attributes.FirstMeasure));
            command.AddCommand(new AddChordToMeasureCom(attributes.SecondMeasure, attributes.SecondChord));
            command.AddCommand(new AddNoteToChordCom(attributes.SecondChord, attributes.Note));

            return command;
        }
    }

    public class ChangeChordPositionBld : IActionBuilder
    {
        private ChangeChordPositionAtr attributes;

        public ChangeChordPositionBld(CommandSelections selection)
        {
            attributes = new ChangeChordPositionAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeChordPositionVal(attributes.Chord, attributes.FirstMeasure,
                                              attributes.SecondMeasure, (int)attributes.Position);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new RemoveChordFromMeasureCom(attributes.FirstMeasure, attributes.Chord));
            attributes.Chord.Position.Index = (int)attributes.Position;
            command.AddCommand(new AddChordToMeasureCom(attributes.SecondMeasure, attributes.Chord));
            return command;
        }
    }

    public class ChangeChordPositionNewMeasureBld : IActionBuilder
    {
        private ChangeChordPositionNewMeasureAtr attributes;

        public ChangeChordPositionNewMeasureBld(CommandSelections selection)
        {
            attributes = new ChangeChordPositionNewMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeChordPositionVal(attributes.Chord, attributes.FirstMeasure, attributes.SecondMeasure, attributes.Position);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new RemoveChordFromMeasureCom(attributes.FirstMeasure, attributes.Chord));
            attributes.Chord.Position.Index = attributes.Position;
            command.AddCommand(new AddMeasureToPartCom(attributes.Part, attributes.SecondMeasure));
            command.AddCommand(new AddChordToMeasureCom(attributes.SecondMeasure, attributes.Chord));
            return command;
        }
    }

    public class ChangeMultipleChordPositionBld : IActionBuilder
    {
        private ChangeMultipleChordPositionAtr attributes;

        public ChangeMultipleChordPositionBld(CommandSelections selection)
        {
            attributes = new ChangeMultipleChordPositionAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeMultipleChordPositionVal(attributes.ChordDict, attributes.SecondMeasure, attributes.Position);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            foreach (Chord chord in attributes.ChordDict.Keys)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.ChordDict[chord], chord));
            }

            int curr_position = (int)attributes.Position;
            foreach (Chord chord in attributes.ChordDict.Keys)
            {
                chord.Position.Index = curr_position;
                command.AddCommand(new AddChordToMeasureCom(attributes.SecondMeasure, chord));
                curr_position++;
            }
            return command;
        }
    }

    public class ChangeMultipleChordPositionNewMeasureBld : IActionBuilder
    {
        private ChangeMultipleChordPositionNewMeasureAtr attributes;

        public ChangeMultipleChordPositionNewMeasureBld(CommandSelections selection)
        {
            attributes = new ChangeMultipleChordPositionNewMeasureAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeMultipleChordPositionVal(attributes.ChordDict, attributes.SecondMeasure, attributes.Position);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new AddMeasureToPartCom(attributes.Part, attributes.SecondMeasure));
            foreach (Chord chord in attributes.ChordDict.Keys)
            {
                command.AddCommand(new RemoveChordFromMeasureCom(attributes.ChordDict[chord], chord));
            }

            int curr_position = attributes.Position;
            foreach (Chord chord in attributes.ChordDict.Keys)
            {
                chord.Position.Index = curr_position;
                command.AddCommand(new AddChordToMeasureCom(attributes.SecondMeasure, chord));
                curr_position++;
            }
            return command;
        }
    }

    public class ChangeChordLengthBld : IActionBuilder
    {
        private ChangeChordLengthAtr attributes;

        public ChangeChordLengthBld(CommandSelections selection)
        {
            attributes = new ChangeChordLengthAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeChordLengthVal(attributes.Measure, attributes.Chord, attributes.Length);
        }

        public IActionCommand buildCommand()
        {
            return new ChangeChordLengthCom(attributes.Chord, attributes.Length);
        }
    }

    public class ChangeMultipleChordLengthBld : IActionBuilder
    {
        private ChangeMultipleChordLengthAtr attributes;

        public ChangeMultipleChordLengthBld(CommandSelections selection)
        {
            attributes = new ChangeMultipleChordLengthAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeMultipleChordLengthVal(attributes.ChordDict, attributes.Length);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            foreach (Chord chord in attributes.ChordDict.Keys)
            {
                command.AddCommand(new ChangeChordLengthCom(chord, attributes.Length));
            }
            return command;
        }
    }

    public class ChangeNoteStringBld : IActionBuilder
    {
        private ChangeNoteStringAtr attributes;

        public ChangeNoteStringBld(CommandSelections selection)
        {
            attributes = new ChangeNoteStringAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeNoteStringVal(attributes.Chord, attributes.Note, attributes.NewString);
        }

        public IActionCommand buildCommand()
        {
            return new ChangeNoteStringCom(attributes.Note, (int)attributes.NewString);
        }
    }

    public class ChangeNoteFretBld : IActionBuilder
    {
        private ChangeNoteFretAtr attributes;

        public ChangeNoteFretBld(CommandSelections selection)
        {
            attributes = new ChangeNoteFretAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeNoteFretVal(attributes.Note, attributes.Fret);
        }

        public IActionCommand buildCommand()
        {
            return new ChangeNoteFretCom(attributes.Note, (int)attributes.Fret);
        }
    }

    public class AddPalmMuteEffectBld : IActionBuilder
    {
        private AddPalmMuteEffectAtr attributes;

        public AddPalmMuteEffectBld(CommandSelections selection)
        {
            attributes = new AddPalmMuteEffectAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddPalmMuteEffectVal(attributes.Chord, attributes.Effect);
        }

        public IActionCommand buildCommand()
        {
            return new AddPalmMuteEffectCom(attributes.Chord, attributes.Effect);
        }
    }

    public class AddSingleNoteEffectBld : IActionBuilder
    {
        private AddSingleNoteEffectAtr attributes;

        public AddSingleNoteEffectBld(CommandSelections selection)
        {
            attributes = new AddSingleNoteEffectAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddSingleNoteEffectVal(attributes.Note, attributes.Effect);
        }

        public IActionCommand buildCommand()
        {
            return new AddSingleNoteEffectCom(attributes.Note, attributes.Effect);
        }
    }

    public class RemoveNoteEffectBld : IActionBuilder
    {
        private RemoveNoteEffectAtr attributes;

        public RemoveNoteEffectBld(CommandSelections selection)
        {
            attributes = new RemoveNoteEffectAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new RemoveNoteEffectVal(attributes.Note, attributes.Effect);
        }

        public IActionCommand buildCommand()
        {
            return new RemoveNoteEffectCom(attributes.Note, attributes.Effect);
        }
    }

    public class AddMultiNoteEffectBld : IActionBuilder
    {
        private AddMultiNoteEffectAtr attributes;

        public AddMultiNoteEffectBld(CommandSelections selection)
        {
            attributes = new AddMultiNoteEffectAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddMultiNoteEffectVal(attributes.Effect);
        }

        public IActionCommand buildCommand()
        {
            return new AddMultiNoteEffectCom(attributes.Effect);
        }
    }

    public class AddMeasureToPartBld : IActionBuilder
    {
        private AddMeasureToPartAtr attributes;

        public AddMeasureToPartBld(CommandSelections selection)
        {
            attributes = new AddMeasureToPartAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new AddMeasureToPartVal(attributes.Part, attributes.Measure);
        }

        public IActionCommand buildCommand()
        {
            return new AddMeasureToPartCom(attributes.Part, attributes.Measure);
        }
    }

    public class RemoveMeasureFromPartBld : IActionBuilder
    {
        private RemoveMeasureFromPartAtr attributes;

        public RemoveMeasureFromPartBld(CommandSelections selection)
        {
            attributes = new RemoveMeasureFromPartAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new RemoveMeasureFromPartVal(attributes.Part, attributes.Measure);
        }

        public IActionCommand buildCommand()
        {
            return new RemoveMeasureFromPartCom(attributes.Part, attributes.Measure);
        }
    }

    public class RemoveMultipleMeasuresFromPartBld : IActionBuilder
    {
        private RemoveMultipleMeasuresFromPartAtr attributes;

        public RemoveMultipleMeasuresFromPartBld(CommandSelections selection)
        {
            attributes = new RemoveMultipleMeasuresFromPartAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            var validator = new MultipleActionValidator();
            foreach (Measure measure in attributes.Measures)
            {
                validator.AddValidator(new RemoveMeasureFromPartVal(attributes.Part, measure));
            }
            return validator;
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            foreach (Measure measure in attributes.Measures)
            {
                command.AddCommand(new RemoveMeasureFromPartCom(attributes.Part, measure));
            }
            return command;
        }
    }

    public class ChangeMeasurePositionBld : IActionBuilder
    {
        private ChangeMeasurePositionAtr attributes;

        public ChangeMeasurePositionBld(CommandSelections selection)
        {
            attributes = new ChangeMeasurePositionAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeMeasurePositionVal(attributes.Part, attributes.Measure, attributes.Position);
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            command.AddCommand(new RemoveMeasureFromPartCom(attributes.Part, attributes.Measure));
            attributes.Measure.Position.Index = (int)attributes.Position;
            command.AddCommand(new AddMeasureToPartCom(attributes.Part, attributes.Measure));
            return command;
        }
    }

    public class ChangeMultipleMeasurePositionBld : IActionBuilder
    {
        private ChangeMultipleMeasurePositionAtr attributes;

        public ChangeMultipleMeasurePositionBld(CommandSelections selection)
        {
            attributes = new ChangeMultipleMeasurePositionAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            var validator = new MultipleActionValidator();
            foreach (Measure measure in attributes.Measures)
            {
                validator.AddValidator(new ChangeMeasurePositionVal(attributes.Part, measure, attributes.Position));
            }
            return validator;
        }

        public IActionCommand buildCommand()
        {
            var command = new MultipleActionCommand();
            foreach (Measure measure in attributes.Measures)
            {
                command.AddCommand(new RemoveMeasureFromPartCom(attributes.Part, measure));
            }

            int curr_position = (int)attributes.Position;
            foreach (Measure measure in attributes.Measures)
            {
                measure.Position.Index = curr_position;
                command.AddCommand(new AddMeasureToPartCom(attributes.Part, measure));
                curr_position++;
            }
            return command;
        }
    }

    public class ChangeMeasureTimeSigBld : IActionBuilder
    {
        private ChangeMeasureTimeSigAtr attributes;

        public ChangeMeasureTimeSigBld(CommandSelections selection)
        {
            attributes = new ChangeMeasureTimeSigAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeMeasureTimeSigVal(attributes.Part, attributes.Measure, attributes.NumBeats, attributes.BeatType);
        }

        public IActionCommand buildCommand()
        {
            return new ChangeMeasureTimeSigCom(attributes.Part, attributes.Measure, (int)attributes.NumBeats, (NoteLength)attributes.BeatType);
        }
    }

    public class ChangeMeasureBpmBld : IActionBuilder
    {
        private ChangeMeasureBpmAtr attributes;

        public ChangeMeasureBpmBld(CommandSelections selection)
        {
            attributes = new ChangeMeasureBpmAtr(selection);
        }

        public IActionValidator buildValidator()
        {
            return new ChangeMeasureBpmVal(attributes.Part, attributes.Measure, attributes.Bpm);
        }

        public IActionCommand buildCommand()
        {
            return new ChangeMeasureBpmCom(attributes.Part, attributes.Measure, (int)attributes.Bpm);
        }
    }

    //change part bpm, change part time_sig
}
