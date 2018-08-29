using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public class NotificationField<T> : BaseViewModel
    {
        private Action<T> action;
        private T val;
        public T Value
        {
            get { return val; }
            set
            {
                action?.Invoke(value);
                SetProperty(ref val, value);
            }
        }

        public NotificationField() { Value = default(T); }

        public NotificationField(Action<T> ac)
        {
            Value = default(T);
            action = ac;
        }

        public void setBackingField(T new_val) { val = new_val; }
    }
}
