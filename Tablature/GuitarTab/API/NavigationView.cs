using GuitarTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuitarTab.API
{
    public class NavigationView : BaseViewModel
    {
        private Credentials credentials;

        public NotificationField<bool> LoggedIn { get; private set; }
        public NotificationField<bool> LoggedInAdmin { get; private set; }

        public ICommand PopupCommand { get; set; }
        public ICommand AdminCommand { get; set; }

        public NavigationView(Credentials cred)
        {
            credentials = cred;
            credentials.LoggedIn += handleLogin;
            credentials.LoggedOut += handleLogout;

            initFields();
            initCommands();
        }

        private void initFields()
        {
            LoggedIn = new NotificationField<bool>();
            LoggedInAdmin = new NotificationField<bool>();
        }

        private void initCommands()
        {
            PopupCommand = new RelayCommand(() => PopupLaunched?.Invoke(this, null));
            AdminCommand = new RelayCommand(() => AdminSelected?.Invoke(this, null));
        }

        private void handleLogin(object sender, EventArgs args)
        {
            LoggedIn.Value = (credentials.CurrentUser != null);
            LoggedInAdmin.Value = (credentials.CurrentUser != null && credentials.CurrentUser.Type != 0);
        }

        private void handleLogout(object sender, EventArgs args)
        {
            LoggedIn.Value = false;
            LoggedInAdmin.Value = false;
        }

        public event EventHandler PopupLaunched;
        public event EventHandler AdminSelected;
    }
}
