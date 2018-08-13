using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    class InstrumentProperties : BasePropertyMenu
    {
        public const int MIN_STRINGS = 4;
        public const int MAX_STRINGS = 8;

        private Part part;

        private InstrumentType instrument;
        public string Instrument
        {
            get { return instrument.getStringFromInstrumentType(); }
            set { SetProperty(ref instrument, InstrumentTypeExtensions.getInstrumentTypeFromString(value)); }
        }

        private string strings;
        public string Strings
        {
            get { return strings; }
            set
            {
                string error = StringError;
                setIntProperty(ref strings, value, MIN_STRINGS, MAX_STRINGS, ref error);
                StringError = error;
            }
        }

        private string string_error;
        public string StringError
        {
            get { return string_error; }
            set { SetProperty(ref string_error, value); }
        }

        public List<string> Instruments { get; }

        public InstrumentProperties(PartTreeNode n, GuiCommandExecutor ex, NodeClick cl)
            :base(cl, ex)
        {
            part = n.getPart();

            Instrument = part.InstrumentInfo.Type.getStringFromInstrumentType();
            Instruments = InstrumentTypeExtensions.getAllInstrumentTypeStrings();

            Strings = part.InstrumentInfo.Strings.ToString();
        }

        public override void resetToDefault()
        {
            Instrument = part.InstrumentInfo.Type.getStringFromInstrumentType();
            Strings = part.InstrumentInfo.Strings.ToString();
        }

        public override void submitChanges()
        {
            if (StringError != String.Empty || !Int32.TryParse(strings, out int strings_i)) { return; }
            if (strings_i != part.InstrumentInfo.Strings || instrument != part.InstrumentInfo.Type)
            {
                executor.executeChangeInstrumentInfoFromProp(getClickCopy(), instrument, strings_i);
            }
        }
    }
}
