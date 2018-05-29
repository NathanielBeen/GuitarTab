using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            onPropertyChanged(propertyName);
            return true;
        }

        protected void onPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class MainView : BaseViewModel
    {
        public LengthView LengthView { get; }
        public DeleteView DeleteView { get; }
        public AddItemView AddItemView { get; }
        public MouseCanvasView CanvasView { get; }
        public PropertyMenuView PropertyMenuView { get; }
        public FretMenuView FretMenuView { get; }
        public NoteSelectView NoteSelectView { get; }
        public BPMTimeSigView BPMTimeSigView { get; }

        public MainView(LengthView length, DeleteView delete, AddItemView add_item, MouseCanvasView canvas_view, PropertyMenuView property, FretMenuView fret, NoteSelectView select,
            BPMTimeSigView bpm, GuiCommandExecutor executor, MouseHandler handler)
        {
            LengthView = length;
            DeleteView = delete;
            AddItemView = add_item;
            CanvasView = canvas_view;
            PropertyMenuView = property;
            FretMenuView = fret;
            BPMTimeSigView = bpm;
            NoteSelectView = select;

            setHandlers(executor, handler);
        }

        public void setHandlers(GuiCommandExecutor executor, MouseHandler handler)
        {
            executor.NoteSelectMenuLaunched += handler.handleNoteSelect;
            executor.FretMenuLaunched += launchNewFretMenu;
            handler.PropertyMenuChanged += launchNewPropertyMenu;
            handler.NoteSelectLaunched += launchNoteSelectMenu;
            handler.NoteSelectEnd += endNoteSelect;
        }

        public void launchNewPropertyMenu(object sender, PropertyMenuEventArgs args) { PropertyMenuView.launchMenu(args.Click); }

        public void launchNewFretMenu(object sender, FretMenuEventArgs args) { FretMenuView.launchMenu(args.Command, args.Click); }

        public void launchNoteSelectMenu(object sender, NoteSelectLaunchEventArgs args)
        {
            NoteSelectView.launchNoteSelect(args);
        }

        public void endNoteSelect(object sender, NoteSelectEndEventArgs args)
        {
            NoteSelectView.noteSelected(args.Click);
        }
    }
}
