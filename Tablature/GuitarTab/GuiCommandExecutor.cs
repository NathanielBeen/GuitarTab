using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public enum UpdateType
    {
        UpdatePart,
        UpdateMeasuresAtAndAfter,
        UpdateMeasure,
        UpdateChord
    }

    public class GuiCommandExecutor
    {
        public GuiTreeUpdater Updater { get; set; }
        private CommandExecutor executor;
        private CommandSelections selections;
        private VisualInfo info;

        public event EventHandler<FretMenuEventArgs> FretMenuLaunched;

        public GuiCommandExecutor(CommandExecutor ex, CommandSelections s, VisualInfo v_info)
        {
            executor = ex;
            selections = s;
            info = v_info;
        }

        public void executeCommandBase(NodeClick click, CommandType type, UpdateType update, bool rebar)
        {
            click.populateCommandSelections(selections);
            IActionBuilder builder = ExecutorFactory.getBuilderFromType(type, selections);

            bool result = executor.executeCommand(builder);
            Updater.populateMouseClick(click);

            if (result)
            {
                runUpdate(click, update);
                if (rebar) { Updater.rebarMeasures(click.PartNode, click.MeasureNodes); }
            }

            click.Handled();
            selections.Clear();
        }

        public void executeFretMenuBase(Action<NodeClick, int> action, NodeClick click)
        {
            var args = new FretMenuEventArgs(click, action);
            FretMenuLaunched?.Invoke(this, args);
            click.Handled();
        }

        public void runUpdate(NodeClick click, UpdateType update)
        {
            switch (update)
            {
                case UpdateType.UpdatePart:
                    Updater.updatePartBounds(click.PartNode);
                    break;
                case UpdateType.UpdateMeasuresAtAndAfter:
                    MeasureTreeNode first_node = click.getFirstMeasureNodeByPosition();
                    MeasureTreeNode prev_node = click.PartNode.getMeasureNodeAtPosition(first_node.getMeasure().Position.Index);
                    List<MeasureTreeNode> to_update = click.PartNode.getMeasureNodesAtAndAfterPosition(first_node.getMeasure().Position.Index);
                    Updater.updateMeasureBoundsAtAndAfter(prev_node, to_update);
                    break;
                case UpdateType.UpdateMeasure:
                    Updater.updateMeasureBounds(click.MeasureNodes.FirstOrDefault());
                    break;
                case UpdateType.UpdateChord:
                    Updater.updateChordBounds(click.ChordNodes.FirstOrDefault());
                    break;
            }
        }

        public void executeInitPart(NodeClick click, int bpm, int num_beats, NoteLength beat_type)
        {
            //once menus are put in to init files, replace below
            selections.BPM = bpm;
            selections.NumBeats = num_beats;
            selections.BeatType = beat_type;
            selections.Position = 0;

            var builder = new InitPartBld(selections);
            executor.executeCommand(builder);

            Updater.setTreePart(selections.SelectedPart);
            Updater.populateMouseClick(click);

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddMeasureToPart, UpdateType.UpdatePart, false);
        }

        public void executeAddRestChordToMeasure(NodeClick click, int position)
        {
            selections.Position = position;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddRestToMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddRestChordToPart(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddRestToPart, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToChord(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeFretMenuBase(continueAddNoteToChord, click);
        }

        public void continueAddNoteToChord(NodeClick click, int fret)
        {
            selections.Fret = fret;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddNoteToChord, UpdateType.UpdateChord, false);
        }

        public void executeAddNoteToMeasure(NodeClick click, int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeFretMenuBase(continueAddNoteToMeasure, click);
        }

        public void continueAddNoteToMeasure(NodeClick click, int fret)
        {
            selections.Fret = fret;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddNoteToMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToPart(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeFretMenuBase(continueAddNoteToPart, click);
        }

        public void continueAddNoteToPart(NodeClick click, int fret)
        {
            selections.Fret = fret;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddNoteToPart, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveNote(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveNoteFromChord, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleNotes(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveMultipleNotesFromChord, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveChord(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveChordFromMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleChords(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveMultipleChordsFromMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordLengthFromMenu(NodeClick click, Length new_length)
        {
            selections.SelectedLength = new_length;
            executeChangeChordLength(click);
        }

        public void executeChangeChordLength(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeChordLength, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNoteStringFromPosition(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);
            executeChangeNoteString(click);
        }

        public void executeChangeNoteStringFromMenu(NodeClick click, int str)
        {
            selections.String = str;
            executeChangeNoteString(click);
        }

        public void executeChangeNoteString(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeNoteString, UpdateType.UpdateChord, false);
        }

        public void executeChangeNoteFret(NodeClick click)
        {
            executeFretMenuBase(continueChangeNoteFret, click);
        }

        public void continueChangeNoteFret(NodeClick click, int fret)
        {
            selections.Fret = fret;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeNoteFret, UpdateType.UpdateChord, false);
            Updater.updateDrawing(click.NoteNodes.FirstOrDefault());
        }

        public void executeChangeNotePosition(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeNotePosition, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewChord(NodeClick click, int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeNotePositionNewChord, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewMeasure(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeNotePositionNewMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPosition(NodeClick click, int position)
        {
            selections.Position = position;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeChordPosition, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPositionNewMeasure(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeChordPositionNewMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPosition(NodeClick click, int position)
        {
            selections.Position = position;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeMultipleChordPosition, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPositionNewMeasure(NodeClick click)
        {
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeMultipleChordPositionNewMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddEffectToNoteProp(NodeClick click, EffectType type)
        {
            selections.SelectedEffectType = type;
            executeAddEffectToNote(click);
        }

        public void executeAddBendToNoteProp(NodeClick click, double amount, bool returns)
        {
            selections.BendAmount = amount;
            selections.Returns = returns;
            executeAddEffectToNote(click);
        }

        public void executeAddVibratoToNoteProp(NodeClick click, bool wide)
        {
            selections.Wide = wide;
            executeAddEffectToNote(click);
        }

        public void executeAddEffectToNote(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddSingleNoteEffect, UpdateType.UpdateMeasure, false);
        }

        public void executeRemoveEffectFromNote(NodeClick click, IEffect effect)
        {
            selections.SelectedEffect = effect;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveNoteEffect, UpdateType.UpdateMeasure, false);
        }


        public void executeAddSlideToNoteProp(NodeClick click, Note first, Note second, bool legato)
        {
            selections.Legato = legato;

            executeAddMultiEffectToNoteProp(click, first, second, EffectType.Slide);
        }

        public void executeAddMultiEffectToNoteProp(NodeClick click, Note first, Note second, EffectType type)
        {
            selections.SelectedNote.Clear();
            selections.SelectedNote.Add(first);
            selections.SelectedNote.Add(second);
            selections.SelectedEffectType = type;

            executeAddMultiEffectToNotes(click);
        }

        public void executeAddMultiEffectToNotes(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddMultiNoteEffect, UpdateType.UpdateMeasure, false);
        }

        public void executeAddMeasureToPart(NodeClick click, int position)
        {
            //in the future, add selected bpm and time sig controls that auto update the selections, but for now,
            // simply use the part's
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = position;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.AddMeasureToPart, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMeasure(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveMeasureFromPart, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMultipleMeasures(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.RemoveMultipleMeasuresFromPart, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasurePosition(NodeClick click, int position)
        {
            selections.Position = position;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeMeasurePosition, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMultipleMeasurePosition(NodeClick click, int position)
        {
            selections.Position = position;

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeMultipleMeasurePosition, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasureTimeSigFromMenu(NodeClick click, int beats, NoteLength type)
        {
            selections.NumBeats = beats;
            selections.BeatType = type;

            executeChangeMeasureTimeSig(click);
        }

        public void executeChangeMeasureTimeSig(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeMeasureTimeSig, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasureBpmFromMenu(NodeClick click, int bpm)
        {
            selections.BPM = bpm;
            executeChangeMeasureBpm(click);
        }

        public void executeChangeMeasureBpm(NodeClick click)
        {
            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeMeasureBPM, UpdateType.UpdateMeasure, false);
        }
    }
}
