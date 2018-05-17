using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GuitarTab
{
    public class PropertyMenuView : BaseViewModel
    {
        private PropertyMenuFactory factory;
        private NodeClick click;
        
        private IPropertyMenu menu;
        public IPropertyMenu Menu
        {
            get { return menu; }
            set { SetProperty(ref menu, value); }
        }

        private Visibility visible;
        public Visibility Visible
        {
            get { return visible; }
            set { SetProperty(ref visible, value); }
        }

        private TreeNode selected_object;
        public TreeNode Selected
        {
            get { return selected_object; }
            set
            {
                var menu = factory.createPropertyMenu(value.BaseObject, click);
                if (menu != null)
                {
                    SetProperty(ref selected_object, value);
                    Menu = menu;
                }
            }
        }

        private ICommand resetCommand;
        public ICommand ResetCommand
        {
            get { return resetCommand ?? (resetCommand = new RelayCommand(() => handleReset())); }
        }

        private ICommand submitCommand;
        public ICommand SubmitCommand
        {
            get { return submitCommand ?? (submitCommand = new RelayCommand(() => handleSubmit())); }
        }

        private ICommand closeCommand;
        public ICommand CloseCommand
        {
            get { return closeCommand ?? (closeCommand = new RelayCommand(() => handleClose())); }
        }

        public PropertyMenuView(PropertyMenuFactory fac)
        {
            factory = fac;
            Visible = Visibility.Collapsed;
            click = null;
        }

        public void launchMenu(NodeClick new_click)
        {
            click = new_click;
            Selected = click.getFirstSelected();
            Visible = Visibility.Visible;
        }

        public void handleReset() { Menu?.resetToDefault(); }

        public void handleSubmit() { Menu?.submitChanges(); }

        public void handleClose() { Visible = Visibility.Collapsed; }
    }

    public class PropertyMenuFactory
    {
        public NodeClick Click { get; set; }

        private GuiCommandExecutor executor;

        public PropertyMenuFactory(GuiCommandExecutor ex)
        {
            Click = null;
            executor = ex;
        }

        public IPropertyMenu createPropertyMenu(object obj, NodeClick click)
        {
            if (obj is Measure) { return new MeasureProperties((Measure)obj, executor, click); }
            else if (obj is Chord) { return new ChordProperties((Chord)obj, executor, click); }
            else if (obj is Note) { return new NoteProperties((Note)obj, executor, click); }
            else { return null; }
        }
    }

    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string).Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
