using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class CommandExecutor
    {
        public bool executeCommand(IActionBuilder builder)
        {
            var validator = builder.buildValidator();
            if (validator.validateAction())
            {
                var command = builder.buildCommand();
                command?.executeAction();
                return true;
            }
            return false;
        }
    }

    public enum CommandType
    {
        AddMeasureToPart,
        AddRestToMeasure,
        AddRestToPart,
        AddNoteToChord,
        AddNoteToMeasure,
        AddNoteToPart,
        RemoveNoteFromChord,
        RemoveMultipleNotesFromChord,
        RemoveChordFromMeasure,
        RemoveMultipleChordsFromMeasure,
        ChangeChordLength,
        ChangeNoteString,
        ChangeNoteFret,
        ChangeNotePosition,
        ChangeNotePositionNewChord,
        ChangeNotePositionNewMeasure,
        ChangeChordPosition,
        ChangeChordPositionNewMeasure,
        ChangeMultipleChordPosition,
        ChangeMultipleChordPositionNewMeasure,
        AddPalmMuteEffect,
        AddSingleNoteEffect,
        RemoveNoteEffect,
        AddMultiNoteEffect,
        RemoveMeasureFromPart,
        RemoveMultipleMeasuresFromPart,
        ChangeMeasurePosition,
        ChangeMultipleMeasurePosition,
        ChangeMeasureBPM,
        ChangeMeasureTimeSig,
        CreateTupletFromNotes
    }

    public static class ExecutorFactory
    {
        public static IActionBuilder getBuilderFromType(CommandType type, CommandSelections selections)
        {
            switch (type)
            {
                case CommandType.AddMeasureToPart:
                    return new AddMeasureToPartBld(selections);
                case CommandType.AddRestToMeasure:
                    return new AddRestChordToMeasureBld(selections);
                case CommandType.AddRestToPart:
                    return new AddRestChordToPartBld(selections);
                case CommandType.AddNoteToChord:
                    return new AddNoteToChordBld(selections);
                case CommandType.AddNoteToMeasure:
                    return new AddNoteToMeasureBld(selections);
                case CommandType.AddNoteToPart:
                    return new AddNoteToPartBld(selections);
                case CommandType.RemoveNoteFromChord:
                    return new RemoveNoteFromChordBld(selections);
                case CommandType.RemoveMultipleNotesFromChord:
                    return new RemoveMultipleNotesFromChordBld(selections);
                case CommandType.RemoveChordFromMeasure:
                    return new RemoveChordFromMeasureBld(selections);
                case CommandType.RemoveMultipleChordsFromMeasure:
                    return new RemoveMultipleChordsFromMeasureBld(selections);
                case CommandType.ChangeChordLength:
                    return new ChangeChordLengthBld(selections);
                case CommandType.ChangeNoteString:
                    return new ChangeNoteStringBld(selections);
                case CommandType.ChangeNoteFret:
                    return new ChangeNoteFretBld(selections);
                case CommandType.ChangeNotePosition:
                    return new ChangeNotePositionBld(selections);
                case CommandType.ChangeNotePositionNewChord:
                    return new ChangeNotePositionNewChordBld(selections);
                case CommandType.ChangeNotePositionNewMeasure:
                    return new ChangeNotePositionNewMeasureBld(selections);
                case CommandType.ChangeChordPosition:
                    return new ChangeChordPositionBld(selections);
                case CommandType.ChangeChordPositionNewMeasure:
                    return new ChangeChordPositionNewMeasureBld(selections);
                case CommandType.ChangeMultipleChordPosition:
                    return new ChangeMultipleChordPositionBld(selections);
                case CommandType.ChangeMultipleChordPositionNewMeasure:
                    return new ChangeMultipleChordPositionNewMeasureBld(selections);
                case CommandType.AddPalmMuteEffect:
                    return new AddPalmMuteEffectBld(selections);
                case CommandType.AddSingleNoteEffect:
                    return new AddSingleNoteEffectBld(selections);
                case CommandType.RemoveNoteEffect:
                    return new RemoveNoteEffectBld(selections);
                case CommandType.AddMultiNoteEffect:
                    return new AddMultiNoteEffectBld(selections);
                case CommandType.RemoveMeasureFromPart:
                    return new RemoveMeasureFromPartBld(selections);
                case CommandType.RemoveMultipleMeasuresFromPart:
                    return new RemoveMultipleMeasuresFromPartBld(selections);
                case CommandType.ChangeMeasurePosition:
                    return new ChangeMeasurePositionBld(selections);
                case CommandType.ChangeMultipleMeasurePosition:
                    return new ChangeMultipleMeasurePositionBld(selections);
                case CommandType.ChangeMeasureBPM:
                    return new ChangeMeasureBpmBld(selections);
                case CommandType.ChangeMeasureTimeSig:
                    return new ChangeMeasureTimeSigBld(selections);
                case CommandType.CreateTupletFromNotes:
                    return new CreateTupletFromNotesBld(selections);
                default:
                    return null;
            }
        }
    }
}
