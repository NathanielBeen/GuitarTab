using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GuitarTab
{
    public class FretMenuView : BaseViewModel
    {
        public const int MIN_FRET = 0;
        public const int MAX_FRET = 26;

        public ContinueCommandDelegate ContinueDelegate { get; set; }
        public CancelDialogueDelegate CancelDelegate { get; set; }

        private string fret;
        public string Fret
        {
            get { return fret; }
            set
            {
                if (validateFretInput(value))
                {
                    SetProperty(ref fret, value);
                }
            }
        }

        private Point position;
        public Point Position
        {
            get { return position; }
            set { SetProperty(ref position, value); }
        }

        private Visibility visible;
        public Visibility Visible
        {
            get { return visible; }
            set { SetProperty(ref visible, value); }
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

        public NodeClick Click { get; set; }

        public FretMenuView()
        {
            Fret = "0";
            Visible = Visibility.Collapsed;
        }

        public bool validateFretInput(string input)
        {
            return (int.TryParse(input, out int int_input) && int_input >= MIN_FRET && int_input <= MAX_FRET);
        }

        public void resetFields()
        {
            Fret = "0";
            Position = default(Point);
            Visible = Visibility.Collapsed;
            ContinueDelegate = null;
            Click = null;
        }

        public void handleSubmit()
        {
            if(int.TryParse(fret, out int conv_fret) && Click != null)
            {
                ContinueDelegate?.Invoke(Click, conv_fret);
                resetFields();
            }
            else { Fret = "0"; }
        }

        public void handleClose()
        {
            CancelDelegate?.Invoke();
            resetFields();
        }

        public void launchMenu(ContinueCommandDelegate command, NodeClick click)
        {
            ContinueDelegate = command;
            Click = click;
            Position = click.Point;
            Visible = Visibility.Visible;
        }
    }
}
