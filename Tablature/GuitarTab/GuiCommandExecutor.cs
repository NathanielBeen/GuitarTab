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

    //when a note is selected with a multieffect, instead of trying to save wierd stuff, have a menu pop up like when choosing a fret that
    //asks the user to select the note to tie/slide/ect to. once a note is selected, complete all the standard checks.
    public class GuiCommandExecutor
    {
        public GuiTreeUpdater Updater { get; set; }
        public MouseHandler Handler { get; set; }
        private CommandExecutor executor;
        private CommandSelections selections;
        private VisualInfo info;

        public event EventHandler<FretMenuEventArgs> FretMenuLaunched;
        public event EventHandler<NoteSelectLaunchEventArgs> NoteSelectMenuLaunched;

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

            if (result)
            {
                Updater.populateMouseClick(click);
                runUpdate(click, update);
                if (rebar) { Updater.rebarMeasures(click.PartNode, click.MeasureNodes); }
            }

            click.setHandled();
            selections.Clear();
            Handler.populateSelected(click);
        }

        public void executeFretMenuBase(Action<NodeClick, int> action, NodeClick click)
        {
            var args = new FretMenuEventArgs(click, action);
            FretMenuLaunched?.Invoke(this, args);
            click.setHandled();
        }

        public void executeNoteSelectMenuBase(Action<NodeClick> action, NodeClick click)
        {
            var args = new NoteSelectLaunchEventArgs(click, action);
            NoteSelectMenuLaunched?.Invoke(this, args);
            click.setHandled();
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
                    if (first_node == null) { return; }

                    MeasureTreeNode prev_node = click.PartNode.getMeasureNodeAtPosition(first_node.getMeasure().Position.Index - 1);
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

            executeCommandBase(click, CommandType.AddMeasureToPart, UpdateType.UpdatePart, false);
        }

        public void executeAddRestChordToMeasure(NodeClick click, int position)
        {
            selections.Position = position;

            executeCommandBase(click, CommandType.AddRestToMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddRestChordToPart(NodeClick click)
        {
            Part part = click.PartNode.getPart();
            selections.BPM = part.DefaultBPM;
            selections.NumBeats = part.TimeSignature.NumberOfBeats;
            selections.BeatType = part.TimeSignature.BeatType;
            selections.Position = part.ModelCollection.Count();

            executeCommandBase(click, CommandType.AddRestToPart, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToChord(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            executeFretMenuBase(continueAddNoteToChord, click);
        }

        public void continueAddNoteToChord(NodeClick click, int fret)
        {
            selections.Fret = fret;

            executeCommandBase(click, CommandType.AddNoteToChord, UpdateType.UpdateChord, false);
        }

        public void executeAddNoteToMeasure(NodeClick click, int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            executeFretMenuBase(continueAddNoteToMeasure, click);
        }

        public void continueAddNoteToMeasure(NodeClick click, int fret)
        {
            selections.Fret = fret;

            executeCommandBase(click, CommandType.AddNoteToMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToPart(NodeClick click)
        {
            Part part = click.PartNode.getPart();
            selections.BPM = part.DefaultBPM;
            selections.NumBeats = part.TimeSignature.NumberOfBeats;
            selections.BeatType = part.TimeSignature.BeatType;
            selections.Position = part.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            executeFretMenuBase(continueAddNoteToPart, click);
        }

        public void continueAddNoteToPart(NodeClick click, int fret)
        {
            selections.Fret = fret;

            executeCommandBase(click, CommandType.AddNoteToPart, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveNote(NodeClick click)
        {
            executeCommandBase(click, CommandType.RemoveNoteFromChord, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleNotes(NodeClick click)
        {
            executeCommandBase(click, CommandType.RemoveMultipleNotesFromChord, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveChord(NodeClick click)
        {
            executeCommandBase(click, CommandType.RemoveChordFromMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleChords(NodeClick click)
        {
            executeCommandBase(click, CommandType.RemoveMultipleChordsFromMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordLengthFromMenu(NodeClick click, Length new_length)
        {
            selections.SelectedLength = new_length;
            executeChangeChordLength(click);
        }

        public void executeChangeChordLength(NodeClick click)
        {
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
            executeCommandBase(click, CommandType.ChangeNoteString, UpdateType.UpdateChord, false);
        }

        public void executeChangeNoteFret(NodeClick click)
        {
            executeFretMenuBase(continueChangeNoteFret, click);
        }

        public void continueChangeNoteFret(NodeClick click, int fret)
        {
            selections.Fret = fret;

            executeCommandBase(click, CommandType.ChangeNoteFret, UpdateType.UpdateChord, false);
            Updater.updateDrawing(click.NoteNodes.FirstOrDefault());
        }

        public void executeChangeNotePosition(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            executeCommandBase(click, CommandType.ChangeNotePosition, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewChord(NodeClick click, int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            executeCommandBase(click, CommandType.ChangeNotePositionNewChord, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewMeasure(NodeClick click)
        {
            Part part = click.PartNode.getPart();
            selections.BPM = part.DefaultBPM;
            selections.NumBeats = part.TimeSignature.NumberOfBeats;
            selections.BeatType = part.TimeSignature.BeatType;
            selections.Position = part.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)click.Point.Y);

            click.populateCommandSelections(selections);
            executeCommandBase(click, CommandType.ChangeNotePositionNewMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPosition(NodeClick click, int position)
        {
            selections.Position = position;

            executeCommandBase(click, CommandType.ChangeChordPosition, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPositionNewMeasure(NodeClick click)
        {
            Part part = click.PartNode.getPart();
            selections.BPM = part.DefaultBPM;
            selections.NumBeats = part.TimeSignature.NumberOfBeats;
            selections.BeatType = part.TimeSignature.BeatType;
            selections.Position = part.ModelCollection.Count();

            executeCommandBase(click, CommandType.ChangeChordPositionNewMeasure, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPosition(NodeClick click, int position)
        {
            selections.Position = position;

            executeCommandBase(click, CommandType.ChangeMultipleChordPosition, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPositionNewMeasure(NodeClick click)
        {
            Part part = click.PartNode.getPart();
            selections.BPM = part.DefaultBPM;
            selections.NumBeats = part.TimeSignature.NumberOfBeats;
            selections.BeatType = part.TimeSignature.BeatType;
            selections.Position = part.ModelCollection.Count();

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
            if (selections.SelectedEffectType == EffectType.Palm_Mute){ executeCommandBase(click, CommandType.AddPalmMuteEffect, UpdateType.UpdateMeasure, false); }
            else { executeCommandBase(click, CommandType.AddSingleNoteEffect, UpdateType.UpdateMeasure, false); }
        }

        public void executeRemoveEffectFromNote(NodeClick click, IEffect effect)
        {
            selections.SelectedEffect = effect;

            executeCommandBase(click, CommandType.RemoveNoteEffect, UpdateType.UpdateMeasure, false);
        }

        public void executeAddMultiEffectToNotesMenu(NodeClick click)
        {
            selections.Legato = false;
            executeNoteSelectMenuBase(executeAddMultiEffectToNotes, click);
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
            executeCommandBase(click, CommandType.AddMultiNoteEffect, UpdateType.UpdateMeasure, false);
        }

        public void executeAddMeasureToPart(NodeClick click, int position)
        {
            //in the future, add selected bpm and time sig controls that auto update the selections, but for now,
            // simply use the part's
            Part part = click.PartNode.getPart();
            selections.BPM = part.DefaultBPM;
            selections.NumBeats = part.TimeSignature.NumberOfBeats;
            selections.BeatType = part.TimeSignature.BeatType;
            selections.Position = position;

            executeCommandBase(click, CommandType.AddMeasureToPart, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMeasure(NodeClick click)
        {
            executeCommandBase(click, CommandType.RemoveMeasureFromPart, UpdateType.UpdatePart, false);
        }

        public void executeRemoveMultipleMeasures(NodeClick click)
        {
            executeCommandBase(click, CommandType.RemoveMultipleMeasuresFromPart, UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasurePosition(NodeClick click, int position)
        {
            selections.Position = position;

            executeCommandBase(click, CommandType.ChangeMeasurePosition, UpdateType.UpdatePart, false);
        }

        public void executeChangeMultipleMeasurePosition(NodeClick click, int position)
        {
            selections.Position = position;

            executeCommandBase(click, CommandType.ChangeMultipleMeasurePosition, UpdateType.UpdatePart, false);
        }

        public void executeChangeMeasureTimeSigFromMenu(NodeClick click, int beats, NoteLength type)
        {
            selections.NumBeats = beats;
            selections.BeatType = type;

            executeChangeMeasureTimeSig(click);
        }

        public void executeChangeMeasureTimeSig(NodeClick click)
        {
            executeCommandBase(click, CommandType.ChangeMeasureTimeSig, UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMeasureBpmFromMenu(NodeClick click, int bpm)
        {
            selections.BPM = bpm;
            executeChangeMeasureBpm(click);
        }

        public void executeChangeMeasureBpm(NodeClick click)
        {
            executeCommandBase(click, CommandType.ChangeMeasureBPM, UpdateType.UpdateMeasure, true);
        }
    }
}
