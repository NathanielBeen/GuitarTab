using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class AdminNavigationBar : BaseViewModel
    {
        private AdminMode mode;
        public AdminMode Mode
        {
            get { return mode; }
            set
            {
                SetProperty(ref mode, value);
                ModeChanged?.Invoke(this, value);
            }
        }

        public AdminNavigationBar()
        {
            Mode = AdminMode.MAIN;
        }

        public event EventHandler<AdminMode> ModeChanged;
    }
}
