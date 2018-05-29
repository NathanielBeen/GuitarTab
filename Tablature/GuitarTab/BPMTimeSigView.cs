using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class BPMTimeSigView : BaseViewModel
    {
        private CommandSelections selections;
        private GuiCommandExecutor executor;
        private Selected selected;

        public int BPM
        {
            get { return selections.BPM; }
            set { selections.BPM = value; }
        }
        public int NumBeats
        {
            get { return selections.NumBeats; }
            set
            {
                if (value >= 1 && value <= 32) { selections.NumBeats = value; }
            }
        }

        public NoteLength BeatType
        {
            get { return selections.BeatType; }
            set { selections.BeatType = value; }
        }

        public Dictionary<NoteLength, ImageBrush> LengthImages { get; set; }
        public ImageBrush IncrementImage { get; set; }
        public ImageBrush DecrementImage { get; set; }

        public ICommand IncrementBPMCommand { get; private set; }
        public ICommand DecrementBPMCommand { get; private set; }     
        public ICommand IncrementNumBeatsCommand { get; private set; }
        public ICommand DecrementNumBeatsCommand { get; private set; }

        public ICommand SetMeasureBPMCommand { get; private set; }
        public ICommand SetMeasureTimeSigCommand { get; private set; }

        public BPMTimeSigView(CommandSelections s, GuiCommandExecutor e, Selected se, Dictionary<string, string> length_uri, Dictionary<string, string> other_uri, int b, int n, NoteLength bl)
        {
            selections = s;
            executor = e;
            selected = se;
            selections.PropertyChanged += handlePropertyChanged;

            BPM = b;
            NumBeats = n;
            BeatType = bl;

            LengthImages = getLengthImages(length_uri);
            getOtherImages(other_uri);
            initCommands();
        }

        public Dictionary<NoteLength, ImageBrush> getLengthImages(Dictionary<string, string> uri_dict)
        {
            var needed_lengths = new List<NoteLength>() { NoteLength.Half, NoteLength.Quarter, NoteLength.Eighth, NoteLength.Sixeteenth, NoteLength.ThirtySecond };
            var image_dict = new Dictionary<NoteLength, ImageBrush>();

            foreach (var entry in uri_dict)
            {
                NoteLength length = NoteLengthExtensions.getNoteLengthFromString(entry.Key);
                if (needed_lengths.Contains(length))
                {
                    var brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(entry.Value, UriKind.Relative));

                    image_dict[length] = brush;
                }
            }

            return image_dict;
        }

        public void getOtherImages(Dictionary<string, string> uri_dict)
        {
            var inc_brush = new ImageBrush();
            inc_brush.ImageSource = new BitmapImage(new Uri(uri_dict["incButton"], UriKind.Relative));
            IncrementImage = inc_brush;

            var dec_brush = new ImageBrush();
            dec_brush.ImageSource = new BitmapImage(new Uri(uri_dict["decButton"], UriKind.Relative));
            DecrementImage = dec_brush;
        }

        public void initCommands()
        {
            IncrementBPMCommand = new RelayCommand(handleIncrementBpm);
            DecrementBPMCommand = new RelayCommand(handleDecrementBpm);
            IncrementNumBeatsCommand = new RelayCommand(handleIncrementNumBeats);
            DecrementNumBeatsCommand = new RelayCommand(handleDecrementNumBeats);

            SetMeasureBPMCommand = new RelayCommand(handleSetMeasureBPM);
            SetMeasureTimeSigCommand = new RelayCommand(handleSetMeasureTimeSig);
        }

        public void handleIncrementBpm() { BPM += 1; }

        public void handleDecrementBpm() { BPM -= 1; }

        public void handleIncrementNumBeats() { NumBeats += 1; }

        public void handleDecrementNumBeats() { NumBeats -= 1; }

        public void handleSetMeasureBPM()
        {
            var click = new NodeClick(default(Point));
            selected.populateNodeClick(click);

            executor.executeChangeMeasureBpm(click);
            selected.populateFromClick(click);
        }

        public void handleSetMeasureTimeSig()
        {
            var click = new NodeClick(default(Point));
            selected.populateNodeClick(click);

            executor.executeChangeMeasureTimeSig(click);
            selected.populateFromClick(click);
        }

        public void handlePropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CommandSelections.BPM)) { onPropertyChanged(nameof(BPM)); }
            else if (args.PropertyName == nameof(CommandSelections.NumBeats)) { onPropertyChanged(nameof(NumBeats)); }
            else if (args.PropertyName == nameof(CommandSelections.BeatType)) { onPropertyChanged(nameof(BeatType)); }
        }
    }
}
