using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class DeleteView : BaseViewModel
    {
        private GuiCommandExecutor executor;
        private Selected selected;

        public ImageBrush Image { get; set; }

        private ICommand deleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get { return deleteItemCommand ?? (deleteItemCommand = new RelayCommand(() => handleButtonClick())); }
        }

        public DeleteView(GuiCommandExecutor e, Selected s, string uri)
        {
            executor = e;
            selected = s;
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
            var click = new ReleaseClick(default(Point));
            selected.populateNodeClick(click);

            if(click.multipleNotes())
            {
                executor.executeRemoveMultipleNotes(click);
            }
            else if (click.anyNote())
            {
                executor.executeRemoveNote(click);
            }
            else if (click.multipleChords())
            {
                executor.executeRemoveMultipleChords(click);
            }
            else if (click.anyChord())
            {
                executor.executeRemoveChord(click);
            }
            else if (click.multipleMeasures())
            {
                executor.executeRemoveMultipleMeasures(click);
            }
            else if (click.anyMeasure())
            {
                executor.executeRemoveMeasure(click);
            }
        }
    }
}
