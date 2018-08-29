using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab.API
{
    public enum AdminMode
    {
        MAIN = 0,
        RATING = 1,
        USER = 2,
        TAG = 3,
        SONG = 4
    }

    public class AdminPage : BaseViewModel
    {
        private IViewModeFactory<AdminMode> factory;

        public NotificationField<AdminMode> Mode { get; private set; }
        public BaseViewModel CurrentView { get; set; }

        public AdminPage(IViewModeFactory<AdminMode> fac)
        {
            factory = fac;

            initFields();
            Mode.Value = AdminMode.MAIN;
        }

        private void initFields() { Mode = new NotificationField<AdminMode>(handleViewModeChanged); }

        private void handleViewModeChanged(AdminMode new_mode)
        {
            if (new_mode == Mode.Value) { return; }
            CurrentView = factory.createView(new_mode);
        }
    }
}

