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
        private MouseSelections m_selections;
        private VisualInfo info;

        public event EventHandler<FretMenuEventArgs> FretMenuLaunched;

        public GuiCommandExecutor(CommandExecutor ex, CommandSelections s, MouseSelections ms, VisualInfo v_info)
        {
            executor = ex;
            selections = s;
            m_selections = ms;
            info = v_info;
        }

        public void executeCommandBase(NodeClick click, IActionBuilder builder, UpdateType update, bool rebar)
        {
            click.populateCommandSelections(selections);
            bool result = executor.executeCommand(builder);
            if (result)
            {
                runUpdate(click, update);
                if (rebar) { Updater.rebarMeasures(click.PartNode, click.MeasureNodes); }
            }
            Updater.populateMouseClick(click);

            selections.Clear();
            selections.SelectedLength = null;
            m_selections.clearOtherSelections();
            m_selections.EventHandled = true;
        }

        public void executeFretMenuBase(Action<NodeClick, int> action)
        {
            var args = new FretMenuEventArgs(m_selections.SelectedPoint, action);
            FretMenuLaunched?.Invoke(this, args);
            m_selections.EventHandled = true;
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

            click.populateCommandSelections(selections);
            var builder = new InitPartBld(selections);
            executor.executeCommand(builder);
            Updater.populateMouseClick(click);

            Updater.setTreePart(selections.SelectedPart);

            executeCommandBase(click, new AddMeasureToPartBld(selections), UpdateType.UpdatePart, false);
        }

        public void executeAddRestChordToMeasure(NodeClick click, int position)
        {
            selections.Position = position;
            executeCommandBase(click, new AddRestChordToMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddRestChordToPart(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();

            executeCommandBase(click, new AddRestChordToPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToChord(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);

            executeFretMenuBase(continueAddNoteToChord);
        }

        public void continueAddNoteToChord(NodeClick click, int fret)
        {
            selections.Fret = fret;
            executeCommandBase(click, new AddNoteToChordBld(selections), UpdateType.UpdateChord, false);
        }

        public void executeAddNoteToMeasure(NodeClick click, int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);

            executeFretMenuBase(continueAddNoteToMeasure);
        }

        public void continueAddNoteToMeasure(NodeClick click, int fret)
        {
            selections.Fret = fret;
            executeCommandBase(click, new AddNoteToMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToPart(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);

            executeFretMenuBase(continueAddNoteToPart);
        }

        public void continueAddNoteToPart(NodeClick click, int fret)
        {
            selections.Fret = fret;
            executeCommandBase(click, new AddNoteToPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveNote(NodeClick click)
        {
            executeCommandBase(click, new RemoveNoteFromChordBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleNotes(NodeClick click)
        {
            executeCommandBase(click, new RemoveMultipleNotesFromChordBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveChord(NodeClick click)
        {
            executeCommandBase(click, new RemoveChordFromMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleChords(NodeClick click)
        {
            executeCommandBase(click, new RemoveMultipleChordsFromMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordLengthFromMenu(NodeClick click, Length new_length)
        {
            selections.SelectedLength = new_length;
            executeChangeChordLength(click);
        }

        public void executeChangeChordLength(NodeClick click)
        {
            executeCommandBase(click, new ChangeChordLengthBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNoteStringFromPosition(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeChangeNoteString(click);
        }

        public void executeChangeNoteStringFromMenu(NodeClick click, int str)
        {
            selections.String = str;
            executeChangeNoteString(click);
        }

        public void executeChangeNoteString(NodeClick click)
        {
            executeCommandBase(click, new ChangeNoteStringBld(selections), UpdateType.UpdateChord, false);
        }

        public void executeChangeNoteFret(NodeClick click)
        {
            executeFretMenuBase(continueChangeNoteFret);
        }

        public void continueChangeNoteFret(NodeClick click, int fret)
        {
            selections.Fret = fret;
            executeCommandBase(click, new ChangeNoteFretBld(selections), UpdateType.UpdateChord, false);
            Updater.updateDrawing(click.NoteNodes.FirstOrDefault());
        }

        public void executeChangeNotePosition(NodeClick click)
        {
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeCommandBase(click, new ChangeNotePositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewChord(NodeClick click, int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeCommandBase(click, new ChangeNotePositionNewChordBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewMeasure(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeCommandBase(click, new ChangeNotePositionNewMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPosition(NodeClick click, int position)
        {
            selections.Position = position;
            executeCommandBase(click, new ChangeChordPositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPositionNewMeasure(NodeClick click)
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();

            executeCommandBase(click, new ChangeChordPositionNewMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPosition(NodeClick click, int position)
        {
            selections.Position = position;
            executeCommandBase(click, new ChangeMultipleChordPositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPositionNewMeasure(NodeClick click)
        {
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            executeCommandBase(click, new ChangeMultipleChordPositionNewMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
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
            executeCommandBase(click, new AddSingleNoteEffectBld(selections), UpdateType.UpdateMeasure, false);
        }

        public void executeRemoveEffectFromNote(NodeClick click, IEffect effect)
        {
            selections.SelectedEffect = effect;
            executeCommandBase(click, new RemoveNoteEffectBld(selections), UpdateType.UpdateMeasure, false);
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
            executeCommandBase(click, new AddMultiNoteEffectBld(selections), UpdateType.UpdateMeasure, false);
        }

        public void executeAddMeasureToPart(NodeClick click, int position)
        {
            //in the future, add selected bpm and time sig controls that auto update the selections, but for now,
            // simply use the part's
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = position;

            executeCommandBase(click, new AddMeasureToPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMeasure(NodeClick click)
        {
            executeCommandBase(click, new RemoveMeasureFromPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMultipleMeasures(NodeClick click)
        {
            executeCommandBase(click, new RemoveMultipleMeasuresFromPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasurePosition(NodeClick click, int position)
        {
            selections.Position = position;
            executeCommandBase(click, new ChangeMeasurePositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMultipleMeasurePosition(NodeClick click, int position)
        {
            selections.Position = position;
            executeCommandBase(click, new ChangeMultipleMeasurePositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasureTimeSigFromMenu(NodeClick click, int beats, NoteLength type)
        {
            selections.NumBeats = beats;
            selections.BeatType = type;

            executeChangeMeasureTimeSig(click);
        }

        public void executeChangeMeasureTimeSig(NodeClick click)
        {
            executeCommandBase(click, new ChangeMeasureBpmBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasureBpmFromMenu(NodeClick click, int bpm)
        {
            selections.BPM = bpm;
            executeChangeMeasureBpm(click);
        }

        public void executeChangeMeasureBpm(NodeClick click)
        {
            executeCommandBase(click, new ChangeMeasureBpmBld(selections), UpdateType.UpdateMeasure, false);
        }
    }
}
