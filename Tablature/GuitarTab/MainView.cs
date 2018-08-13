using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab
{
    public class MainView : BaseViewModel
    {
        private ViewType view_type;
        public ViewType ViewType
        {
            get { return view_type; }
            set { SetProperty(ref view_type, value); }
        }

        public BaseViewModel ControlView { get; set; }
        public Part Part { get; set; }

        private StartingFactory starting_factory;
        private IViewModelFactory view_factory;

        public ICommand ChangeToEditingViewCommand { get; set; }
        public ICommand ChangeToViewingViewCommand { get; set; }
        public event EventHandler<ViewModeChangedEventArgs> ViewModeChanged;

        public MainView( Part part)
        {
            ViewType = ViewType.Viewing;
            Part = part;
            starting_factory = new StartingFactory();

            initCommands();
        }

        public void initCommands()
        {
            ChangeToEditingViewCommand = new RelayCommand(handleChangeToEditView);
            ChangeToViewingViewCommand = new RelayCommand(handleChangeToViewingView);
        }

        public void handleChangeToEditView()
        {
            ViewType = ViewType.Editing;
            runEditingFactory();
            refreshPart();
        }

        public void handleChangeToViewingView()
        {
            ViewType = ViewType.Viewing;
            runViewingFactory();
            refreshPart();
        }

        public void runEditingFactory()
        {
            view_factory = new EditingViewModelFactory();
            view_factory.runFactory(starting_factory, Part);
            ControlView = view_factory.getMainView();
            ViewModeChanged?.Invoke(this, new ViewModeChangedEventArgs(ViewType.Editing));
        }

        public void runViewingFactory()
        {
            view_factory = new ViewingViewModelFactory();
            view_factory.runFactory(starting_factory, Part);
            ControlView = view_factory.getMainView();
            ViewModeChanged?.Invoke(this, new ViewModeChangedEventArgs(ViewType.Viewing));
        }

        public void refreshPart() { Part = view_factory?.getPart(); }
    }

    public class ViewModeChangedEventArgs : EventArgs
    {
        public ViewType Type { get; }

        public ViewModeChangedEventArgs(ViewType type) { Type = type; }
    }

    public enum ViewType
    {
        Editing,
        Viewing
    }
}
