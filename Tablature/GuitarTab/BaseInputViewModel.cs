using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class BaseInputViewModel : BaseViewModel
    {
        public void setIntProperty(ref string prop, string value, int min, int max, ref string error)
        {
            if (value == String.Empty) { error = "required field"; }
            else if (Int32.TryParse(value, out int converted))
            {
                if (converted >= min && converted <= max) { error = ""; }
                else { error = "number must be between "+min+" and "+max; }
            }
            else { error = "must be a number"; }
            prop = value;
        }

        public void setDoubleProperty(ref string prop, string value, double min, double max, ref string error)
        {
            if (value == String.Empty) { error = "required field"; }
            else if (Double.TryParse(value, out double converted))
            {
                if (converted >= min && converted <= max) { error = ""; }
                else { error = "number must be between " + min + " and " + max; }
            }
            else { error = "must be a number"; }
            prop = value;
        }

        public void setTimeSigNoteLengthProperty(ref string prop, string value, ref string error)
        {
            if (value == String.Empty) { error = "required field"; }
            else if (Int32.TryParse(value, out int converted))
            {
                NoteLength length = NoteLengthExtensions.getNoteLengthFromVisualLength(converted);
                if (length == NoteLength.None) { error = "must be 2, 4, 8, 16, or 32"; }
                else { error = ""; }
            }
            else { error = "must be a number"; }
            prop = value;
        }

        public void setStringProperty(ref string prop, string value, ref string error)
        {
            var regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");

            if (value == String.Empty) { error = "required field"; }
            else if (regex.IsMatch(value))
            {
                if (value.Length < 255) { error = ""; }
                else { error = "too long"; }
            }
            else { error = "must not contain special chars or start/end with spaces"; }
            prop = value;
        }

        public void setNonRequiredStringProperty(ref string prop, string value, ref string error)
        {
            var regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");

            if (regex.IsMatch(value))
            {
                if (value.Length < 255) { error = ""; }
                else { error = "too long"; }
            }
            else { error = "must not contain special chars or start/end with spaces"; }
            prop = value;
        }
    }
}
