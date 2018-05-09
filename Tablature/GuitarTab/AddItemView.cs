using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    //create a "converter" class that will recieve inputs from both this from Item and the selectionstate. When one is updated,
    //it will convert to the othe. (the other will only actually be set if the value has changed, thus avoiding a spiralling call
    //of set methods. The conversion will occur outside of the setters, and setters will call conversion

    //this will reference the converter item and not contain an actual reference to item
    public class AddItemView : BaseViewModel
    {
        private MouseStateConverter converter;

        public Dictionary<AddItem, ImageBrush> Images { get; set; }

        public AddItem Item
        {
            get { return converter.GuiSelectionState; }
            set { converter.GuiSelectionState = value; }
        }

        public AddItemView(MouseStateConverter conv, Dictionary<string, string> uri_dict)
        {
            converter = conv;
            Images = getImages(uri_dict);
            Item = AddItem.None;
        }

        public Dictionary<AddItem, ImageBrush> getImages(Dictionary<string, string> uris)
        {
            var image_dict = new Dictionary<AddItem, ImageBrush>();
            foreach (var entry in uris)
            {
                AddItem item = AddItemExtensions.getAddItemFromString(entry.Key);

                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(entry.Value, UriKind.Relative));
                image_dict[item] = brush;
            }

            return image_dict;
        }
    }

    public enum AddItem
    {
        None = 0,
        Measure = 1,
        Rest = 2,
        Note = 3,
        PalmMute = 4,
        Bend = 5,
        PinchHarmonic = 6,
        Vibrato = 7,
        Slide = 8,
        Hopo = 9,
        Tie = 10
    }

    public static class AddItemExtensions
    {
        public static AddItem getAddItemFromString(string input)
        {
            return (from enum_value in Enum.GetValues(typeof(AddItem)).Cast<AddItem>()
                    where Enum.GetName(typeof(AddItem), enum_value) == input
                    select enum_value).FirstOrDefault();
        }

        public static EffectType convertToEffectType(this AddItem item)
        {
            switch (item)
            {
                case AddItem.PalmMute:
                    return EffectType.Palm_Mute;
                case AddItem.Bend:
                    return EffectType.Bend;
                case AddItem.PinchHarmonic:
                    return EffectType.Pinch_Harmonic;
                case AddItem.Vibrato:
                    return EffectType.Vibrato;
                case AddItem.Slide:
                    return EffectType.Slide;
                case AddItem.Hopo:
                    return EffectType.HOPO;
                case AddItem.Tie:
                    return EffectType.Tie;
                default:
                    return EffectType.No_Type;
            }
        }

        public static Selection convertToSelection(this AddItem item)
        {
            switch (item)
            {
                case AddItem.Measure:
                    return Selection.Add_Measure;
                case AddItem.Rest:
                    return Selection.Add_Rest;
                case AddItem.Note:
                    return Selection.Add_Note;
                case AddItem.PalmMute:
                case AddItem.Bend:
                case AddItem.PinchHarmonic:
                case AddItem.Vibrato:
                    return Selection.Add_Effect;
                case AddItem.Slide:
                case AddItem.Hopo:
                case AddItem.Tie:
                    return Selection.Add_Multi_Effect;
                default:
                    return Selection.Standard;
            }
        }
    }
}
