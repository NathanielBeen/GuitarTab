using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class CreateSongView : BaseViewModel
    {
        public StringInputField Name { get; private set; }
        public StringInputField Album { get; private set; }
        public StringInputField Artist { get; private set; }

        public IntInputField BPM { get; private set; }
        public IntInputField BeatNum { get; private set; }
        public BeatTypeInputField BeatType { get; private set; }

        public ICommand SubmitCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        public CreateSongView()
        {
            initFields();
            initCommands();
        }

        private void initFields()
        {
            Name = new StringInputField("Name", 0, 32);
            Album = new StringInputField("Album", 0, 32);
            Artist = new StringInputField("Artist", 0, 32);
            BPM = new IntInputField("BPM", 20, 300);
            BeatNum = new IntInputField("Number of Beats", 1, 32);
            BeatType = new BeatTypeInputField("Beat Type", new int[] { 2, 4, 8, 16, 32 });
        }

        private void initCommands()
        {
            SubmitCommand = new RelayCommand(handleSubmit);
            ResetCommand = new RelayCommand(handleReset);
        }

        private void handleSubmit()
        {
            //do a thing in a bit
        }

        private void handleReset()
        {
            Name.clearField();
            Album.clearField();
            Artist.clearField();
            BPM.clearField();
            BeatNum.clearField();
            BeatType.clearField();
        }
    }
}
