using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace API
{
    public class BaseErrorViewModel : BaseViewModel
    {
        private string error;
        public string Error
        {
            get { return error; }
            set { SetProperty(ref error, value); }
        }
    }

    public abstract class InputField<T> : BaseViewModel
    {
        public string Name { get; }

        protected string value;
        public string Value
        {
            get { return value; }
            set { setField(value); }
        }

        protected string error;
        public string Error
        {
            get { return error; }
            set { SetProperty(ref error, value); }
        }

        public InputField(string name)
        {
            Name = name;
        }

        protected abstract void setField(string value);
        public abstract T getConvertedField();

        public void clearField()
        {
            Value = string.Empty;
            Error = string.Empty;
        }
    }

    public class IntInputField : InputField<int>
    {
        private int min;
        private int max;

        public IntInputField(string name, int min, int max)
            :base(name)
        {
            this.min = min;
            this.max = max;
        }

        protected override void setField(string val)
        {
            if (value == string.Empty) { Error = "required field"; }
            else if (Int32.TryParse(val, out int converted))
            {
                if (converted >= min && converted <= max) { Error = ""; }
                else { Error = "number must be between " + min + " and " + max; }
            }
            else { Error = "must be a number"; }
            value = val;
            onPropertyChanged(nameof(Value));
        }

        public override int getConvertedField()
        {
            return Int32.Parse(Value);
        }
    }

    public class DoubleInputField : InputField<double>
    {
        private double min;
        private double max;

        public DoubleInputField(string name, double min, double max)
            :base(name)
        {
            this.min = min;
            this.max = max;
        }

        protected override void setField(string val)
        {
            if (value == string.Empty) { Error = "required field"; }
            else if (Double.TryParse(val, out double converted))
            {
                if (converted >= min && converted <= max) { Error = ""; }
                else { Error = "number must be between " + min + " and " + max; }
            }
            else { Error = "must be a number"; }
            value = val;
            onPropertyChanged(nameof(Value));
        }

        public override double getConvertedField()
        {
            return Double.Parse(Value);
        }
    }

    public class StringInputField : InputField<string>
    {
        protected Regex regex;
        protected int min_length;
        protected int max_length;

        public StringInputField(string name, int min, int max)
            :base(name)
        {
            regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");
            min_length = min;
            max_length = max;
        }

        protected override void setField(string val)
        {
            if (val == string.Empty) { Error = "required field"; }
            else if (regex.IsMatch(val))
            {
                if (val.Length >= min_length || val.Length <= max_length) { Error = ""; }
                else { Error = "must have a length between " + min_length + " and " + max_length; }
            }
            else { Error = "must not contain special chars or start/end with spaces"; }
            value = val;
            onPropertyChanged(nameof(Value));
        }

        public override string getConvertedField()
        {
            return Value;
        }
    }

    public class NonRequiredStringInputField : StringInputField
    {
        public NonRequiredStringInputField(string name, int min, int max) : base(name, min, max) { }

        protected override void setField(string val)
        {
            if (regex.IsMatch(val))
            {
                if (val.Length >= min_length && val.Length <= max_length) { Error = ""; }
                else { Error = "length must be between " + min_length + " and " + max_length; }
            }
            else { Error = "must not contain special chars or start/end with spaces"; }
            value = val;
            onPropertyChanged(nameof(Value));
        }
    }
}
