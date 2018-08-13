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

        public const int WIDTH = 190;
        public const int HEIGHT = 70;

        public ContinueCommandDelegate ContinueDelegate { get; set; }

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
            Left = 0;
            Top = 0;
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
            resetFields();
        }

        public void launchMenu(ContinueCommandDelegate command, NodeClick click)
        {
            ContinueDelegate = command;
            Click = click;
            Left = Math.Max(0, (int)click.Point.X - WIDTH/2);
            Top = (int)click.Point.Y + HEIGHT/2;
            Visible = Visibility.Visible;
        }
    }
}
