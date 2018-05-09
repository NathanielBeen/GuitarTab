using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public delegate void BoolMenuDelegate(bool new_value);
    public delegate void IntMenuDelegate(int new_value);
    public delegate void DoubleMenuDelegate(double new_value);
    public delegate void StringMenuDelegate(string new_value);
    public delegate void EffectTypeMenuDelegate(EffectType new_value);

    public interface IPropertyMenuItem : INotifyPropertyChanged
    {
        string Name { get; }

        void resetToDefault();
        void executeChange();
    }

    public class BoolMenuItem : BaseViewModel, IPropertyMenuItem
    {
        public string Name { get; }

        private bool bool_value;
        public bool Value
        {
            get { return bool_value; }
            set { SetProperty(ref bool_value, value); }
        }

        private bool def_value;
        private BoolMenuDelegate del;

        public BoolMenuItem(string name, bool def, BoolMenuDelegate d)
        {
            Name = name;
            def_value = def;
            Value = def;
            del = d;
        }

        public void resetToDefault() { Value = def_value; }

        public void executeChange() { del?.Invoke(Value); }
    }

    public class IntegerMenuItem : BaseViewModel, IPropertyMenuItem
    {
        public string Name { get; }

        private int int_value;
        public int Value
        {
            get { return int_value; }
            set { SetProperty(ref int_value, value); }
        }

        private int def_value;
        private IntMenuDelegate del;

        public IntegerMenuItem(string name, int def, IntMenuDelegate d)
        {
            Name = name;
            def_value = def;
            Value = def;
            del = d;
        }

        public void resetToDefault() { Value = def_value; }

        public void executeChange() { del?.Invoke(Value); }
    }

    public class DoubleMenuItem : BaseViewModel, IPropertyMenuItem
    {
        public string Name { get; }

        private double dbl_value;
        public double Value
        {
            get { return dbl_value; }
            set { SetProperty(ref dbl_value, value); }
        }

        private double def_value;
        private DoubleMenuDelegate del;

        public DoubleMenuItem(string name, double def, DoubleMenuDelegate d)
        {
            Name = name;
            def_value = def;
            Value = def;
            del = d;
        }

        public void resetToDefault() { Value = def_value; }

        public void executeChange() { del?.Invoke(Value); }
    }

    public class DropDownMenuItem : BaseViewModel, IPropertyMenuItem
    {
        public string Name { get; }
        public List<string> Options { get; }

        protected string str_value;
        public virtual string Value
        {
            get { return str_value; }
            set { SetProperty(ref str_value, value); }
        }

        protected string def_value;
        protected StringMenuDelegate del;

        public DropDownMenuItem(string name, string def, List<string> options, StringMenuDelegate d)
        {
            Name = name;
            def_value = def;
            Options = options;
            Value = def;
            del = d;
        }

        public void resetToDefault() { Value = def_value; }

        public void executeChange() { del?.Invoke(Value); }
    }

    public class EffectTypeMenuItem : DropDownMenuItem
    {
        private EffectTypeMenuDelegate effect_del;

        public EffectTypeMenuItem(string name, string def, List<string> options, StringMenuDelegate d, EffectTypeMenuDelegate ed)
            :base(name, def, options, d)
        {
            effect_del = ed;
        }

        public override string Value
        {
            get { return str_value; }
            set
            {
                SetProperty(ref str_value, value);
                EffectType type = EffectTypeExtensions.getEffectTypeFromString(value);
                effect_del?.Invoke(type);
            }
        }
    }
}
