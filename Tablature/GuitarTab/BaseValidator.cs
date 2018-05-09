using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class BaseValidator : BaseViewModel, INotifyDataErrorInfo
    {
        private Dictionary<string, ICollection<string>> error_dict;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void RaiseErrorsChanged(string prop_name)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop_name));
        }

        protected bool ValidateProperty(string prop_name, string value, Func<string, ICollection<string>> method)
        {
            ICollection<string> errors = method(value);
            if (errors.Any())
            {
                error_dict[prop_name] = errors;
                RaiseErrorsChanged(prop_name);
                return false;
            }
            else if (error_dict.ContainsKey(prop_name))
            {
                error_dict.Remove(prop_name);
                RaiseErrorsChanged(prop_name);
            }
            return true;
        }

        public IEnumerable GetErrors(string prop_name)
        {
            if (!error_dict.ContainsKey(prop_name)) { return null; }

            return error_dict[prop_name];
        }

        public bool HasErrors
        {
            get { return error_dict.Count > 0; }
        }
    }
}
