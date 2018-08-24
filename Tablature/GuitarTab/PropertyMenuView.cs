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
    public class PropertyMenuView : BaseViewModel, IRecieveDimensionUpdates
    {
        public const int HEIGHT = 70;
        public const int WIDTH = 300;

        private PropertyMenuFactory factory;
        private VisualInfo info;
        private NodeClick ref_click;
        private bool instrument_menu;

        private int scroll;
        private int screen;
        
        private BasePropertyMenu menu;
        public BasePropertyMenu Menu
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

        private int left;
        public int Left
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        private int top;
        public int Top
        {
            get { return top; }
            set { SetProperty(ref top, value); }
        }

        private TreeNode selected_object;
        public TreeNode Selected
        {
            get { return selected_object; }
            set
            {
                if (value == null)
                {
                    SetProperty(ref selected_object, value);
                    Menu = null;
                    return;
                }

                var menu = factory.createPropertyMenu(value, instrument_menu, ref_click);
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

        public PropertyMenuView(PropertyMenuFactory fac, VisualInfo v_info)
        {
            factory = fac;
            Menu = null;
            Visible = Visibility.Collapsed;
            Selected = null;
            ref_click = null;
            Left = 0;
            Top = 0;

            info = v_info;
        }

        public void launchMenu(NodeClick new_click)
        {
            ref_click = new_click;
            Selected = new_click.getFirstSelected();
            Visible = Visibility.Visible;
            Left = Math.Max(0, (int)new_click.Point.X - WIDTH/2);
            //this needs to be improved somehow
            Top = Math.Min((int)new_click.Point.Y + HEIGHT/2 - scroll, screen - HEIGHT*4);
        }

        public void launchPartMenu(NodeClick node, int width, int height, bool instrument)
        {
            ref_click = node;
            instrument_menu = instrument;

            Selected = node.PartNode;
            Visible = Visibility.Visible;
            Left = info.Dimensions.PageWidth / 2 - WIDTH / 2;
            Top = HEIGHT;
        }

        public void handleReset() { Menu?.resetToDefault(); }

        public void handleSubmit()
        {
            Menu?.submitChanges();
            Visible = Visibility.Collapsed;
        }

        public void handleClose() { Visible = Visibility.Collapsed; }

        public void handleDimensionUpdate(int new_val, DimensionType type)
        {
            if (type == DimensionType.ScrollAmount) { scroll = new_val; }
            else if (type == DimensionType.ScreenHeight) { screen = new_val; }
        }
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

        public BasePropertyMenu createPropertyMenu(object obj, bool instrument, NodeClick click)
        {
            if (obj is PartTreeNode && !instrument) { return new PartProperties((PartTreeNode)obj, executor, click); }
            else if (obj is PartTreeNode && instrument) { return new InstrumentProperties((PartTreeNode)obj, executor, click); }
            else if (obj is MeasureTreeNode) { return new MeasureProperties((MeasureTreeNode)obj, executor, click); }
            else if (obj is ChordTreeNode) { return new ChordProperties((ChordTreeNode)obj, executor, click); }
            else if (obj is NoteTreeNode) { return new NoteProperties((NoteTreeNode)obj, executor, click); }
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
