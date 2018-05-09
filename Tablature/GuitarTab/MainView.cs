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

        public MainView(LengthView length, DeleteView delete, AddItemView add_item, MouseCanvasView canvas_view, PropertyMenuView property, FretMenuView fret, 
            GuiCommandExecutor executor, MouseHandler handler)
        {
            LengthView = length;
            DeleteView = delete;
            AddItemView = add_item;
            CanvasView = canvas_view;
            PropertyMenuView = property;
            FretMenuView = fret;

            setHandlers(executor, handler);
        }

        public void setHandlers(GuiCommandExecutor executor, MouseHandler handler)
        {
            executor.FretMenuLaunched += launchNewFretMenu;
            handler.PropertyMenuChanged += launchNewPropertyMenu;
        }

        public void launchNewPropertyMenu(object sender, PropertyMenuEventArgs args)
        {
            PropertyMenuView.Selected = args.Selected;
            PropertyMenuView.Visible = System.Windows.Visibility.Visible;
        }

        public void launchNewFretMenu(object sender, FretMenuEventArgs args)
        {
            FretMenuView.ContinueDelegate = args.Command;
            FretMenuView.Position = args.CurrentPosition;
            FretMenuView.Visible = System.Windows.Visibility.Visible;
        }
    }
}
