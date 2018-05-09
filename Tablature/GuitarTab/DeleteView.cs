using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class DeleteView : BaseViewModel
    {
        private CommandSelections selections;
        private GuiCommandExecutor executor;

        public ImageBrush Image { get; set; }

        private ICommand deleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get { return deleteItemCommand ?? (deleteItemCommand = new RelayCommand(() => handleButtonClick())); }
        }

        public DeleteView(CommandSelections s, GuiCommandExecutor e, string uri)
        {
            selections = s;
            executor = e;
            Image = getImageBrush(uri);
        }

        public ImageBrush getImageBrush(string uri)
        {
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative));
            return brush;
        }

        public void handleButtonClick()
        {
            if (selections.SelectedNote.Count > 1)
            {
                executor.executeRemoveMultipleNotes();
            }
            else if (selections.SelectedNote.Count == 1)
            {
                executor.executeRemoveNote();
            }
            else if (selections.SelectedChord.Count > 1)
            {
                executor.executeRemoveMultipleChords();
            }
            else if (selections.SelectedChord.Count == 1)
            {
                executor.executeRemoveChord();
            }
            else if (selections.SelectedMeasure.Count > 1)
            {
                executor.executeRemoveMultipleMeasures();
            }
            else if (selections.SelectedMeasure.Count == 1)
            {
                executor.executeRemoveMeasure();
            }
        }
    }
}
