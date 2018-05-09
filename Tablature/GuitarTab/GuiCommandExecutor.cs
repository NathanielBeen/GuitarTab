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

        public void executeCommandBase(IActionBuilder builder, UpdateType update, bool rebar)
        {
            executor.executeCommand(builder);
            runUpdate(update);
            if (rebar) { Updater.rebarMeasures(selections.SelectedPart, selections.SelectedMeasure); }
            selections.Clear();
            m_selections.EventHandled = true;
        }

        public void executeFretMenuBase(Action<int> action)
        {
            var args = new FretMenuEventArgs(m_selections.SelectedPoint, action);
            FretMenuLaunched?.Invoke(this, args);
            m_selections.EventHandled = true;
        }

        public void runUpdate(UpdateType update)
        {
            switch (update)
            {
                case UpdateType.UpdatePart:
                    Updater.updatePartBounds(selections.SelectedPart);
                    break;
                case UpdateType.UpdateMeasuresAtAndAfter:
                    Updater.updateMeasureBoundsAtAndAfter(selections.SelectedPart, selections.getFirstMeasureByPosition());
                    break;
                case UpdateType.UpdateMeasure:
                    Updater.updateMeasureBounds(selections.SelectedPart, selections.SelectedMeasure.FirstOrDefault());
                    break;
                case UpdateType.UpdateChord:
                    Updater.updateChordBounds(selections.SelectedPart, selections.SelectedMeasure.First(), selections.SelectedChord.FirstOrDefault() as NoteChord);
                    break;
            }
        }

        public void executeInitPart(int bpm, int num_beats, NoteLength beat_type)
        {
            //once menus are put in to init files, replace below
            selections.BPM = bpm;
            selections.NumBeats = num_beats;
            selections.BeatType = beat_type;
            selections.Position = 0;

            var builder = new InitPartBld(selections);
            executor.executeCommand(builder);

            Updater.setTreePart(selections.SelectedPart);

            executeCommandBase(new AddMeasureToPartBld(selections), UpdateType.UpdatePart, false);
        }

        public void executeAddRestChordToMeasure(int position)
        {
            selections.Position = position;
            executeCommandBase(new AddRestChordToMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddRestChordToPart()
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();

            executeCommandBase(new AddRestChordToPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToChord()
        {
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);

            executeFretMenuBase(continueAddNoteToChord);
        }

        public void continueAddNoteToChord(int fret)
        {
            selections.Fret = fret;
            executeCommandBase(new AddNoteToChordBld(selections), UpdateType.UpdateChord, false);
        }

        public void executeAddNoteToMeasure(int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);

            executeFretMenuBase(continueAddNoteToMeasure);
        }

        public void continueAddNoteToMeasure(int fret)
        {
            selections.Fret = fret;
            executeCommandBase(new AddNoteToMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddNoteToPart()
        {
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);

            executeFretMenuBase(continueAddNoteToPart);
        }

        public void continueAddNoteToPart(int fret)
        {
            selections.Fret = fret;
            executeCommandBase(new AddNoteToPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveNote()
        {
            executeCommandBase(new RemoveNoteFromChordBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleNotes()
        {
            executeCommandBase(new RemoveMultipleNotesFromChordBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveChord()
        {
            executeCommandBase(new RemoveChordFromMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeRemoveMultipleChords()
        {
            executeCommandBase(new RemoveMultipleChordsFromMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordLengthFromMenu(Length new_length)
        {
            selections.SelectedLength = new_length;
            executeChangeChordLength();
        }

        public void executeChangeChordLength()
        {
            executeCommandBase(new ChangeChordLengthBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNoteStringFromPosition()
        {
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeChangeNoteString();
        }

        public void executeChangeNoteStringFromMenu(int str)
        {
            selections.String = str;
            executeChangeNoteString();
        }

        public void executeChangeNoteString()
        {
            executeCommandBase(new ChangeNoteStringBld(selections), UpdateType.UpdateChord, false);
        }

        public void executeChangeNoteFret()
        {
            executeFretMenuBase(continueChangeNoteFret);
        }

        public void continueChangeNoteFret(int fret)
        {
            selections.Fret = fret;
            executeCommandBase(new ChangeNoteFretBld(selections), UpdateType.UpdateChord, false);
            Updater.updateNoteDrawing(selections.SelectedPart, selections.SelectedMeasure.FirstOrDefault(), selections.SelectedChord.FirstOrDefault(), selections.SelectedNote.FirstOrDefault());
        }

        public void executeChangeNotePosition()
        {
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeCommandBase(new ChangeNotePositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewChord(int position)
        {
            selections.Position = position;
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeCommandBase(new ChangeNotePositionNewChordBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeNotePositionNewMeasure()
        {
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            selections.String = info.Position.getStringFromYPosition((int)m_selections.SelectedPoint.Y);
            executeCommandBase(new ChangeNotePositionNewMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPosition(int position)
        {
            selections.Position = position;
            executeCommandBase(new ChangeChordPositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeChordPositionNewMeasure()
        {
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            executeCommandBase(new ChangeChordPositionNewMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPosition(int position)
        {
            selections.Position = position;
            executeCommandBase(new ChangeMultipleChordPositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeChangeMultipleChordPositionNewMeasure()
        {
            selections.Position = selections.SelectedPart.ModelCollection.Count();
            executeCommandBase(new ChangeMultipleChordPositionNewMeasureBld(selections), UpdateType.UpdateMeasuresAtAndAfter, true);
        }

        public void executeAddEffectToNoteProp(EffectType type)
        {
            selections.SelectedEffectType = type;
            executeAddEffectToNote();
        }

        public void executeAddBendToNoteProp(double amount, bool returns)
        {
            selections.BendAmount = amount;
            selections.Returns = returns;
            executeAddEffectToNote();
        }

        public void executeAddVibratoToNoteProp(bool wide)
        {
            selections.Wide = wide;
            executeAddEffectToNote();
        }

        public void executeAddEffectToNote()
        {
            executeCommandBase(new AddChordToMeasureBld(selections), UpdateType.UpdateMeasure, false);
        }

        public void executeRemoveEffectFromNote(IEffect effect)
        {
            selections.SelectedEffect = effect;
            executeCommandBase(new RemoveNoteEffectBld(selections), UpdateType.UpdateMeasure, false);
        }


        public void executeAddSlideToNoteProp(Note first, Note second, bool legato)
        {
            selections.Legato = legato;
            executeAddMultiEffectToNoteProp(first, second, EffectType.Slide);
        }

        public void executeAddMultiEffectToNoteProp(Note first, Note second, EffectType type)
        {
            selections.SelectedNote.Clear();
            selections.SelectedNote.Add(first);
            selections.SelectedNote.Add(second);
            selections.SelectedEffectType = type;

            executeAddMultiEffectToNotes();
        }

        public void executeAddMultiEffectToNotes()
        {
            executeCommandBase(new AddMultiNoteEffectBld(selections), UpdateType.UpdateMeasure, false);
        }

        public void executeAddMeasureToPart(int position)
        {
            //in the future, add selected bpm and time sig controls that auto update the selections, but for now,
            // simply use the part's
            selections.BPM = selections.SelectedPart.DefaultBPM;
            selections.NumBeats = selections.SelectedPart.TimeSignature.NumberOfBeats;
            selections.BeatType = selections.SelectedPart.TimeSignature.BeatType;
            selections.Position = position;

            executeCommandBase(new AddMeasureToPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMeasure()
        {
            executeCommandBase(new RemoveMeasureFromPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeRemoveMultipleMeasures()
        {
            executeCommandBase(new RemoveMultipleMeasuresFromPartBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasurePosition( int position)
        {
            selections.Position = position;
            executeCommandBase(new ChangeMeasurePositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMultipleMeasurePosition(int position)
        {
            selections.Position = position;
            executeCommandBase(new ChangeMultipleMeasurePositionBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasureTimeSigFromMenu(int beats, NoteLength type)
        {
            selections.NumBeats = beats;
            selections.BeatType = type;

            executeChangeMeasureTimeSig();
        }

        public void executeChangeMeasureTimeSig()
        {
            executeCommandBase(new ChangeMeasureBpmBld(selections), UpdateType.UpdateMeasuresAtAndAfter, false);
        }

        public void executeChangeMeasureBpmFromMenu(int bpm)
        {
            selections.BPM = bpm;
            executeChangeMeasureBpm();
        }

        public void executeChangeMeasureBpm()
        {
            executeCommandBase(new ChangeMeasureBpmBld(selections), UpdateType.UpdateMeasure, false);
        }
    }
}
